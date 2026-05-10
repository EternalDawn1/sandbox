[Icon( "🔩" )]
[Title( "Axis" )]
[ClassName( "axis" )]
[Group( "Constraints" )]
[Description( "Create axis (hinge) constraints between two props. Click first prop, then second prop to create a hinge connection." )]
public class AxisTool : BaseConstraintToolMode
{
	[Property, Sync]
	public bool EnableCollision { get; set; } = false;

	[Property, Sync]
	public float Friction { get; set; } = 0f;

	public override string Description => Stage == 1 ? "Click second prop to create hinge" : "Click first prop to set hinge origin";
	public override string PrimaryAction => Stage == 1 ? "Create Hinge" : "Set Origin";
	public override string ReloadAction => "Remove Hinges";

	protected override IEnumerable<GameObject> FindConstraints( GameObject linked, GameObject target )
	{
		foreach ( var joint in linked.GetComponentsInChildren<HingeJoint>( true ) )
			if ( linked == target || joint.Body?.Root == target )
				yield return joint.GameObject;
	}

	protected override void CreateConstraint( SelectionPoint point1, SelectionPoint point2 )
	{
		if ( point1.GameObject == point2.GameObject )
			return;

		var go2 = new GameObject( point2.GameObject, false, "axis" );
		go2.LocalTransform = point2.LocalTransform;

		var go1 = new GameObject( point1.GameObject, false, "axis" );
		go1.WorldTransform = go2.WorldTransform;

		var joint = go1.AddComponent<HingeJoint>();
		joint.Body = go2;
		joint.Friction = Friction;
		joint.EnableCollision = EnableCollision;

		go2.NetworkSpawn();
		go1.NetworkSpawn();

		Track( go1, go2 );

		var undo = Player.Undo.Create();
		undo.Name = "Axis";
		undo.Add( go1 );
		undo.Add( go2 );
	}
}
