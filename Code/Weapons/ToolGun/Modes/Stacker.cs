using Sandbox.UI;
using System.Linq;
using System.Text.Json;

[Icon( "vertical_align_top" )]
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
	LinkedGameObjectBuilder builder = new() { RejectPlayers = true };

	Rotation _rotationOffset = Rotation.Identity;
	Rotation _spinRotation = Rotation.Identity;
	Rotation _snapRotation = Rotation.Identity;
	bool _isSnapping;
	bool _isRotating;

	public override string Description => "Copy an object and stack duplicates with a live preview using Offset and RotationStep.";
	public override string PrimaryAction => spawner is not null ? "Spawn stack" : null;
	public override string SecondaryAction => "Copy object";
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

		_isRotating = spawner is not null && Input.Down( "use" );
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

		var select = TraceSelect();
		IsValidState = IsValidTarget( select );

		if ( spawner is { IsReady: true } && Input.Pressed( "attack1" ) )
		{
			if ( !IsValidPlacementTarget( select ) )
			{
				return;
			}

			var tx = GetPlacementTransform( select );
			DuplicateStack( tx, select.GameObject );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "attack2" ) )
		{
			if ( !IsValidState )
			{
				CopiedJson = default;
				return;
			}

			var selectionAngle = new Transform( select.WorldPosition(), Player.EyeTransform.Rotation.Angles().WithPitch( 0 ) );
			Copy( select.GameObject, selectionAngle, Input.Down( "run" ) );
			ShootEffects( select );
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
		if ( spawner is null || !spawner.IsReady )
			return;

		var select = TraceSelect();
		if ( !IsValidPlacementTarget( select ) )
			return;

		var tx = GetPlacementTransform( select );

		var overlayMaterial = IsProxy ? Material.Load( "materials/effects/duplicator_override_other.vmat" ) : Material.Load( "materials/effects/duplicator_override.vmat" );
		var step = GetStackStep( tx, spawner.Bounds );

		for ( int i = 0; i < Count; i++ )
		{
			var drawTx = tx;
			drawTx.Position += step * i;
			var rotation = Rotation.From( new Angles( RotationStep.pitch * i, RotationStep.yaw * i, RotationStep.roll * i ) );
			drawTx.Rotation = tx.Rotation * rotation;
			spawner.DrawPreview( drawTx, overlayMaterial );
		}
	}

	Transform GetPlacementTransform( SelectionPoint select )
	{
		var target = select.GameObject.Network.RootGameObject ?? select.GameObject;
		var targetBounds = target.GetBounds();
		var targetRotation = AlignWithWorld ? Rotation.Identity : target.WorldTransform.Rotation;

		var tx = new Transform();
		tx.Rotation = targetRotation * _rotationOffset;

		if ( Offset != Vector3.Zero )
		{
			tx.Position = targetBounds.Center + GetStackStep( tx, spawner.Bounds );
			return tx;
		}

		var axis = Direction switch
		{
			StackDirection.Up => AlignWithWorld ? Vector3.Up : targetRotation.Up,
			StackDirection.Down => AlignWithWorld ? Vector3.Down : targetRotation.Down,
			StackDirection.Forward => AlignWithWorld ? Vector3.Forward : targetRotation.Forward,
			StackDirection.Backward => AlignWithWorld ? Vector3.Backward : targetRotation.Backward,
			StackDirection.Left => AlignWithWorld ? Vector3.Left : targetRotation.Left,
			StackDirection.Right => AlignWithWorld ? Vector3.Right : targetRotation.Right,
			_ => Vector3.Up
		};

		var targetExtent = Direction switch
		{
			StackDirection.Up or StackDirection.Down => targetBounds.Extents.z,
			StackDirection.Forward or StackDirection.Backward => targetBounds.Extents.x,
			StackDirection.Left or StackDirection.Right => targetBounds.Extents.y,
			_ => targetBounds.Extents.z
		};

		var startPosition = targetBounds.Center + axis * targetExtent;
		var previewOffset = Direction switch
		{
			StackDirection.Up => -spawner.Bounds.Mins.z,
			StackDirection.Down => spawner.Bounds.Maxs.z,
			StackDirection.Forward => -spawner.Bounds.Mins.x,
			StackDirection.Backward => spawner.Bounds.Maxs.x,
			StackDirection.Left => -spawner.Bounds.Mins.y,
			StackDirection.Right => spawner.Bounds.Maxs.y,
			_ => -spawner.Bounds.Mins.z
		};

		tx.Position = startPosition + axis * previewOffset;

		return tx;
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
	public async void DuplicateStack( Transform dest, GameObject anchor = null )
	{
		if ( spawner is null )
			return;

		var player = Player.FindForConnection( Rpc.Caller );
		if ( player is null )
			return;

		var undo = player.Undo.Create();
		undo.Name = "Stacker";

		GameObject previousRoot = null;
		var step = GetStackStep( dest, spawner.Bounds );

		for ( int i = 0; i < Count; i++ )
		{
			var tx = dest;
			tx.Position += step * i;
			var rotation = Rotation.From( new Angles( RotationStep.pitch * i, RotationStep.yaw * i, RotationStep.roll * i ) );
			tx.Rotation = dest.Rotation * rotation;

			var objects = await spawner.Spawn( tx, player );
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
