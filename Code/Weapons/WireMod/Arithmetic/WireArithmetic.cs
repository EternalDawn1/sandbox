[Group("Arithmetic")]
[Alias( "wire_arithmetic_add" )]
public class WireArithmeticAdd : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a + b );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_sub" )]
public class WireArithmeticSub : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a - b );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_mul" )]
public class WireArithmeticMul : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a * b );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_div" )]
public class WireArithmeticDiv : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( b != 0f ? a / b : 0f );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_mod" )]
public class WireArithmeticMod : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( b != 0f ? a % b : 0f );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_abs" )]
public class WireArithmeticAbs : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Abs( a ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_clamp" )]
public class WireArithmeticClamp : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Min", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Max", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var min = GetInput( "Min" ).Value.AsNumber();
		var max = GetInput( "Max" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( Math.Clamp( a, min, max ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_round" )]
public class WireArithmeticRound : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Decimals", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var decimals = (int)GetInput( "Decimals" ).Value.AsNumber();
		var factor = MathF.Pow( 10f, decimals );
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Round( a * factor ) / factor );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_sqrt" )]
public class WireArithmeticSqrt : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a >= 0f ? MathF.Sqrt( a ) : 0f );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_pow" )]
public class WireArithmeticPow : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Pow( a, b ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_sin" )]
public class WireArithmeticSin : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Sin( a ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_cos" )]
public class WireArithmeticCos : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Cos( a ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_tan" )]
public class WireArithmeticTan : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Tan( a ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_min" )]
public class WireArithmeticMin : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Min( a, b ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_max" )]
public class WireArithmeticMax : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "B", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		var b = GetInput( "B" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Max( a, b ) );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_negate" )]
public class WireArithmeticNegate : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( -a );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_increment" )]
public class WireArithmeticIncrement : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a + 1f );
	}
}

[Group("Arithmetic")]
[Alias( "wire_arithmetic_decrement" )]
public class WireArithmeticDecrement : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a - 1f );
	}
}
