namespace Sandbox.Npcs.Layers;

/// <summary>
/// Handles random wandering behavior within a defined radius
/// </summary>
public class WanderLayer : BehaviorLayer
{
	[Property] public float WanderRadius { get; set; } = 64f;
	[Property] public float WanderInterval { get; set; } = 5f;
	
	public Vector3 HomePosition { get; private set; }
	private TimeSince _lastWander;

	protected override void OnUpdate()
	{
		// Set home position on first update if not set
		if ( HomePosition == Vector3.Zero && Npc.IsValid() )
		{
			HomePosition = Npc.WorldPosition;
		}
	}

	/// <summary>
	/// Set a new home position for wandering
	/// </summary>
	public void SetHome( Vector3 position )
	{
		HomePosition = position;
	}

	/// <summary>
	/// Get a random wander position within radius
	/// </summary>
	public Vector3 GetWanderPosition()
	{
		if ( !Npc.IsValid() ) return HomePosition;

		var attempts = 0;
		const int maxAttempts = 10;

		while ( attempts < maxAttempts )
		{
			// Generate random position within radius
			var randomOffset = Vector3.Random * WanderRadius;
			randomOffset = randomOffset.WithZ( 0 ); // Keep on ground plane
			var targetPos = HomePosition + randomOffset;

			// Check if position is reachable via NavMesh
			if ( IsPositionReachable( targetPos ) )
			{
				return targetPos;
			}

			attempts++;
		}

		// Fallback to current position if no valid position found
		return Npc.WorldPosition;
	}

	/// <summary>
	/// Check if a position is reachable via NavMesh
	/// </summary>
	private bool IsPositionReachable( Vector3 position )
	{
		if ( !Npc.Agent.IsValid() ) return false;

		// Use NavMesh sampling to check if position is valid
		var navMeshPoint = Behavior.Scene.NavMesh.GetClosestPoint( position );
		return navMeshPoint.HasValue && navMeshPoint.Value.Distance( position ) < 32f;
	}

	/// <summary>
	/// Should we wander now?
	/// </summary>
	public bool ShouldWander()
	{
		return _lastWander > WanderInterval;
	}

	/// <summary>
	/// Mark that we just wandered
	/// </summary>
	public void MarkWandered()
	{
		_lastWander = 0;
	}

	public override void Reset()
	{
		_lastWander = 0;
	}
}
