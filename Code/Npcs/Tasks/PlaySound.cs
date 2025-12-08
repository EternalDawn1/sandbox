using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Generic task to play sounds through the SoundLayer
/// </summary>
public class PlaySound : TaskBase
{
	public string SoundPath { get; set; }
	public float Volume { get; set; } = -1f; // -1 = use default volume
	public bool IsLoop { get; set; } = false;
	public bool StopCurrentLoop { get; set; } = false;

	/// <summary>
	/// Play a one-shot sound
	/// </summary>
	public PlaySound( string soundPath, float volume = -1f )
	{
		SoundPath = soundPath;
		Volume = volume;
		IsLoop = false;
	}

	/// <summary>
	/// Start or stop a looping sound
	/// </summary>
	public PlaySound( string soundPath, bool isLoop, float volume = -1f )
	{
		SoundPath = soundPath;
		Volume = volume;
		IsLoop = isLoop;
	}

	/// <summary>
	/// Stop current loop sound
	/// </summary>
	public static PlaySound StopLoop()
	{
		return new PlaySound( null ) { StopCurrentLoop = true };
	}

	protected override void OnStart()
	{
		var soundLayer = Layer<SoundLayer>();
		if ( soundLayer is null ) return;

		if ( StopCurrentLoop )
		{
			soundLayer.StopLoopSound();
		}
		else if ( !string.IsNullOrEmpty( SoundPath ) )
		{
			if ( IsLoop )
			{
				soundLayer.StartLoopSound( SoundPath, Volume );
			}
			else
			{
				soundLayer.PlaySound( SoundPath, Volume );
			}
		}
	}

	protected override TaskStatus OnUpdate()
	{
		// Sound tasks complete immediately
		return TaskStatus.Success;
	}
}
