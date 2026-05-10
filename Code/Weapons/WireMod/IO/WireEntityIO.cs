[Alias( "wire_entity_input" )]
public class WireEntityInput : WireComponent, IPlayerControllable
{
	[Property, Sync, ClientEditable]
	public ClientInput Trigger { get; set; }

	[Property, Sync]
	public string TargetName { get; set; } = "";

	[Property, Sync]
	public string InputName { get; set; } = "";

	protected override void RegisterPorts()
	{
		AddInput( "Fire", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
		var analog = Trigger.GetAnalog();
		if ( MathF.Abs( analog ) > 0.1f )
			SetInputValue( "Fire", WireValue.FromNumber( analog ) );
	}
}

[Alias( "wire_entity_output" )]
public class WireEntityOutput : WireComponent
{
	[Property, Sync]
	public string TargetName { get; set; } = "";

	[Property, Sync]
	public string OutputName { get; set; } = "Out";

	protected override void RegisterPorts()
	{
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = GetInput( "Value" ).Value;
	}
}

[Alias( "wire_entity_controller" )]
public class WireEntityController : WireComponent
{
	[Property, Sync]
	public float Throttle { get; set; } = 0f;

	[Property, Sync]
	public float Steering { get; set; } = 0f;

	protected override void RegisterPorts()
	{
		AddInput( "Throttle", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Steering", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Brake", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Handbrake", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		Throttle = GetInput( "Throttle" ).Value.AsNumber();
		Steering = GetInput( "Steering" ).Value.AsNumber();
	}
}
