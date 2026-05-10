[Icon( "⚙️" )]
[Title( "#tool.name.motor" )]
[ClassName( "motor" )]
[Group( "#tool.group.constraints" )]
public class MotorTool : BaseConstraintToolMode
{
	[Property, Sync]
	public float Torque { get; set; } = 100f;

	[Property, Sync]
	public float Speed { get; set; } = 10f;

	[Property, Sync]
	public bool FreeSpin { get; set; } = false;

	public override string Description => Stage == 1 ? "#tool.hint.motor.stage1" : "#tool.hint.motor.stage0";
	public override string PrimaryAction => Stage == 1 ? "#tool.hint.motor.finish" : "#tool.hint.motor.source";
	public override string ReloadAction => "#tool.hint.motor.remove";

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

		var go2 = new GameObject( point2.GameObject, false, "motor" );
		go2.LocalTransform = point2.LocalTransform;

		var go1 = new GameObject( point1.GameObject, false, "motor" );
		go1.WorldTransform = go2.WorldTransform;

		var joint = go1.AddComponent<HingeJoint>();
		joint.Body = go2;
		joint.Friction = FreeSpin ? 0f : 1f;
		joint.EnableCollision = false;

		var motor = go1.AddComponent<MotorComponent>();
		motor.Torque = Torque;
		motor.Speed = Speed;
		motor.FreeSpin = FreeSpin;

		go2.NetworkSpawn();
		go1.NetworkSpawn();

		Track( go1, go2 );

		var undo = Player.Undo.Create();
		undo.Name = "Motor";
		undo.Add( go1 );
		undo.Add( go2 );
	}
}

public class MotorComponent : Component
{
	[Property, Sync]
	public float Torque { get; set; } = 100f;

	[Property, Sync]
	public float Speed { get; set; } = 10f;

	[Property, Sync]
	public bool FreeSpin { get; set; } = false;

	[Property, Sync]
	public float Input { get; set; } = 0f;

	HingeJoint _joint;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		_joint = GetComponent<HingeJoint>();
	}

	protected override void OnUpdate()
	{
		if ( !_joint.IsValid() ) return;

		var rb = _joint.Body?.GetComponent<Rigidbody>();
		if ( !rb.IsValid() ) return;

		if ( MathF.Abs( Input ) > 0.01f )
		{
			var angularVel = WorldRotation.Up * Input * Speed;
			rb.AngularVelocity = angularVel;
		}
		else if ( !FreeSpin )
		{
			rb.AngularVelocity = Vector3.Zero;
		}
	}
}
