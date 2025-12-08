using Sandbox.Npcs.Layers;
using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow landing schedule - attempts to find a safe landing spot
/// </summary>
public class CrowLandingSchedule : ScheduleBase
{
	private FindLandingSpot _findLandingTask;

	protected override void OnStart()
	{
		var senses = Behavior.Layer<SensesLayer>();
		Vector3 avoidPosition = Vector3.Zero;

		// If there's still a threat nearby, avoid that area
		if ( senses.Nearest.IsValid() )
		{
			avoidPosition = senses.Nearest.WorldPosition;
		}
		else
		{
			avoidPosition = Npc.WorldPosition;
		}

		// Try to find a safe landing spot
		_findLandingTask = new FindLandingSpot( avoidPosition, 256f, 1024f );
		AddTask( _findLandingTask );

		// If we found a landing spot, land there
		AddTask( new Land() );

		// Stop flight sounds when landing
		AddTask( new StopLoopSound() );
	}
}
