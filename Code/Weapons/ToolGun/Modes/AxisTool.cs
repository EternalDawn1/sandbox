[Icon( "🔩" )]
[Title( "#tool.name.axis" )]
[ClassName( "axis" )]
[Group( "#tool.group.constraints" )]
public class AxisTool : BaseConstraintToolMode
{
	[Property, Sync]
	public bool EnableCollision { get; set; } = false;

	[Property, Sync]
	public float Friction { get; set; } = 0f;

	public override string Description => Stage == 1 ? "#tool.hint.axis.stage1" : "#tool.hint.axis.stage0";
	public override string PrimaryAction => Stage == 1 ? "#tool.hint.axis.finish" : "#tool.hint.axis.source";
	public override string ReloadAction => "#tool.hint.axis.remove";

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
