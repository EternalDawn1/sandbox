namespace Sandbox;

/// <summary>
/// Put this on a <see cref="InspectorEditor"/> subclass to specify the type of component it handles for the inspector
/// </summary>
[AttributeUsage( AttributeTargets.Class )]
public class InspectorEditorAttribute : Attribute
{
	public Type Type { get; }

	public InspectorEditorAttribute( Type type = null )
	{
		Type = type;
	}
}
