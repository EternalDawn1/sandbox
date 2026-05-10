[Group("Debug")]
[Alias( "wire_debugger" )]
public class WireDebugger : WireComponent
{
	[Property, Sync]
	public string DebugText { get; set; } = "";

	[Property, Sync]
	public float RefreshRate { get; set; } = 0.5f;

	protected override float TickRate => RefreshRate;

	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "C", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "D", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		var c = GetInput( "C" ).Value.AsNumber();
		var d = GetInput( "D" ).Value.AsNumber();
		DebugText = $"A:{a:F2} B:{b:F2} C:{c:F2} D:{d:F2}";
	}
}

[Group("Debug")]
[Alias( "wire_hud" )]
public class WireHUD : WireComponent
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
