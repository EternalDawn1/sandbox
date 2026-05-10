[Group("Memory")]
[Alias( "wire_memory_cell" )]
public class WireMemoryCell : WireComponent
{
	[Property, Sync]
	public float DefaultValue { get; set; } = 0f;

	protected override void RegisterPorts()
	{
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Set", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _stored;

	protected override void Process()
	{
		var set = GetInput( "Set" ).Value.AsBoolean();
		if ( set )
			_stored = GetInput( "Value" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromNumber( _stored );
	}
}

[Group("Memory")]
[Alias( "wire_memory_latch" )]
public class WireMemoryLatch : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "Set", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Reset", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _stored;

	protected override void Process()
	{
		var set = GetInput( "Set" ).Value.AsBoolean();
		var reset = GetInput( "Reset" ).Value.AsBoolean();
		if ( set && !reset )
			_stored = 1f;
		else if ( reset && !set )
			_stored = 0f;
		GetOutput( "Out" ).Value = WireValue.FromNumber( _stored );
	}
}

[Group("Memory")]
[Alias( "wire_memory_toggle" )]
public class WireMemoryToggle : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "Toggle", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Reset", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _stored;

	protected override void Process()
	{
		var toggle = GetInput( "Toggle" ).Value.AsBoolean();
		var reset = GetInput( "Reset" ).Value.AsBoolean();
		if ( toggle )
			_stored = _stored > 0.5f ? 0f : 1f;
		if ( reset )
			_stored = 0f;
		GetOutput( "Out" ).Value = WireValue.FromNumber( _stored );
	}
}

[Group("Memory")]
[Alias( "wire_memory_counter" )]
public class WireMemoryCounter : WireComponent
{
	[Property, Sync]
	public float Min { get; set; } = 0f;

	[Property, Sync]
	public float Max { get; set; } = 10f;

	[Property, Sync]
	public float Step { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddInput( "Up", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Down", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Reset", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _count;

	protected override void Process()
	{
		var up = GetInput( "Up" ).Value.AsBoolean();
		var down = GetInput( "Down" ).Value.AsBoolean();
		var reset = GetInput( "Reset" ).Value.AsBoolean();

		if ( reset )
			_count = Min;
		else if ( up && !down )
			_count = Math.Min( _count + Step, Max );
		else if ( down && !up )
			_count = Math.Max( _count - Step, Min );

		GetOutput( "Out" ).Value = WireValue.FromNumber( _count );
	}
}

[Group("Memory")]
[Alias( "wire_memory_register" )]
public class WireMemoryRegister : WireComponent
{
	[Property, Sync]
	public int BitCount { get; set; } = 8;

	protected override void RegisterPorts()
	{
		AddInput( "Data", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Write", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Read", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _stored;

	protected override void Process()
	{
		var write = GetInput( "Write" ).Value.AsBoolean();
		var read = GetInput( "Read" ).Value.AsBoolean();

		if ( write )
			_stored = GetInput( "Data" ).Value.AsNumber();
		if ( read )
			GetOutput( "Out" ).Value = WireValue.FromNumber( _stored );
	}
}
