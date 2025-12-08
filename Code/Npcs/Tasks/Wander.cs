using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task that makes the NPC wander to a random nearby location
/// </summary>
public class Wander : TaskBase
{
	private WanderLayer _wanderLayer;
	private LocomotionLayer _locomotion;
	private Vector3 _wanderTarget;

	protected override void OnStart()
	{
		_wanderLayer = Layer<WanderLayer>();
		_locomotion = Layer<LocomotionLayer>();

		if ( _wanderLayer is not null && _locomotion is not null )
		{
			_wanderTarget = _wanderLayer.GetWanderPosition();
			_locomotion.MoveTo( _wanderTarget, 5f );
			_wanderLayer.MarkWandered();
		}
	}

	protected override TaskStatus OnUpdate()
	{
		if ( _locomotion is null || _wanderLayer is null )
			return TaskStatus.Failed;

		// Check if we've reached the wander target
		return _locomotion.HasReachedTarget() ? TaskStatus.Success : TaskStatus.Running;
	}
}
