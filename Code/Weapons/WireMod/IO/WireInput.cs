[Alias( "wire_input_button" )]
public class WireInputButton : WireComponent, IPlayerControllable
{
	[Property, Sync, ClientEditable]
	public ClientInput Press { get; set; }

	[Property, Sync]
	public float OutputValue { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
		var analog = Press.GetAnalog();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Abs( analog ) > 0.1f ? OutputValue : 0f );
	}
}

[Alias( "wire_input_lever" )]
public class WireInputLever : WireComponent, IPlayerControllable
{
	[Property, Sync, ClientEditable]
	public ClientInput Move { get; set; }

	[Property, Sync]
	public float Min { get; set; } = 0f;

	[Property, Sync]
	public float Max { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
		var analog = Move.GetAnalog();
		var range = Max - Min;
		GetOutput( "Out" ).Value = WireValue.FromNumber( Min + MathF.Abs( analog ) * range );
	}
}

[Alias( "wire_input_keypad" )]
public class WireInputKeypad : WireComponent
{
	[Property, Sync]
	public float Value { get; set; } = 0f;

	[Property, Sync]
	public float MaxLength { get; set; } = 6f;

	protected override void RegisterPorts()
	{
		AddInput( "Clear", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Enter", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit0", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit1", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit2", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit3", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit4", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit5", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit6", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit7", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit8", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Digit9", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _buffer;

	protected override void Process()
	{
		var clear = GetInput( "Clear" ).Value.AsBoolean();
		if ( clear )
		{
			_buffer = 0f;
			Value = 0f;
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			return;
		}

		for ( int i = 0; i <= 9; i++ )
		{
			if ( GetInput( $"Digit{i}" ).Value.AsBoolean() )
			{
				if ( _buffer < MathF.Pow( 10, MaxLength ) )
					_buffer = _buffer * 10 + i;
			}
		}

		var enter = GetInput( "Enter" ).Value.AsBoolean();
		if ( enter )
		{
			Value = _buffer;
			GetOutput( "Out" ).Value = WireValue.FromNumber( Value );
		}
	}
}

[Alias( "wire_input_constant" )]
public class WireInputConstant : WireComponent
{
	[Property, Sync]
	public float Value { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 1 ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = WireValue.FromNumber( Value );
	}
}

[Alias( "wire_input_toggle" )]
public class WireInputToggleSwitch : WireComponent, IPlayerControllable
{
	[Property, Sync, ClientEditable]
	public ClientInput Toggle { get; set; }

	[Property, Sync]
	public bool State { get; set; } = false;

	protected override void RegisterPorts()
	{
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	bool _lastPressed;

	public void OnControl()
	{
		if ( !Networking.IsHost ) return;
		var pressed = Toggle.GetAnalog() > 0.5f;
		if ( pressed && !_lastPressed )
			State = !State;
		_lastPressed = pressed;
		GetOutput( "Out" ).Value = WireValue.FromNumber( State ? 1f : 0f );
	}
}
