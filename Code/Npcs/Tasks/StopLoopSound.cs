using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task to stop any currently playing loop sound
/// </summary>
public class StopLoopSound : TaskBase
{
	protected override void OnStart()
	{
		var soundLayer = Layer<SoundLayer>();
		soundLayer?.StopLoopSound();
	}

	protected override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
