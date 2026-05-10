[Alias( "wire_output_lamp" )]
public class WireOutputLamp : WireComponent
{
	[Property]
	public GameObject LightOn { get; set; }

	[Property]
	public GameObject LightOff { get; set; }

	[Property, Sync]
	public Color OnColor { get; set; } = Color.Green;

	[Property, Sync]
	public Color OffColor { get; set; } = Color.Red;

	protected override void RegisterPorts()
	{
		AddInput( "On", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	bool _isOn;

	protected override void Process()
	{
		_isOn = GetInput( "On" ).Value.AsBoolean();
		LightOn?.Enabled = _isOn;
		LightOff?.Enabled = !_isOn;
	}
}

[Alias( "wire_output_sound" )]
public class WireOutputSound : WireComponent
{
	[Property, ClientEditable, Metadata( SoundDefinition.Thruster )]
	public SoundDefinition Sound { get; set; }

	[Property, Sync]
	public float Volume { get; set; } = 1f;

	[Property, Sync]
	public float Pitch { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddInput( "Play", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Volume", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Pitch", WirePortType.Number, WireValue.FromNumber( 1 ) );
	}

	SoundHandle _handle;
	bool _lastPlay;

	protected override void Process()
	{
		var play = GetInput( "Play" ).Value.AsBoolean();
		Volume = GetInput( "Volume" ).Value.AsNumber();
		Pitch = GetInput( "Pitch" ).Value.AsNumber();

		if ( play && !_lastPlay )
		{
			_handle = Sound?.Play( WorldPosition, GameObject );
		}
		else if ( !play && _lastPlay )
		{
			if ( _handle.IsValid() )
			{
				_handle.Stop( 0.1f );
				_handle = default;
			}
		}
		_lastPlay = play;
	}
}

[Alias( "wire_output_textscreen" )]
public class WireOutputTextScreen : WireComponent
{
	[Property, Sync]
	public string DisplayText { get; set; } = "";

	[Property, Sync]
	public Color TextColor { get; set; } = Color.White;

	protected override void RegisterPorts()
	{
		AddInput( "Text", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "Red", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Green", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Blue", WirePortType.Number, WireValue.FromNumber( 1 ) );
	}

	protected override void Process()
	{
		DisplayText = GetInput( "Text" ).Value.AsString();
		var r = GetInput( "Red" ).Value.AsNumber();
		var g = GetInput( "Green" ).Value.AsNumber();
		var b = GetInput( "Blue" ).Value.AsNumber();
		TextColor = new Color( r, g, b, 1f );
	}
}
