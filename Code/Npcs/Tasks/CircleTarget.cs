using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task that makes the NPC circle around a target GameObject using steering-based flight
/// </summary>
public class CircleTarget : TaskBase
{
	public GameObject Target { get; set; }
	public float Radius { get; set; } = 150f;
	public float Height { get; set; } = 100f;
	public float Duration { get; set; } = 3f;

	private FlightLayer _flight;
	private TimeUntil _endTime;
	private TimeSince _circleStartTime;
	private float _currentAngle;
	private float _circleDirection; // 1 for clockwise, -1 for counter-clockwise
	private const float CircleSpeed = 60f; // degrees per second

	public CircleTarget( GameObject target, float radius = 150f, float height = 100f, float duration = 3f )
	{
		Target = target;
		Radius = radius;
		Height = height;
		Duration = duration;
	}

	protected override void OnStart()
	{
		_flight = Layer<FlightLayer>();
		_endTime = Duration;
		_circleStartTime = 0;

		if ( _flight is not null && Target.IsValid() && Npc.IsValid() )
		{
			// Start flying if not already
			if ( !_flight.IsFlying )
			{
				_flight.StartFlying();
			}

			// Calculate starting angle based on current position
			var centerPos = Target.WorldPosition;
			var currentPos = Npc.WorldPosition;
			var offset = (currentPos - centerPos).WithZ( 0 );

			_currentAngle = MathF.Atan2( offset.y, offset.x ) * 180f / MathF.PI;
			_circleDirection = Game.Random.Float( 0f, 1f ) > 0.5f ? 1f : -1f; // Random direction

			// Set initial target on the circle
			UpdateCircleTarget();
		}
	}

	protected override TaskStatus OnUpdate()
	{
		if ( _flight is null )
			return TaskStatus.Failed;

		// If target is no longer valid, fail the task
		if ( !Target.IsValid() )
			return TaskStatus.Failed;

		// Check if duration has elapsed
		if ( _endTime )
			return TaskStatus.Success;

		// Update circle position
		UpdateCircleTarget();

		return TaskStatus.Running;
	}

	/// <summary>
	/// Update the circle target position for smooth steering
	/// </summary>
	private void UpdateCircleTarget()
	{
		if ( !Target.IsValid() || !Npc.IsValid() )
			return;

		// Update angle based on time
		_currentAngle += CircleSpeed * _circleDirection * Time.Delta;

		// Keep angle in valid range
		if ( _currentAngle > 360f ) _currentAngle -= 360f;
		if ( _currentAngle < 0f ) _currentAngle += 360f;

		// Calculate position on circle
		var centerPosition = Target.WorldPosition;
		var angleRadians = _currentAngle * MathF.PI / 180f;
		var x = MathF.Cos( angleRadians ) * Radius;
		var y = MathF.Sin( angleRadians ) * Radius;

		var circlePosition = centerPosition + new Vector3( x, y, Height );

		// Update flight target to this position
		_flight.FlyTowards( circlePosition, 25f );
	}

	protected override void OnEnd()
	{
		// Clear the target when task ends
		_flight?.ClearTarget();
	}
}
