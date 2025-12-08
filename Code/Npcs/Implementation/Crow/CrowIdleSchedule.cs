using Sandbox.Npcs.Layers;
using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow idle schedule - stands around, occasionally wanders
/// </summary>
public class CrowIdleSchedule : ScheduleBase
{
	protected override void OnStart()
	{
		// Start idle sound
		AddTask( new StartLoopSound( "sounds/bird/crow_ok.sound", 0.7f ) );

		var wanderLayer = Behavior.Layer<WanderLayer>();

		if ( wanderLayer.ShouldWander() )
		{
			// Time to wander around
			AddTask( new Wander() );
			AddTask( new Wait( Game.Random.Float( 2f, 5f ) ) );
		}
		else
		{
			// Just idle for a while
			AddTask( new Wait( Game.Random.Float( 1f, 3f ) ) );
		}
	}
}
