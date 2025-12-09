using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Tells the LookAtLayer to face a target position or object
/// </summary>
public class LookAt : TaskBase
{
	public Vector3? TargetPosition { get; set; }
	public GameObject TargetObject { get; set; }

	private AnimationLayer _animation;

	public LookAt( Vector3 targetPosition )
	{
		TargetPosition = targetPosition;
	}

	public LookAt( GameObject gameObject )
	{
		TargetObject = gameObject;
	}

	protected override void OnStart()
	{
		_animation ??= GetLayer<AnimationLayer>();
	}

	protected override TaskStatus OnUpdate()
	{
		if ( _animation is null )
			return TaskStatus.Failed;

		var targetPos = GetTargetPosition();
		if ( !targetPos.HasValue )
			return TaskStatus.Failed;

		_animation.LookAt( targetPos.Value );

		return _animation.IsFacingTarget() ? TaskStatus.Success : TaskStatus.Running;
	}

	private Vector3? GetTargetPosition()
	{
		if ( TargetObject.IsValid() ) return TargetObject.WorldPosition;
		return TargetPosition;
	}
}
