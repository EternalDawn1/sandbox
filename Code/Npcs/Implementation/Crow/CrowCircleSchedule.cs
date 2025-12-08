using Sandbox.Npcs.Layers;
using Sandbox.Npcs.Tasks;

namespace Sandbox.Npcs.Crow;

/// <summary>
/// Crow circling schedule - handles circling around threats
/// </summary>
public class CrowCircleSchedule : ScheduleBase
{
	protected override void OnStart()
	{
		var senses = Behavior.Layer<SensesLayer>();
		if ( !senses.Nearest.IsValid() )
		{
			// No threat found, but let the behavior decide what to do next
			return;
		}

		var threatTarget = senses.Nearest;
		var height = Game.Random.Float( 128f, 256f );

		// Circle around the threat for a limited time
		AddTask( new CircleTarget(
			threatTarget,
			radius: Game.Random.Float( 128, 256f ),
			height: height,
			duration: Game.Random.Float( 2, 6 )
		) );
	}
}
