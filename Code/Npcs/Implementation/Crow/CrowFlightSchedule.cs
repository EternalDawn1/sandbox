using Sandbox.Npcs.Layers;
using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow flight schedule - handles scared flight behavior with smooth landing
/// </summary>
public class CrowFlightSchedule : ScheduleBase
{
	protected override void OnStart()
	{
		// Play alert sound
		AddTask( new PlaySound( "sounds/bird/crow_mad.sound" ) );

		// Start flight loop sound
		AddTask( new StartLoopSound( "sounds/bird/bird_wings_loop.sound", 0.7f ) );

		var senses = Behavior.Layer<SensesLayer>();
		if ( !senses.Nearest.IsValid() ) return;

		var threatTarget = senses.Nearest;
		var threatPosition = threatTarget.WorldPosition;
		var npcPosition = Npc.WorldPosition;

		// Calculate flee direction (away from threat)
		var fleeDirection = (npcPosition - threatPosition).Normal;

		var height = Game.Random.Float( 128f, 256f );

		// 1. Take off away from the threat
		AddTask( new TakeOff( fleeDirection, height ) );

		// 2. Circle around the threat for a limited time
		AddTask( new CircleTarget(
			threatTarget,
			radius: Game.Random.Float( 128, 256f ),
			height: height,
			duration: Game.Random.Float( 1, 5 )
		) );

		// 3. Find a safe landing spot, this shouldn't really be its own task though
		AddTask( new FindLandingSpot( threatPosition, 512f, 2048f ) );

		// 4. Land smoothly at the new spot
		AddTask( new Land() );

		// Stop all sounds when done
		AddTask( new StopLoopSound() );
	}
}
