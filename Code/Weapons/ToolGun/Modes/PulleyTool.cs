[Icon( "🏗️" )]
[Title( "Pulley" )]
[ClassName( "pulley" )]
[Group( "Constraints" )]
[Description( "Create pulley constraints between two props. Click first prop, then second prop to create a pulley connection." )]
public class PulleyTool : BaseConstraintToolMode
{
	[Property, Sync]
	public bool EnableCollision { get; set; } = false;

	public override string Description => Stage == 1 ? "Click second prop to create pulley" : "Click first prop to set pulley origin";
	public override string PrimaryAction => Stage == 1 ? "Create Pulley" : "Set Origin";
	public override string ReloadAction => "Remove Pulleys";

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
