[Alias( "wire_sensor_range" )]
public class WireSensorRange : WireComponent
{
	[Property, Sync]
	public float Range { get; set; } = 500f;

	[Property, Sync]
	public bool TraceHitboxes { get; set; } = false;

	protected override void RegisterPorts()
	{
		AddOutput( "Distance", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Entity", WirePortType.Entity, WireValue.Default );
		AddOutput( "Hit", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var ray = new Ray( WorldPosition, WorldRotation.Forward );
		var trace = Scene.Trace.Ray( ray, Range );
		if ( TraceHitboxes )
			trace = trace.UseHitboxes();
		var tr = trace.Run();

		if ( tr.Hit )
		{
			GetOutput( "Distance" ).Value = WireValue.FromNumber( tr.Distance );
			GetOutput( "Entity" ).Value = WireValue.FromEntity( tr.GameObject );
			GetOutput( "Hit" ).Value = WireValue.FromNumber( 1f );
		}
		else
		{
			GetOutput( "Distance" ).Value = WireValue.FromNumber( Range );
			GetOutput( "Entity" ).Value = WireValue.FromEntity( null );
			GetOutput( "Hit" ).Value = WireValue.FromNumber( 0f );
		}
	}
}

[Alias( "wire_sensor_speed" )]
public class WireSensorSpeed : WireComponent
{
	protected override void RegisterPorts()
	{
		AddOutput( "Speed", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var rb = GetComponent<Rigidbody>();
		if ( rb.IsValid() )
			GetOutput( "Speed" ).Value = WireValue.FromNumber( rb.Velocity.Length );
		else
			GetOutput( "Speed" ).Value = WireValue.FromNumber( 0f );
	}
}

[Alias( "wire_sensor_angle" )]
public class WireSensorAngle : WireComponent
{
	protected override void RegisterPorts()
	{
		AddOutput( "Pitch", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Yaw", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Roll", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var angles = WorldRotation.Angles();
		GetOutput( "Pitch" ).Value = WireValue.FromNumber( angles.pitch );
		GetOutput( "Yaw" ).Value = WireValue.FromNumber( angles.yaw );
		GetOutput( "Roll" ).Value = WireValue.FromNumber( angles.roll );
	}
}

[Alias( "wire_sensor_position" )]
public class WireSensorPosition : WireComponent
{
	protected override void RegisterPorts()
	{
		AddOutput( "X", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Y", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Z", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var pos = WorldPosition;
		GetOutput( "X" ).Value = WireValue.FromNumber( pos.x );
		GetOutput( "Y" ).Value = WireValue.FromNumber( pos.y );
		GetOutput( "Z" ).Value = WireValue.FromNumber( pos.z );
	}
}

[Alias( "wire_sensor_target" )]
public class WireSensorTarget : WireComponent
{
	[Property, Sync]
	public float Range { get; set; } = 2000f;

	protected override void RegisterPorts()
	{
		AddOutput( "Entity", WirePortType.Entity, WireValue.Default );
		AddOutput( "Position", WirePortType.Vector, WireValue.FromVector( Vector3.Zero ) );
		AddOutput( "Found", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var ray = new Ray( WorldPosition, WorldRotation.Forward );
		var trace = Scene.Trace.Ray( ray, Range ).WithoutTags( "player" ).Run();

		if ( trace.Hit && !trace.GameObject.Tags.Contains( "world" ) )
		{
			GetOutput( "Entity" ).Value = WireValue.FromEntity( trace.GameObject );
			GetOutput( "Position" ).Value = WireValue.FromVector( trace.EndPosition );
			GetOutput( "Found" ).Value = WireValue.FromNumber( 1f );
		}
		else
		{
			GetOutput( "Entity" ).Value = WireValue.FromEntity( null );
			GetOutput( "Position" ).Value = WireValue.FromVector( Vector3.Zero );
			GetOutput( "Found" ).Value = WireValue.FromNumber( 0f );
		}
	}
}
