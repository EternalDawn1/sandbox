using System.Collections.Generic;
using System.Linq;

public abstract class WireComponent : Component
{
	[Sync]
	public Dictionary<string, WirePort> Inputs { get; set; } = new();

	[Sync]
	public Dictionary<string, WirePort> Outputs { get; set; } = new();

	[Sync]
	public bool Enabled_Wire { get; set; } = true;

	protected virtual float TickRate => 0f;

	private float _lastTick;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		RegisterPorts();
		WireSystem.Instance?.Register( this );
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		DisconnectAll();
		WireSystem.Instance?.Unregister( this );
	}

	protected virtual void RegisterPorts() { }

	protected void AddInput( string name, WirePortType type, WireValue defaultValue = default )
	{
		var port = new WirePort
		{
			Name = name,
			Type = type,
			Owner = this,
			IsInput = true,
			Value = defaultValue.Type == WirePortType.Any ? WireValue.FromNumber( 0 ) : defaultValue
		};
		Inputs[name] = port;
	}

	protected void AddOutput( string name, WirePortType type, WireValue defaultValue = default )
	{
		var port = new WirePort
		{
			Name = name,
			Type = type,
			Owner = this,
			IsInput = false,
			Value = defaultValue.Type == WirePortType.Any ? WireValue.FromNumber( 0 ) : defaultValue
		};
		Outputs[name] = port;
	}

	public void DisconnectAll()
	{
		foreach ( var port in Inputs.Values )
			port.DisconnectAll();
		foreach ( var port in Outputs.Values )
			port.DisconnectAll();
	}

	public WirePort GetInput( string name ) => Inputs.TryGetValue( name, out var port ) ? port : null;
	public WirePort GetOutput( string name ) => Outputs.TryGetValue( name, out var port ) ? port : null;

	public void SetInputValue( string name, WireValue value )
	{
		if ( Inputs.TryGetValue( name, out var port ) )
			port.Value = value;
	}

	public WireValue GetOutputValue( string name )
	{
		if ( Outputs.TryGetValue( name, out var port ) )
			return port.Value;
		return WireValue.Default;
	}

	internal void Tick()
	{
		if ( !Enabled_Wire ) return;
		if ( TickRate > 0 && Time.Now - _lastTick < TickRate ) return;
		_lastTick = Time.Now;

		ReadInputs();
		Process();
		WriteOutputs();
	}

	protected virtual void ReadInputs() { }
	protected virtual void Process() { }
	protected virtual void WriteOutputs() { }
}

public sealed class WireSystem : GameObjectSystem<WireSystem>
{
	private readonly List<WireComponent> _components = new();

	public static WireSystem Instance { get; private set; }

	public WireSystem( Scene scene ) : base( scene )
	{
		Instance = this;
		Listen( Stage.StartUpdate, 0, OnTick, "WireSystem" );
	}

	private void OnTick()
	{
		foreach ( var comp in _components.ToList() )
		{
			if ( comp.IsValid() && comp.Enabled_Wire )
				comp.Tick();
		}
	}

	public void Register( WireComponent component )
	{
		if ( !_components.Contains( component ) )
			_components.Add( component );
	}

	public void Unregister( WireComponent component )
	{
		_components.Remove( component );
	}

	public void Clear()
	{
		foreach ( var comp in _components.ToList() )
			comp.DisconnectAll();
		_components.Clear();
	}

	public IEnumerable<T> GetComponents<T>() where T : WireComponent
	{
		return _components.OfType<T>();
	}

	public IReadOnlyList<WireComponent> Components => _components;
}
