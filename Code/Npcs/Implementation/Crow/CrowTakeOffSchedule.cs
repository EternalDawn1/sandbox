using Sandbox.Npcs.Layers;
using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow takeoff schedule - handles initial flight response to threats
/// </summary>
public class CrowTakeOffSchedule : ScheduleBase
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

		// Take off away from the threat
		AddTask( new TakeOff( fleeDirection, height ) );
	}
}
