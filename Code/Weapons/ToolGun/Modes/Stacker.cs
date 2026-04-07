using Sandbox.UI;
using System.Linq;
using System.Text.Json;

[Icon( "🧱" )]
[ClassName( "stacker" )]
[Group( "Building" )]
public class Stacker : ToolMode
{
	public enum StackDirection
	{
		Up,
		Down,
		Forward,
		Backward,
		Left,
		Right
	}

	[Sync( SyncFlags.FromHost ), Change( nameof( JsonChanged ) )]
	public string CopiedJson { get; set; }

	[Property, Sync, Group( "Placement" )]
	public StackDirection Direction { get; set; } = StackDirection.Up;

	[Property, Sync, Group( "Placement" )]
	public bool AlignWithWorld { get; set; }

	[Property, Sync, Group( "Stacker" )]
	public int Count { get; set; } = 5;

	[Property, Sync, Group( "Stacker" )]
	public Vector3 Offset { get; set; } = Vector3.Zero;

	[Property, Sync, Group( "Stacker" )]
	public Angles RotationStep { get; set; } = Angles.Zero;

	[Property, Sync, Group( "Stacker" )]
	public float Spacing { get; set; } = 0.1f;

	[Property, Sync, Group( "Stacker" )]
	public bool AutoWeld { get; set; }

	[Property, Sync, Group( "Stacker" ), ShowIf( nameof( AutoWeld ), true )]
	public bool WeldToWorld { get; set; }

	DuplicatorSpawner spawner;
	DuplicatorSpawner _previewSpawner;
	GameObject _previewSelection;
	LinkedGameObjectBuilder builder = new() { RejectPlayers = true };

	Rotation _rotationOffset = Rotation.Identity;
	Rotation _spinRotation = Rotation.Identity;
	Rotation _snapRotation = Rotation.Identity;
	bool _isSnapping;
	bool _isRotating;

	public override string Description => "Stack the selected object with a live preview using Offset and RotationStep.";
	public override string PrimaryAction => spawner is not null || _previewSpawner is not null ? "Spawn stack" : null;
	public override string SecondaryAction => null;
	public override string ReloadAction => "Cycle stack count";

	public override void OnCameraMove( Player player, ref Angles angles )
	{
		base.OnCameraMove( player, ref angles );

		if ( _isRotating )
			angles = default;
	}

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		var previewSpawner = spawner ?? GetPreviewSpawner( select );

		_isRotating = previewSpawner is not null && Input.Down( "use" );
		Toolgun.SetIsUsingJoystick( _isRotating );

		var isSnapping = Input.Down( "run" );
		if ( !isSnapping && _isSnapping ) _spinRotation = _snapRotation;
		_isSnapping = isSnapping;

		if ( _isRotating )
		{
			var look = Input.AnalogLook with { pitch = 0 };

			if ( _isSnapping )
			{
				if ( MathF.Abs( look.yaw ) > MathF.Abs( look.pitch ) )
					look.pitch = 0;
				else
					look.yaw = 0;
			}

			_spinRotation = Rotation.From( look ) * _spinRotation;
			Input.Clear( "use" );

			if ( _isSnapping )
			{
				var snapped = _spinRotation.Angles();
				_rotationOffset = snapped.SnapToGrid( 45f );
			}
			else
			{
				_rotationOffset = _spinRotation;
			}

			_snapRotation = _rotationOffset;
			Toolgun.UpdateJoystick( new Angles( look.yaw, look.pitch, 0 ) );
		}

		IsValidState = IsValidTarget( select );

