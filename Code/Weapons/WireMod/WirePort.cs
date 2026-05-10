using System;
using System.Collections.Generic;
using System.Linq;

[Flags]
public enum WireConnectionFlags
{
	None = 0,
	Active = 1,
	Dirty = 2
}

public struct WireConnection
{
	public WirePort Source;
	public WirePort Target;
	public WireConnectionFlags Flags;

	public bool IsValid => Source.IsValid() && Target.IsValid();
}

public class WirePort
{
	public string Name { get; set; }
	public WirePortType Type { get; set; }
	public WireComponent Owner { get; set; }
	public bool IsInput { get; set; }
	public List<WireConnection> Connections { get; } = new();

	public WireValue Value { get; set; } = new();

	public bool IsValid() => Owner != null && Owner.GameObject.IsValid();

	public void ConnectTo( WirePort target )
	{
		if ( !IsValid() || !target.IsValid() ) return;
		if ( IsInput == target.IsInput ) return;
		if ( Connections.Any( c => c.Target == target ) ) return;

		Connections.Add( new WireConnection { Source = this, Target = target, Flags = WireConnectionFlags.Active | WireConnectionFlags.Dirty } );
	}

	public void DisconnectFrom( WirePort target )
	{
		Connections.RemoveAll( c => c.Target == target );
	}

	public void DisconnectAll()
	{
		foreach ( var conn in Connections.ToList() )
		{
			if ( conn.Target.IsValid() )
			{
				conn.Target.DisconnectFrom( this );
			}
		}
		Connections.Clear();
	}

	public IEnumerable<WireValue> GetInputValues()
	{
		foreach ( var conn in Connections )
		{
			if ( conn.IsValid && ( conn.Flags & WireConnectionFlags.Active ) != 0 )
			{
				yield return conn.Source.Value;
			}
		}
	}
}
