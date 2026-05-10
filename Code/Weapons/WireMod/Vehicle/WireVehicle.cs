[Alias( "wire_vehicle_controller" )]
public class WireVehicleController : WireComponent, IPlayerControllable
{
	[Property, Sync]
	public float Throttle { get; set; } = 0f;

	[Property, Sync]
	public float Steering { get; set; } = 0f;

	[Property, Sync]
	public float Brake { get; set; } = 0f;

	[Property, Sync]
	public bool Handbrake { get; set; } = false;

	protected override void RegisterPorts()
	{
		AddInput( "Throttle", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Steering", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Brake", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Handbrake", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Speed", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		Throttle = GetInput( "Throttle" ).Value.AsNumber();
		Steering = GetInput( "Steering" ).Value.AsNumber();
		Brake = GetInput( "Brake" ).Value.AsNumber();
		Handbrake = GetInput( "Handbrake" ).Value.AsBoolean();

		var rb = GetComponent<Rigidbody>();
		if ( rb.IsValid() )
			GetOutput( "Speed" ).Value = WireValue.FromNumber( rb.Velocity.Length );
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
	}
}

[Alias( "wire_vehicle_seat" )]
public class WireVehicleSeat : WireComponent, IPlayerControllable
{
	[Property, Sync, ClientEditable]
	public ClientInput Use { get; set; }

	[Property, Sync]
	public float Throttle { get; set; } = 0f;

	[Property, Sync]
	public float Steering { get; set; } = 0f;

	protected override void RegisterPorts()
	{
		AddInput( "Throttle", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Steering", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Speed", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
		var analog = Use.GetAnalog();
		if ( MathF.Abs( analog ) > 0.1f )
			Throttle = analog;
	}
}
