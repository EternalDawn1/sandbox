[Icon( "🪑" )]
[Title( "#tool.name.pod" )]
[ClassName( "pod" )]
[Group( "#tool.group.building" )]
public class PodTool : ToolMode
{
	[Property, Sync]
	public float ExitForce { get; set; } = 200f;

	public override string Description => "#tool.hint.pod.description";
	public override string PrimaryAction => "#tool.hint.pod.place";
	public override string ReloadAction => "#tool.hint.pod.remove";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			SpawnPod( select );
			ShootEffects( select );
		}

		if ( Input.Pressed( "reload" ) )
		{
			RemovePod( select.GameObject );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void SpawnPod( SelectionPoint select )
	{
		if ( !select.IsValid() ) return;

		var go = new GameObject( false, "pod" );
		go.Tags.Add( "removable" );
		go.WorldTransform = select.WorldTransform();

		var pod = go.AddComponent<WirePodEntity>();
		pod.ExitForce = ExitForce;

		go.NetworkSpawn();

		var undo = Player.Undo.Create();
		undo.Name = "Pod";
		undo.Icon = "🪑";
		undo.Add( go );
	}

	[Rpc.Host]
	public void RemovePod( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		if ( !root.Tags.Contains( "removable" ) ) return;

		root.Destroy();
	}
}

public class WirePodEntity : Component, IPlayerControllable
{
	[Property, Sync]
	public float ExitForce { get; set; } = 200f;

	[Property, Sync, ClientEditable]
	public ClientInput Use { get; set; }

	Player _occupant;

	public bool CanControl( Player player ) => _occupant == null || _occupant == player;

	public void OnStartControl()
	{
		_occupant = Scene.GetAll<Player>().FirstOrDefault();
	}

	public void OnEndControl()
	{
		if ( _occupant.IsValid() )
		{
			var rb = _occupant.GetComponent<Rigidbody>();
			if ( rb.IsValid() )
			{
				rb.ApplyImpulse( WorldRotation.Up * ExitForce );
			}
		}
		_occupant = null;
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;

		if ( _occupant.IsValid() && Use.Pressed() )
		{
			var rb = _occupant.GetComponent<Rigidbody>();
			if ( rb.IsValid() )
			{
				rb.ApplyImpulse( WorldRotation.Up * ExitForce );
			}
			_occupant = null;
		}
	}
}
