[Alias( "wire_converter_num2str" )]
public class WireConverterNumberToString : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.String, WireValue.FromString( "" ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = WireValue.FromString( GetInput( "A" ).Value.AsString() );
	}
}

[Alias( "wire_converter_str2num" )]
public class WireConverterStringToNumber : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.String, WireValue.FromString( "" ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = WireValue.FromNumber( GetInput( "A" ).Value.AsNumber() );
	}
}

[Alias( "wire_converter_vec2num" )]
public class WireConverterVectorToNumber : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Vector, WireValue.FromVector( Vector3.Zero ) );
		AddOutput( "X", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Y", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Z", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var v = GetInput( "A" ).Value.AsVector();
		GetOutput( "X" ).Value = WireValue.FromNumber( v.x );
		GetOutput( "Y" ).Value = WireValue.FromNumber( v.y );
		GetOutput( "Z" ).Value = WireValue.FromNumber( v.z );
	}
}

[Alias( "wire_converter_num2vec" )]
public class WireConverterNumberToVector : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "X", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Y", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddInput( "Z", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Vector, WireValue.FromVector( Vector3.Zero ) );
	}

	protected override void Process()
	{
		var x = GetInput( "X" ).Value.AsNumber();
		var y = GetInput( "Y" ).Value.AsNumber();
		var z = GetInput( "Z" ).Value.AsNumber();
		GetOutput( "Out" ).Value = WireValue.FromVector( new Vector3( x, y, z ) );
	}
}

[Alias( "wire_converter_bool2num" )]
public class WireConverterBooleanToNumber : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Boolean, WireValue.FromBoolean( false ) );
		AddOutput( "Out", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = WireValue.FromNumber( GetInput( "A" ).Value.AsBoolean() ? 1f : 0f );
	}
}

[Alias( "wire_converter_num2bool" )]
public class WireConverterNumberToBoolean : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Out", WirePortType.Boolean, WireValue.FromBoolean( false ) );
	}

	protected override void Process()
	{
		GetOutput( "Out" ).Value = WireValue.FromBoolean( GetInput( "A" ).Value.AsBoolean() );
	}
}

[Alias( "wire_converter_angle2num" )]
public class WireConverterAngleToNumber : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "A", WirePortType.Angle, WireValue.FromAngle( Rotation.Identity ) );
		AddOutput( "Pitch", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Yaw", WirePortType.Number, WireValue.FromNumber( 0 ) );
		AddOutput( "Roll", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var angles = GetInput( "A" ).Value.AsAngle().Angles();
		GetOutput( "Pitch" ).Value = WireValue.FromNumber( angles.pitch );
		GetOutput( "Yaw" ).Value = WireValue.FromNumber( angles.yaw );
		GetOutput( "Roll" ).Value = WireValue.FromNumber( angles.roll );
	}
}

[Alias( "wire_converter_ent2pos" )]
public class WireConverterEntityToPosition : WireComponent
{
	protected override void RegisterPorts()
	{
		AddInput( "Entity", WirePortType.Entity, WireValue.Default );
		AddOutput( "Position", WirePortType.Vector, WireValue.FromVector( Vector3.Zero ) );
		AddOutput( "Valid", WirePortType.Number, WireValue.FromNumber( 0 ) );
	}

	protected override void Process()
	{
		var entity = GetInput( "Entity" ).Value.AsEntity();
		if ( entity.IsValid() )
		{
			GetOutput( "Position" ).Value = WireValue.FromVector( entity.WorldPosition );
			GetOutput( "Valid" ).Value = WireValue.FromNumber( 1f );
		}
		else
		{
			GetOutput( "Position" ).Value = WireValue.FromVector( Vector3.Zero );
			GetOutput( "Valid" ).Value = WireValue.FromNumber( 0f );
		}
	}
}
