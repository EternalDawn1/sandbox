[Icon( "🔊" )]
[Title( "Sound" )]
[ClassName( "sound" )]
[Group( "Building" )]
[Description( "Place sound emitters. Click to spawn a sound source at the target location." )]
public class SoundTool : ToolMode
{
	[Property, ClientEditable, Metadata( SoundDefinition.Thruster )]
	public SoundDefinition Sound { get; set; }

	[Property, Sync, Range( 0, 5 )]
	public float Volume { get; set; } = 1f;

	[Property, Sync, Range( 0.1f, 4f )]
	public float Pitch { get; set; } = 1f;

	[Property, Sync, Range( 0, 1000 )]
	public float Radius { get; set; } = 500f;

	[Property, Sync]
	public bool Loop { get; set; } = false;

	public override string Description => "Click to place a sound emitter";
	public override string PrimaryAction => "Place Sound";
	public override string ReloadAction => "Remove Sound";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			SpawnSound( select );
			ShootEffects( select );
		}

		if ( Input.Pressed( "reload" ) )
		{
			RemoveSound( select.GameObject );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void SpawnSound( SelectionPoint select )
	{
		if ( !select.IsValid() ) return;
		if ( Sound is null ) return;

		var go = new GameObject( false, "sound_emitter" );
		go.Tags.Add( "removable" );
		go.WorldTransform = select.WorldTransform();

		var emitter = go.AddComponent<ToolSoundEmitter>();
		emitter.Sound = Sound;
		emitter.Volume = Volume;
		emitter.Pitch = Pitch;
		emitter.Radius = Radius;
		emitter.Loop = Loop;

		go.NetworkSpawn();

		var undo = Player.Undo.Create();
		undo.Name = "Sound";
		undo.Icon = "🔊";
		undo.Add( go );
	}

	[Rpc.Host]
	public void RemoveSound( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		if ( !root.Tags.Contains( "removable" ) ) return;

		root.Destroy();
	}
}

public class ToolSoundEmitter : Component
{
	[Property, ClientEditable, Metadata( SoundDefinition.Thruster )]
	public SoundDefinition Sound { get; set; }

	[Property, Sync, Range( 0, 5 )]
	public float Volume { get; set; } = 1f;

	[Property, Sync, Range( 0.1f, 4f )]
	public float Pitch { get; set; } = 1f;

	[Property, Sync, Range( 0, 1000 )]
	public float Radius { get; set; } = 500f;

	[Property, Sync]
	public bool Loop { get; set; } = false;

	SoundHandle _handle;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		if ( Loop && Sound != null )
		{
			_handle = Sound.Play( WorldPosition, GameObject );
		}
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		if ( _handle.IsValid() )
		{
			_handle.Stop( 0.1f );
			_handle = default;
		}
	}

	protected override void OnUpdate()
	{
		if ( _handle.IsValid() )
		{
			_handle.Position = WorldPosition;
		}
	}
}
