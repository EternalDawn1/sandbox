using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow behavior - idles and wanders, flees when threatened
/// </summary>
[Icon( "flutter_dash" ), Group( "NPCs" )]
public class CrowBehavior : Behavior
{
	[Property] public float ThreatDistance { get; set; } = 80f;

	private TimeSince _flightStartTime;
	private bool _hasCircled;

	protected override void OnStart()
	{
		var senses = AddLayer<SensesLayer>();
		senses.SightRange = 400f;
		senses.HearingRange = 300f;
		senses.PersonalSpace = ThreatDistance;
		senses.TargetTags = ["player"];

		AddLayer<SoundLayer>();
		AddLayer<FlightLayer>();
		AddLayer<LocomotionLayer>();

		var wander = AddLayer<WanderLayer>();
		wander.WanderRadius = 64f;
		wander.WanderInterval = Game.Random.Float( 5f, 10f );
	}

	public override ScheduleBase Run()
	{
		var flight = Layer<FlightLayer>();
		var senses = Layer<SensesLayer>();

		// If there's an immediate threat and we're not flying, start takeoff
		if ( senses.DistanceToNearest < ThreatDistance && !flight.IsFlying )
		{
			_flightStartTime = 0;
			_hasCircled = false;
			return Schedule<CrowTakeOffSchedule>();
		}

		// If we're flying, decide based on flight phase timing
		if ( flight.IsFlying )
		{
			// Just started flying - takeoff phase (first ~1 second)
			if ( _flightStartTime < 1f )
			{
				return Schedule<CrowTakeOffSchedule>();
			}
			// Circle phase (after takeoff, for a limited time)
			else if ( !_hasCircled || _flightStartTime < 8f )
			{
				_hasCircled = true;
				return Schedule<CrowCircleSchedule>();
			}
			// Landing phase
			else
			{
				return Schedule<CrowLandingSchedule>();
			}
		}
		else
		{
			// Reset flight state when not flying
			_hasCircled = false;
		}

		// Default ground behavior
		return Schedule<CrowIdleSchedule>();
	}
}
