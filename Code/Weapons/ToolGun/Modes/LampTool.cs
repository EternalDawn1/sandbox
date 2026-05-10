[Icon( "💡" )]
[Title( "Lamp" )]
[ClassName( "lamp" )]
[Group( "Building" )]
[Description( "Place lights and lamps. Click to spawn a light source at the target location." )]
public class LampTool : ToolMode
{
	[Property, Sync]
	public Color LightColor { get; set; } = Color.White;

	[Property, Sync, Range( 0, 50 )]
	public float Brightness { get; set; } = 2f;

	[Property, Sync, Range( 0, 1000 )]
	public float Radius { get; set; } = 500f;

	[Property, Sync, Range( 0, 90 )]
	public float Angle { get; set; } = 35f;

	[Property, Sync]
	public bool Shadows { get; set; } = true;

	[Property, Sync]
	public bool SpotLight { get; set; } = false;

	public override string Description => "Click to place a light";
	public override string PrimaryAction => "Place Light";
	public override string ReloadAction => "Remove Light";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			SpawnLamp( select );
			ShootEffects( select );
		}

		if ( Input.Pressed( "reload" ) )
		{
			RemoveLamp( select.GameObject );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void SpawnLamp( SelectionPoint select )
	{
		if ( !select.IsValid() ) return;

		var go = new GameObject( false, "lamp" );
		go.Tags.Add( "removable" );
		go.WorldTransform = select.WorldTransform();

		var color = LightColor;
		color.r *= Brightness;
		color.g *= Brightness;
		color.b *= Brightness;

		if ( SpotLight )
		{
			var light = go.AddComponent<SpotLight>();
			light.LightColor = color;
			light.Radius = Radius;
			light.ConeOuter = Angle;
			light.ConeInner = Angle * 0.5f;
			light.Shadows = Shadows;
		}
		else
		{
			var light = go.AddComponent<PointLight>();
			light.LightColor = color;
			light.Radius = Radius;
			light.Shadows = Shadows;
		}

		go.NetworkSpawn();

		var undo = Player.Undo.Create();
		undo.Name = "Lamp";
		undo.Icon = "💡";
		undo.Add( go );
	}

	[Rpc.Host]
	public void RemoveLamp( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		if ( !root.Tags.Contains( "removable" ) ) return;

		root.Destroy();
	}
}
