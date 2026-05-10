[Group("Gates")]
[Alias( "wire_gate_and" )]
public class WireGateAnd : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( a && b ) ? 1f : 0f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_or" )]
public class WireGateOr : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( a || b ) ? 1f : 0f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_not" )]
public class WireGateNot : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a ? 0f : 1f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_nand" )]
public class WireGateNand : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( !( a && b ) ) ? 1f : 0f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_nor" )]
public class WireGateNor : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( !( a || b ) ) ? 1f : 0f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_xor" )]
public class WireGateXor : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( a != b ) ? 1f : 0f );
	}
}

[Group("Gates")]
[Alias( "wire_gate_xnor" )]
public class WireGateXnor : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsBoolean();
		var b = GetInput( "B" ).Value.AsBoolean();
		GetOutput( "Out" ).Value = WireValue.FromNumber( ( a == b ) ? 1f : 0f );
	}
}
