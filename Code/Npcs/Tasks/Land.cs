using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task that lands the NPC smoothly by steering towards the ground
/// </summary>
public class Land : TaskBase
{
	private FlightLayer _flight;
	private WanderLayer _wander;
	private Vector3 _landingTarget;

	protected override void OnStart()
	{
		_flight = Layer<FlightLayer>();
		_wander = Layer<WanderLayer>();

		if ( !Npc.IsValid() ) return;

		// Find ground position below current location
		var groundPos = Npc.Scene.NavMesh.GetClosestPoint( Npc.WorldPosition );
		_landingTarget = groundPos ?? Npc.WorldPosition.WithZ( Npc.WorldPosition.z - 100f );

		// Start flying towards the landing spot
		_flight?.FlyTowards( _landingTarget, 15f );
	}

	protected override TaskStatus OnUpdate()
	{
		if ( _flight is null || !Npc.IsValid() )
			return TaskStatus.Failed;

		// Check if we've reached the landing spot
		if ( _flight.HasReachedTarget() )
		{
			// Complete landing
			_flight.StopFlying();

			if ( _wander is not null )
			{
				_wander.SetHome( _landingTarget );
			}

			return TaskStatus.Success;
		}

		return TaskStatus.Running;
	}

	protected override void OnEnd()
	{
		_flight?.StopFlying();

		if ( _wander is not null && Npc.IsValid() )
		{
			_wander.SetHome( Npc.WorldPosition );
		}
	}
}
