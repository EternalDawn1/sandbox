using System;
using System.Collections.Generic;
using System.Globalization;

public struct WireValue
{
	public WirePortType Type { get; set; }
	public object RawValue { get; set; }

	public static readonly WireValue Default = new() { Type = WirePortType.Number, RawValue = 0.0f };

	public float AsNumber()
	{
		if ( RawValue is float f ) return f;
		if ( RawValue is int i ) return i;
		if ( RawValue is double d ) return (float)d;
		if ( RawValue is bool b ) return b ? 1f : 0f;
		if ( RawValue is string s && float.TryParse( s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed ) ) return parsed;
		return 0f;
	}

	public string AsString()
	{
		if ( RawValue is null ) return "";
		if ( RawValue is string s ) return s;
		if ( RawValue is float f ) return f.ToString( "G6", CultureInfo.InvariantCulture );
		return RawValue.ToString();
	}

	public bool AsBoolean()
	{
		if ( RawValue is bool b ) return b;
		if ( RawValue is float f ) return MathF.Abs( f ) > 0.001f;
		if ( RawValue is int i ) return i != 0;
		if ( RawValue is string s ) return !string.IsNullOrEmpty( s );
		return RawValue != null;
	}

	public Vector3 AsVector()
	{
		if ( RawValue is Vector3 v ) return v;
		if ( RawValue is float f ) return new Vector3( f, f, f );
		return Vector3.Zero;
	}

	public Rotation AsAngle()
	{
		if ( RawValue is Rotation r ) return r;
		if ( RawValue is float f ) return Rotation.From( f, f, f );
		return Rotation.Identity;
	}

	public Color AsColor()
	{
		if ( RawValue is Color c ) return c;
		return Color.White;
	}

	public GameObject AsEntity()
	{
		if ( RawValue is GameObject go ) return go;
		return null;
	}

	public static WireValue FromNumber( float value ) => new() { Type = WirePortType.Number, RawValue = value };
	public static WireValue FromString( string value ) => new() { Type = WirePortType.String, RawValue = value ?? "" };
	public static WireValue FromBoolean( bool value ) => new() { Type = WirePortType.Boolean, RawValue = value };
	public static WireValue FromVector( Vector3 value ) => new() { Type = WirePortType.Vector, RawValue = value };
	public static WireValue FromEntity( GameObject value ) => new() { Type = WirePortType.Entity, RawValue = value };
	public static WireValue FromAngle( Rotation value ) => new() { Type = WirePortType.Angle, RawValue = value };
	public static WireValue FromColor( Color value ) => new() { Type = WirePortType.Color, RawValue = value };
}
