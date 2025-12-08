namespace Sandbox.Npcs.Layers;

/// <summary>
/// Generic sound layer for NPCs - handles sound playback and management
/// </summary>
public class SoundLayer : BehaviorLayer
{
	[Property] public float DefaultVolume { get; set; } = 1.0f;

	private SoundHandle _currentLoopSound;
	private string _currentLoopSoundPath;

	protected override void OnUpdate()
	{
		// Update loop sound position if playing
		if ( _currentLoopSound.IsValid() && Npc.IsValid() )
		{
			_currentLoopSound.Position = Npc.WorldPosition;
		}
	}

	/// <summary>
	/// Play a one-shot sound at the NPC's position
	/// </summary>
	public SoundHandle PlaySound( string soundPath, float volume = -1f )
	{
		if ( string.IsNullOrEmpty( soundPath ) || !Npc.IsValid() )
			return default;

		var soundEvent = ResourceLibrary.Get<SoundEvent>( soundPath );
		if ( !soundEvent.IsValid() )
			return default;

		var sound = Sound.Play( soundEvent, Npc.WorldPosition );
		if ( sound.IsValid() )
		{
			sound.Volume = volume >= 0f ? volume : DefaultVolume;
		}

		return sound;
	}

	/// <summary>
	/// Start playing a looping sound (stops any current loop)
	/// </summary>
	public SoundHandle StartLoopSound( string soundPath, float volume = -1f )
	{
		if ( string.IsNullOrEmpty( soundPath ) || !Npc.IsValid() )
			return default;

		// Stop current loop if different sound
		if ( _currentLoopSoundPath != soundPath )
		{
			StopLoopSound();
		}

		// Don't restart if already playing the same sound
		if ( _currentLoopSound.IsValid() && _currentLoopSoundPath == soundPath )
			return _currentLoopSound;

		var soundEvent = ResourceLibrary.Get<SoundEvent>( soundPath );
		if ( !soundEvent.IsValid() )
			return default;

		_currentLoopSound = Sound.Play( soundEvent, Npc.WorldPosition );
		if ( _currentLoopSound.IsValid() )
		{
			_currentLoopSound.Volume = volume >= 0f ? volume : DefaultVolume;
			_currentLoopSoundPath = soundPath;
		}

		return _currentLoopSound;
	}

	/// <summary>
	/// Stop the current looping sound
	/// </summary>
	public void StopLoopSound()
	{
		if ( _currentLoopSound.IsValid() )
		{
			_currentLoopSound.Stop();
		}

		_currentLoopSound = default;
		_currentLoopSoundPath = null;
	}

	/// <summary>
	/// Check if a specific loop sound is currently playing
	/// </summary>
	public bool IsLoopPlaying( string soundPath = null )
	{
		if ( string.IsNullOrEmpty( soundPath ) )
			return _currentLoopSound.IsValid();

		return _currentLoopSound.IsValid() && _currentLoopSoundPath == soundPath;
	}

	/// <summary>
	/// Get the currently playing loop sound path
	/// </summary>
	public string GetCurrentLoopSound() => _currentLoopSoundPath;

	public override void Reset()
	{
		StopLoopSound();
	}
}
