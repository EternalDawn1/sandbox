using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Schedules;

/// <summary>
/// Schedule to move a npc away from a threat while keeping eyes on the threat
/// </summary>
public sealed class FleeSchedule : ScheduleBase
{
	private GameObject _target;
	private float _distance;

	public FleeSchedule( GameObject target, float distance = 512f )
	{
		_target = target;
		_distance = distance;
	}

	public override async Task Execute()
	{
		if ( !_target.IsValid() )
			return;

		// Find a position away from the threat
		var escape = FindEscapePosition();
		if ( !escape.HasValue ) return;

		await ExecuteParallel( ExecutionMode.SucceedOnOne,
			new MoveTo( escape.Value ).CancelWhen( "threat-gone", "new-threat" ),
			new LookAt( _target ).CancelWhen( "threat-gone", "new-threat" )
		);
	}

	/// <summary>
	/// Find a suitable position to flee to
	/// </summary>
	private Vector3? FindEscapePosition()
	{
		var threatPos = _target.WorldPosition;
		var npcPos = Npc.WorldPosition;

		// Calculate direction away from threat
		var awayDirection = (npcPos - threatPos).Normal;

		// Try multiple positions at increasing distances
		for ( var distance = 100; distance <= _distance; distance += 50 )
		{
			// Try straight away
			var testPos = npcPos + awayDirection * distance;
			if ( IsValidEscapePosition( testPos ) )
				return testPos;

			// Try 45 degrees left and right
			for ( var angle = -45f; angle <= 45f; angle += 45f )
			{
				var rotatedDirection = awayDirection * Rotation.FromYaw( angle );
				testPos = npcPos + rotatedDirection * distance;

				if ( IsValidEscapePosition( testPos ) )
					return testPos;
			}
		}

		return null;
	}

	/// <summary>
	/// Check if a position is suitable for escaping
	/// </summary>
	private bool IsValidEscapePosition( Vector3 position )
	{
		// Is it reachable?
		var path = Scene.NavMesh.CalculatePath( new Navigation.CalculatePathRequest()
		{
			Start = Npc.WorldPosition,
			Target = position
		} );

		if ( !path.IsValid() || path.Points.Count < 1 )
			return false;

		return position.Distance( _target.WorldPosition ) >= _distance;
	}

	protected override async Task OnTaskCancelled( TaskBase task, string condition, bool wasConditionPresent )
	{
		if ( condition == "threat-gone" || condition == "new-threat" )
		{
			// the terms have changed, cancel -- re-evaluate if needed
			Cancel();
		}
	}
}
