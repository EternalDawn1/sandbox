using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task to start a looping sound that continues until stopped
/// </summary>
public class StartLoopSound : TaskBase
{
	public string SoundPath { get; set; }
	public float Volume { get; set; } = -1f;

	public StartLoopSound( string soundPath, float volume = -1f )
	{
		SoundPath = soundPath;
		Volume = volume;
	}

	protected override void OnStart()
	{
		var soundLayer = Layer<SoundLayer>();
		soundLayer?.StartLoopSound( SoundPath, Volume );
	}

	protected override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
