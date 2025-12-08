using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task that transitions the NPC from ground to air using steering-based flight
/// </summary>
public class TakeOff : TaskBase
{
	public Vector3 FlightDirection { get; set; }
	public float TakeOffHeight { get; set; } = 100f;

	private FlightLayer _flight;
	private Vector3 _takeOffTarget;

	public TakeOff( Vector3 flightDirection, float takeOffHeight = 100f )
	{
		FlightDirection = flightDirection.Normal;
		TakeOffHeight = takeOffHeight;
	}

	protected override void OnStart()
	{
		_flight = Layer<FlightLayer>();

		if ( _flight is not null && Npc.IsValid() )
		{
			// Calculate takeoff target: away from threat and upward
			_takeOffTarget = Npc.WorldPosition + (FlightDirection * 100f) + (Vector3.Up * TakeOffHeight);
			_flight.FlyTowards( _takeOffTarget, 15f );
		}
	}

	protected override TaskStatus OnUpdate()
	{
		if ( _flight is null )
			return TaskStatus.Failed;

		return _flight.HasReachedTarget() ? TaskStatus.Success : TaskStatus.Running;
	}
}