		if ( Input.Pressed( "attack1" ) )
		{
			var spawnPreview = spawner ?? GetPreviewSpawner( select );
			if ( spawnPreview is null || !spawnPreview.IsReady || !IsValidPlacementTarget( select ) )
			{
				return;
			}

			var target = select.GameObject.Network.RootGameObject ?? select.GameObject;
			var selectionAngle = target.WorldTransform;

			var tx = GetPlacementTransform( select, spawnPreview.Bounds );
			DuplicateStack( tx, select.GameObject, selectionAngle );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "reload" ) )
		{
			Count = Count % 8 + 1;
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		DrawPreview();
	}

	void Copy( GameObject obj, Transform selectionAngle, bool additive )
	{
		if ( !additive )
			builder.Clear();

		builder.AddConnected( obj );
		builder.RemoveDeletedObjects();

		var tempDupe = DuplicationData.CreateFromObjects( builder.Objects, selectionAngle );
		CopiedJson = Json.Serialize( tempDupe );

		PlayerData.For( Rpc.Caller )?.AddStat( "tool.stacker.copy" );
	}

	void JsonChanged()
	{
		spawner = null;

		if ( string.IsNullOrWhiteSpace( CopiedJson ) )
			return;

		spawner = DuplicatorSpawner.FromJson( CopiedJson );
	}

	void DrawPreview()
	{
		var select = TraceSelect();
		if ( !IsValidPlacementTarget( select ) )
			return;

		var previewSpawner = spawner ?? GetPreviewSpawner( select );
		if ( previewSpawner is null || !previewSpawner.IsReady )
			return;

		var tx = GetPlacementTransform( select, previewSpawner.Bounds );

		var overlayMaterial = IsProxy ? Material.Load( "materials/effects/duplicator_override_other.vmat" ) : Material.Load( "materials/effects/duplicator_override.vmat" );
		var step = GetStackStep( tx, previewSpawner.Bounds );

		for ( int i = 0; i < Count; i++ )
		{
			var drawTx = tx;
			drawTx.Position += step * i;
			var rotation = Rotation.From( new Angles( RotationStep.pitch * i, RotationStep.yaw * i, RotationStep.roll * i ) );
			drawTx.Rotation = tx.Rotation * rotation;
			previewSpawner.DrawPreview( drawTx, overlayMaterial );
		}
	}

	DuplicatorSpawner GetPreviewSpawner( SelectionPoint select )
	{
		if ( !select.IsValid() || select.IsWorld || select.IsPlayer )
			return null;

		var root = select.GameObject.Network.RootGameObject ?? select.GameObject;
		if ( _previewSelection == root && _previewSpawner is not null )
			return _previewSpawner;

		_previewSelection = root;
		var builder = new LinkedGameObjectBuilder() { RejectPlayers = true };
		builder.AddConnected( root );
		builder.RemoveDeletedObjects();

		var selectionAngle = root.WorldTransform;
		var tempDupe = DuplicationData.CreateFromObjects( builder.Objects, selectionAngle );
		var json = Json.Serialize( tempDupe );
		_previewSpawner = new DuplicatorSpawner( tempDupe, json );

		return _previewSpawner;
	}

	Transform GetPlacementTransform( SelectionPoint select, BBox bounds )
	{
		var target = select.GameObject.Network.RootGameObject ?? select.GameObject;
		var targetTransform = target.WorldTransform;
		var targetRotation = AlignWithWorld ? Rotation.Identity : targetTransform.Rotation;

		var localBounds = GetLocalBounds( target );
		var axis = Direction switch
		{
			StackDirection.Up => Vector3.Up,
			StackDirection.Down => Vector3.Down,
			StackDirection.Forward => Vector3.Forward,
			StackDirection.Backward => Vector3.Backward,
			StackDirection.Left => Vector3.Left,
			StackDirection.Right => Vector3.Right,
			_ => Vector3.Up
		};

		var targetExtent = Direction switch
		{
			StackDirection.Up or StackDirection.Down => localBounds.Extents.z,
			StackDirection.Forward or StackDirection.Backward => localBounds.Extents.x,
			StackDirection.Left or StackDirection.Right => localBounds.Extents.y,
			_ => localBounds.Extents.z
		};

		var startLocalPos = localBounds.Center + axis * targetExtent;
		var startPosition = targetTransform.ToWorld( new Transform( startLocalPos, Rotation.Identity ) ).Position;

		var tx = new Transform();
		tx.Rotation = targetRotation * _rotationOffset;
		var previewOffset = Direction switch
		{
			StackDirection.Up => -bounds.Mins.z,
			StackDirection.Down => bounds.Maxs.z,
			StackDirection.Forward => -bounds.Mins.x,
			StackDirection.Backward => bounds.Maxs.x,
			StackDirection.Left => -bounds.Mins.y,
			StackDirection.Right => bounds.Maxs.y,
			_ => -bounds.Mins.z
		};

		tx.Position = startPosition + tx.Rotation * axis * previewOffset;
		return tx;
	}

	BBox GetLocalBounds( GameObject target )
	{
		var worldBounds = target.GetBounds();
		var corners = new Vector3[8];
		int i = 0;
		for ( int x = 0; x < 2; x++ )
		{
			for ( int y = 0; y < 2; y++ )
			{
				for ( int z = 0; z < 2; z++ )
				{
					corners[i++] = new Vector3(
						x == 0 ? worldBounds.Mins.x : worldBounds.Maxs.x,
						y == 0 ? worldBounds.Mins.y : worldBounds.Maxs.y,
						z == 0 ? worldBounds.Mins.z : worldBounds.Maxs.z
					);
				}
			}
		}

		var localMin = Vector3.Zero;
		var localMax = Vector3.Zero;
		for ( int j = 0; j < corners.Length; j++ )
		{
			var localPoint = target.WorldTransform.PointToLocal( corners[j] );
			if ( j == 0 )
			{
				localMin = localPoint;
				localMax = localPoint;
			}
			localMin = Vector3.Min( localMin, localPoint );
			localMax = Vector3.Max( localMax, localPoint );
		}

		return new BBox( localMin, localMax );
	}

	Vector3 GetStackStep( Transform tx, BBox bounds )
	{
		if ( Offset != Vector3.Zero )
		{
			return AlignWithWorld ? Offset : tx.Rotation * Offset;
		}

		var axis = Direction switch
		{
			StackDirection.Up => AlignWithWorld ? Vector3.Up : tx.Rotation.Up,
			StackDirection.Down => AlignWithWorld ? Vector3.Down : tx.Rotation.Down,
			StackDirection.Forward => AlignWithWorld ? Vector3.Forward : tx.Rotation.Forward,
			StackDirection.Backward => AlignWithWorld ? Vector3.Backward : tx.Rotation.Backward,
			StackDirection.Left => AlignWithWorld ? Vector3.Left : tx.Rotation.Left,
			StackDirection.Right => AlignWithWorld ? Vector3.Right : tx.Rotation.Right,
			_ => Vector3.Up
		};

		var size = Direction switch
		{
			StackDirection.Up or StackDirection.Down => bounds.Size.z,
			StackDirection.Forward or StackDirection.Backward => bounds.Size.x,
			StackDirection.Left or StackDirection.Right => bounds.Size.y,
			_ => bounds.Size.z
		};

		var offset = size + Spacing;
		return axis.Normal * offset;
	}

	void CreateWeld( GameObject a, GameObject b )
	{
		if ( !a.IsValid() || !b.IsValid() || a == b )
			return;

		var go1 = new GameObject( false, "weld" );
		go1.Parent = a;
		go1.LocalTransform = new Transform();
		go1.LocalRotation = Rotation.Identity;

		var go2 = new GameObject( false, "weld" );
		go2.Parent = b;
		go2.LocalTransform = new Transform();
		go2.LocalRotation = Rotation.Identity;

		var cleanup = go1.AddComponent<ConstraintCleanup>();
		cleanup.Attachment = go2;

		var joint = go1.AddComponent<FixedJoint>();
		joint.Attachment = Joint.AttachmentMode.Auto;
		joint.Body = go2;
		joint.EnableCollision = true;
		joint.AngularFrequency = 0;
		joint.LinearFrequency = 0;

		go2.NetworkSpawn();
		go1.NetworkSpawn();
	}

	bool IsValidTarget( SelectionPoint source )
	{
		if ( !source.IsValid() ) return false;
		if ( source.IsWorld ) return false;
		if ( source.IsPlayer ) return false;

		return true;
	}

	bool IsValidPlacementTarget( SelectionPoint source )
	{
		return source.IsValid();
	}

	[Rpc.Host]
	public async void DuplicateStack( Transform dest, GameObject anchor = null, Transform selectionAngle = default )
	{
		var player = Player.FindForConnection( Rpc.Caller );
		if ( player is null )
			return;

		var activeSpawner = spawner;
		if ( activeSpawner is null && anchor.IsValid() )
		{
			var root = anchor.Network.RootGameObject ?? anchor;
			var builder = new LinkedGameObjectBuilder() { RejectPlayers = true };
			builder.AddConnected( root );
			builder.RemoveDeletedObjects();

			var dupeCenter = selectionAngle.Position.IsNearlyZero() && selectionAngle.Rotation == Rotation.Identity
				? new Transform( root.WorldTransform.Position, root.WorldTransform.Rotation )
				: selectionAngle;

			var tempDupe = DuplicationData.CreateFromObjects( builder.Objects, dupeCenter );
			var json = Json.Serialize( tempDupe );
			activeSpawner = new DuplicatorSpawner( tempDupe, json );
		}

		if ( activeSpawner is null || !activeSpawner.IsReady )
			return;

		var undo = player.Undo.Create();
		undo.Name = "Stacker";

		GameObject previousRoot = null;
		var step = GetStackStep( dest, activeSpawner.Bounds );

		for ( int i = 0; i < Count; i++ )
		{
			var tx = dest;
			tx.Position += step * i;
			var rotation = Rotation.From( new Angles( RotationStep.pitch * i, RotationStep.yaw * i, RotationStep.roll * i ) );
			tx.Rotation = dest.Rotation * rotation;

			var objects = await activeSpawner.Spawn( tx, player );
			foreach ( var go in objects )
			{
				undo.Add( go );
			}

			var currentRoot = objects.Select( o => o.Root ).FirstOrDefault();

			if ( AutoWeld && previousRoot != null && currentRoot.IsValid() )
			{
				CreateWeld( previousRoot, currentRoot );
			}

			if ( AutoWeld && WeldToWorld && i == 0 && anchor != null && anchor.IsValid() )
			{
				CreateWeld( anchor, currentRoot );
			}

			previousRoot = currentRoot;
		}

		player.PlayerData?.AddStat( "tool.stacker.spawn" );
	}
}
