[Icon( "🔘" )]
[Title( "#tool.name.button" )]
[ClassName( "button" )]
[Group( "#tool.group.building" )]
public class ButtonTool : ToolMode
{
	[Property, Sync]
	public float ToggleTime { get; set; } = 0f;

	[Property, Sync]
	public bool ToggleMode { get; set; } = false;

	public override string Description => "#tool.hint.button.description";
	public override string PrimaryAction => "#tool.hint.button.place";
	public override string ReloadAction => "#tool.hint.button.remove";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			SpawnButton( select );
			ShootEffects( select );
		}

		if ( Input.Pressed( "reload" ) )
		{
			RemoveButton( select.GameObject );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void SpawnButton( SelectionPoint select )
	{
		if ( !select.IsValid() ) return;

		var go = new GameObject( false, "button" );
		go.Tags.Add( "removable" );
		go.WorldTransform = select.WorldTransform();

		var button = go.AddComponent<WireButtonEntity>();
		button.ToggleTime = ToggleTime;
		button.ToggleMode = ToggleMode;

		go.NetworkSpawn();

		var undo = Player.Undo.Create();
		undo.Name = "Button";
		undo.Icon = "🔘";
		undo.Add( go );
	}

	[Rpc.Host]
	public void RemoveButton( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		if ( !root.Tags.Contains( "removable" ) ) return;

		root.Destroy();
	}
}

public class WireButtonEntity : Component, IPlayerControllable
{
	[Property, Sync]
	public float ToggleTime { get; set; } = 0f;

	[Property, Sync]
	public bool ToggleMode { get; set; } = false;

	[Property, Sync]
	public bool IsPressed { get; set; } = false;

	[Property, Sync]
	public float PressedTime { get; set; } = 0f;

	[Property, Sync, ClientEditable]
	public ClientInput Press { get; set; }

	bool _wasPressed;

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;

		var pressed = Press.GetAnalog() > 0.5f;

		if ( ToggleMode )
		{
			if ( pressed && !_wasPressed )
				IsPressed = !IsPressed;
		}
		else
		{
			IsPressed = pressed;
		}

		_wasPressed = pressed;
	}
}
