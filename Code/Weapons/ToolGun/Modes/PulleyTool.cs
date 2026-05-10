[Icon( "🏗️" )]
[Title( "#tool.name.pulley" )]
[ClassName( "pulley" )]
[Group( "#tool.group.constraints" )]
public class PulleyTool : BaseConstraintToolMode
{
	[Property, Sync]
	public bool EnableCollision { get; set; } = false;

	public override string Description => Stage == 1 ? "#tool.hint.pulley.stage1" : "#tool.hint.pulley.stage0";
	public override string PrimaryAction => Stage == 1 ? "#tool.hint.pulley.finish" : "#tool.hint.pulley.source";
	public override string ReloadAction => "#tool.hint.pulley.remove";

	protected override IEnumerable<GameObject> FindConstraints( GameObject linked, GameObject target )
	{
		foreach ( var joint in linked.GetComponentsInChildren<FixedJoint>( true ) )
			if ( linked == target || joint.Body?.Root == target )
				yield return joint.GameObject;
	}

	protected override void CreateConstraint( SelectionPoint point1, SelectionPoint point2 )
	{
		if ( point1.GameObject == point2.GameObject )
			return;

		var go1 = new GameObject( point1.GameObject, false, "pulley" );
		go1.LocalTransform = point1.LocalTransform;

		var go2 = new GameObject( point2.GameObject, false, "pulley" );
		go2.LocalTransform = point2.LocalTransform;

		var joint = go1.AddComponent<FixedJoint>();
		joint.Body = go2;
		joint.Attachment = Joint.AttachmentMode.Auto;
		joint.EnableCollision = EnableCollision;
		joint.AngularFrequency = 10;
		joint.LinearFrequency = 10;

		go2.NetworkSpawn();
		go1.NetworkSpawn();

		Track( go1, go2 );

		var undo = Player.Undo.Create();
		undo.Name = "Pulley";
		undo.Add( go1 );
		undo.Add( go2 );
	}
}
