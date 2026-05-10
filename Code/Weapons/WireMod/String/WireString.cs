[Group("String")]
[Alias( "wire_string_concat" )]
public class WireStringConcat : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "B", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		var b = GetInput( "B" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromString( a + b );
	}
}

[Group("String")]
[Alias( "wire_string_length" )]
public class WireStringLength : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a.Length );
	}
}

[Group("String")]
[Alias( "wire_string_sub" )]
public class WireStringSub : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "Start", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Length", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		var start = (int)GetInput( "Start" ).Value.AsNumber();
		var length = (int)GetInput( "Length" ).Value.AsNumber();
		if ( start < 0 || start >= a.Length || length <= 0 )
		{
			GetOutput( "Out" ).Value = WireValue.FromString( "" );
			return;
		}
		length = Math.Min( length, a.Length - start );
		GetOutput( "Out" ).Value = WireValue.FromString( a.Substring( start, length ) );
	}
}

[Group("String")]
[Alias( "wire_string_find" )]
public class WireStringFind : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "B", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( -1 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		var b = GetInput( "B" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromNumber( a.IndexOf( b ) );
	}
}

[Group("String")]
[Alias( "wire_string_replace" )]
public class WireStringReplace : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "Find", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "Replace", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		var find = GetInput( "Find" ).Value.AsString();
		var replace = GetInput( "Replace" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromString( a.Replace( find, replace ) );
	}
}

[Group("String")]
[Alias( "wire_string_upper" )]
public class WireStringToUpper : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromString( a.ToUpper() );
	}
}

[Group("String")]
[Alias( "wire_string_lower" )]
public class WireStringToLower : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromString( a.ToLower() );
	}
}

[Group("String")]
[Alias( "wire_string_trim" )]
public class WireStringTrim : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromString( a.Trim() );
	}
}

[Group("String")]
[Alias( "wire_string_format" )]
public class WireStringFormat : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "Format", WirePortType.String, WireValue.FromString( "{0}" ) );
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		var format = GetInput( "Format" ).Value.AsString();
		var a = GetInput( "A" ).Value.AsNumber();
		try
		{
			GetOutput( "Out" ).Value = WireValue.FromString( string.Format( format, a ) );
		}
		catch
		{
			GetOutput( "Out" ).Value = WireValue.FromString( format );
		}
	}
}

[Group("String")]
[Alias( "wire_string_compare" )]
public class WireStringCompare : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddInput( "B", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var a = GetInput( "A" ).Value.AsString();
		var b = GetInput( "B" ).Value.AsString();
		GetOutput( "Out" ).Value = WireValue.FromNumber( string.Equals( a, b ) ? 1f : 0f );
	}
}
