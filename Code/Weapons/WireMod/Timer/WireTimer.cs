[Alias( "wire_timer_delay" )]
public class WireTimerDelay : WireComponent
{
	[Property, Sync]
	public float Delay { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddInput( "In", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _timer;
	bool _active;

	protected override void Process()
	{
		var input = GetInput( "In" ).Value.AsBoolean();

		if ( input && !_active )
		{
			_active = true;
			_timer = 0f;
		}

		if ( _active )
		{
			_timer += Time.Delta;
			if ( _timer >= Delay )
			{
				GetOutput( "Out" ).Value = WireValue.FromNumber( 1f );
				_active = false;
				_timer = 0f;
			}
			else
			{
				GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			}
		}
		else
		{
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
		}
	}
}

[Alias( "wire_timer_pulse" )]
public class WireTimerPulse : WireComponent
{
	[Property, Sync]
	public float Interval { get; set; } = 1f;

	[Property, Sync]
	public float Duration { get; set; } = 0.1f;

	protected override void RegisterPorts()
	{
		AddInput( "Enable", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _timer;

	protected override void Process()
	{
		var enable = GetInput( "Enable" ).Value.AsBoolean();
		if ( !enable )
		{
			_timer = 0f;
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			return;
		}

		_timer += Time.Delta;
		if ( _timer >= Interval )
			_timer -= Interval;

		GetOutput( "Out" ).Value = WireValue.FromNumber( _timer < Duration ? 1f : 0f );
	}
}

[Alias( "wire_timer_oscillator" )]
public class WireTimerOscillator : WireComponent
{
	[Property, Sync]
	public float Frequency { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddInput( "Enable", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _phase;

	protected override void Process()
	{
		var enable = GetInput( "Enable" ).Value.AsBoolean();
		if ( !enable )
		{
			_phase = 0f;
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			return;
		}

		_phase += Time.Delta * Frequency;
		GetOutput( "Out" ).Value = WireValue.FromNumber( MathF.Sin( _phase * MathF.Tau ) );
	}
}

[Alias( "wire_timer_timer" )]
public class WireTimerTimer : WireComponent
{
	[Property, Sync]
	public float Duration { get; set; } = 5f;

	protected override void RegisterPorts()
	{
		AddInput( "Start", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Reset", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Remaining", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	float _elapsed;
	bool _running;

	protected override void Process()
	{
		var start = GetInput( "Start" ).Value.AsBoolean();
		var reset = GetInput( "Reset" ).Value.AsBoolean();

		if ( reset )
		{
			_elapsed = 0f;
			_running = false;
		}

		if ( start && !_running )
			_running = true;

		if ( _running )
		{
			_elapsed += Time.Delta;
			if ( _elapsed >= Duration )
			{
				GetOutput( "Out" ).Value = WireValue.FromNumber( 1f );
				GetOutput( "Remaining" ).Value = WireValue.FromNumber( 0f );
				_running = false;
				_elapsed = 0f;
			}
			else
			{
				GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
				GetOutput( "Remaining" ).Value = WireValue.FromNumber( Duration - _elapsed );
			}
		}
		else
		{
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			GetOutput( "Remaining" ).Value = WireValue.FromNumber( Duration );
		}
	}
}

[Alias( "wire_timer_toggle" )]
public class WireTimerToggle : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "In", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	bool _lastInput;
	bool _state;

	protected override void Process()
	{
		var input = GetInput( "In" ).Value.AsBoolean();
		if ( input && !_lastInput )
			_state = !_state;
		_lastInput = input;
		GetOutput( "Out" ).Value = WireValue.FromNumber( _state ? 1f : 0f );
	}
}

[Alias( "wire_timer_edge" )]
public class WireTimerEdge : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "In", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Rising", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Falling", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	bool _lastInput;

	protected override void Process()
	{
		var input = GetInput( "In" ).Value.AsBoolean();
		GetOutput( "Rising" ).Value = WireValue.FromNumber( ( input && !_lastInput ) ? 1f : 0f );
		GetOutput( "Falling" ).Value = WireValue.FromNumber( ( !input && _lastInput ) ? 1f : 0f );
		_lastInput = input;
	}
}

[Alias( "wire_timer_random" )]
public class WireTimerRandom : WireComponent
{
	[Property, Sync]
	public float Min { get; set; } = 0f;

	[Property, Sync]
	public float Max { get; set; } = 1f;

	protected override void RegisterPorts()
	{
		AddInput( "Trigger", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	bool _lastTrigger;

	protected override void Process()
	{
		var trigger = GetInput( "Trigger" ).Value.AsBoolean();
		if ( trigger && !_lastTrigger )
		{
			var value = Min + ( Max - Min ) * (float)Game.Random.NextDouble();
			GetOutput( "Out" ).Value = WireValue.FromNumber( value );
		}
		_lastTrigger = trigger;
	}
}
