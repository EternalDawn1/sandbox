[Alias( "wire_screen_text" )]
public class WireScreenText : WireComponent
{
	[Property, Sync]
	public string Text { get; set; } = "";

	[Property, Sync]
	public float FontSize { get; set; } = 24f;

	[Property, Sync]
	public Color TextColor { get; set; } = Color.White;

	protected override void RegisterPorts()
	{
		AddInput( "Text", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "FontSize", WirePortType.Number, WireValue.FromNumber( 24 ) );
		AddInput( "Red", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Green", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Blue", WirePortType.Number, WireValue.FromNumber( 1 ) );
	}

	protected override void Process()
	{
		Text = GetInput( "Text" ).Value.AsString();
		FontSize = GetInput( "FontSize" ).Value.AsNumber();
		var r = GetInput( "Red" ).Value.AsNumber();
		var g = GetInput( "Green" ).Value.AsNumber();
		var b = GetInput( "Blue" ).Value.AsNumber();
		TextColor = new Color( r, g, b, 1f );
	}
}

[Alias( "wire_screen_number" )]
public class WireScreenNumber : WireComponent
{
	[Property, Sync]
	public float Value { get; set; } = 0f;

	[Property, Sync]
	public int Decimals { get; set; } = 2;

	[Property, Sync]
	public float FontSize { get; set; } = 32f;

	[Property, Sync]
	public Color TextColor { get; set; } = Color.Green;

	protected override void RegisterPorts()
	{
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Decimals", WirePortType.Number, WireValue.FromNumber( 2 ) );
		AddInput( "FontSize", WirePortType.Number, WireValue.FromNumber( 32 ) );
	}

	protected override void Process()
	{
		Value = GetInput( "Value" ).Value.AsNumber();
		Decimals = (int)GetInput( "Decimals" ).Value.AsNumber();
		FontSize = GetInput( "FontSize" ).Value.AsNumber();
	}
}

[Alias( "wire_screen_graph" )]
public class WireScreenGraph : WireComponent
{
	[Property, Sync]
	public float Value { get; set; } = 0f;

	[Property, Sync]
	public float Min { get; set; } = -1f;

	[Property, Sync]
	public float Max { get; set; } = 1f;

	[Property, Sync]
	public Color LineColor { get; set; } = Color.Green;

	protected override void RegisterPorts()
	{
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Min", WirePortType.Number, WireValue.FromNumber( -1 ) );
		AddInput( "Max", WirePortType.Number, WireValue.FromNumber( 1 ) );
	}

	protected override void Process()
	{
		Value = GetInput( "Value" ).Value.AsNumber();
		Min = GetInput( "Min" ).Value.AsNumber();
		Max = GetInput( "Max" ).Value.AsNumber();
	}
}
