using System.Collections.Generic;

[Alias( "wire_array_cell" )]
public class WireArrayCell : WireComponent
{
	[Property, Sync]
	public int Index { get; set; } = 0;

	[Property, Sync]
	public float DefaultValue { get; set; } = 0f;

	protected override void RegisterPorts()
	{
		AddInput( "Index", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Set", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
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

[Alias( "wire_array_table" )]
public class WireArrayTable : WireComponent
{
	[Property, Sync]
	public int Size { get; set; } = 32;

	[Property, Sync]
	public float DefaultValue { get; set; } = 0f;

	float[] _data;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		_data = new float[Size];
		for ( int i = 0; i < Size; i++ )
			_data[i] = DefaultValue;
	}

	protected override void RegisterPorts()
	{
		AddInput( "Index", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Set", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Value", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Clear", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var clear = GetInput( "Clear" ).Value.AsBoolean();
		if ( clear )
		{
			for ( int i = 0; i < Size; i++ )
				_data[i] = DefaultValue;
			return;
		}

		var index = (int)GetInput( "Index" ).Value.AsNumber();
		if ( index < 0 || index >= Size )
		{
			GetOutput( "Out" ).Value = WireValue.FromNumber( 0f );
			return;
		}

		var set = GetInput( "Set" ).Value.AsBoolean();
		if ( set )
			_data[index] = GetInput( "Value" ).Value.AsNumber();

		GetOutput( "Out" ).Value = WireValue.FromNumber( _data[index] );
	}
}

[Alias( "wire_array_sort" )]
public class WireArraySort : WireComponent
{
	[Property, Sync]
	public int Size { get; set; } = 16;

	protected override void RegisterPorts()
	{
		for ( int i = 0; i < 16; i++ )
			AddInput( $"In{i}", WirePortType.Number, WireValue.FromNumber( 0 ) );
		for ( int i = 0; i < 16; i++ )
			AddOutput( $"Out{i}", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var values = new List<float>();
		for ( int i = 0; i < 16; i++ )
		{
			var port = GetInput( $"In{i}" );
			if ( port != null )
				values.Add( port.Value.AsNumber() );
		}
		values.Sort();
		for ( int i = 0; i < 16; i++ )
		{
			var port = GetOutput( $"Out{i}" );
			if ( port != null )
				port.Value = WireValue.FromNumber( i < values.Count ? values[i] : 0f );
		}
	}
}
