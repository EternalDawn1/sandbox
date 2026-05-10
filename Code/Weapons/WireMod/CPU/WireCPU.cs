[Group("CPU")]
[Alias( "wire_cpu" )]
public class WireCPU : WireComponent
{
	[Property, Sync]
	public int MemorySize { get; set; } = 256;

	[Property, Sync]
	public float Speed { get; set; } = 100f;

	protected override void RegisterPorts()
	{
		AddInput( "Enable", WirePortType.Number, WireValue.FromNumber( 1 ) );
		AddInput( "Reset", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "DataIn", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "DataOut", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Address", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float[] _memory;
	float _pc;
	float _acc;
	float _instruction;
	int _maxInstructionsPerFrame;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		_memory = new float[MemorySize];
		_maxInstructionsPerFrame = (int)Speed;
	}

	protected override void Process()
	{
		var enable = GetInput( "Enable" ).Value.AsBoolean();
		var reset = GetInput( "Reset" ).Value.AsBoolean();

		if ( reset )
		{
			_pc = 0;
			_acc = 0;
			System.Array.Clear( _memory, 0, _memory.Length );
			GetOutput( "DataOut" ).Value = WireValue.FromNumber( 0f );
			GetOutput( "Address" ).Value = WireValue.FromNumber( 0f );
			return;
		}

		if ( !enable )
			return;

		int count = 0;
		while ( count < _maxInstructionsPerFrame )
		{
			if ( _pc < 0 || _pc >= MemorySize )
				break;

			_instruction = _memory[(int)_pc];
			var opcode = (int)( _instruction / 1000 );
			var operand = (int)( _instruction % 1000 );

			ExecuteInstruction( opcode, operand );
			_pc++;
			count++;
		}

		GetOutput( "DataOut" ).Value = WireValue.FromNumber( _acc );
		GetOutput( "Address" ).Value = WireValue.FromNumber( _pc );
	}

	void ExecuteInstruction( int opcode, int operand )
	{
		switch ( opcode )
		{
			case 1:
				if ( operand >= 0 && operand < MemorySize )
					_acc = _memory[operand];
				break;
			case 2:
				if ( operand >= 0 && operand < MemorySize )
					_memory[operand] = _acc;
				break;
			case 3:
				if ( operand >= 0 && operand < MemorySize )
					_acc += _memory[operand];
				break;
			case 4:
				if ( operand >= 0 && operand < MemorySize )
					_acc -= _memory[operand];
				break;
			case 5:
				if ( operand >= 0 && operand < MemorySize )
					_acc *= _memory[operand];
				break;
			case 6:
				if ( operand >= 0 && operand < MemorySize )
					_acc = _memory[operand] != 0f ? _acc / _memory[operand] : 0f;
				break;
			case 7:
				_pc = operand - 1;
				break;
			case 8:
				if ( MathF.Abs( _acc ) < 0.001f )
					_pc = operand - 1;
				break;
			case 9:
				if ( MathF.Abs( _acc ) >= 0.001f )
					_pc = operand - 1;
				break;
			case 10:
				_acc = GetInput( "DataIn" ).Value.AsNumber();
				break;
			case 11:
				GetOutput( "DataOut" ).Value = WireValue.FromNumber( _acc );
				break;
			case 15:
				break;
		}
	}
}
