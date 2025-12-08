namespace Sandbox.Npcs.Layers;

/// <summary>
/// Handles aerial movement for flying NPCs using steering-based flight
/// </summary>
public class FlightLayer : BehaviorLayer
{
	[Property] public float FlightSpeed { get; set; } = 300f;
	[Property] public float TurnSpeed { get; set; } = 90f; // degrees per second
	[Property] public float PitchSpeed { get; set; } = 45f; // degrees per second for up/down
	[Property] public float MaxBankAngle { get; set; } = 30f; // banking when turning
	[Property] public bool IsFlying { get; private set; }

	// Current flight state
	public Vector3 FlightDirection { get; private set; } = Vector3.Forward;
	public Vector3? TargetPosition { get; private set; }
	public float StopDistance { get; private set; } = 20f;

	// Internal steering
	private Vector3 _velocity;
	private float _currentBankAngle;

	protected override void OnUpdate()
	{
		if ( !IsFlying || !Npc.IsValid() )
			return;

		UpdateSteering();
		UpdateMovement();
		UpdateRotation();
	}

	/// <summary>
	/// Start flying mode
	/// </summary>
	public void StartFlying()
	{
		if ( IsFlying ) return;

		Log.Info( $"FlightLayer: {Npc.GameObject.Name} starting flight" );

		IsFlying = true;

		// Disable NavMesh agent - this is critical!
		if ( Npc.Agent.IsValid() )
		{
			Log.Info( $"FlightLayer: Disabling NavMesh agent for {Npc.GameObject.Name}" );
			Npc.Agent.Stop();
			Npc.Agent.Enabled = false;
		}

		// Initialize flight direction based on current facing
		FlightDirection = Npc.WorldRotation.Forward;
		_velocity = FlightDirection * FlightSpeed;

		Log.Info( $"FlightLayer: {Npc.GameObject.Name} flight direction: {FlightDirection}, velocity: {_velocity}" );
	}

	/// <summary>
	/// Stop flying mode
	/// </summary>
	public void StopFlying()
	{
		if ( !IsFlying ) return;

		Log.Info( $"FlightLayer: {Npc.GameObject.Name} stopping flight" );

		IsFlying = false;
		TargetPosition = null;
		_velocity = Vector3.Zero;

		// Re-enable NavMesh agent
		if ( Npc.Agent.IsValid() )
		{
			Npc.Agent.Enabled = true;
		}
	}

	/// <summary>
	/// Set a target position to fly towards
	/// </summary>
	public void FlyTowards( Vector3 target, float stopDistance = 20f )
	{
		TargetPosition = target;
		StopDistance = stopDistance;

		if ( !IsFlying )
		{
			StartFlying();
		}
	}

	/// <summary>
	/// Clear current target
	/// </summary>
	public void ClearTarget()
	{
		TargetPosition = null;
	}

	/// <summary>
	/// Check if we've reached our target
	/// </summary>
	public bool HasReachedTarget()
	{
		if ( !TargetPosition.HasValue || !Npc.IsValid() )
			return true;

		var distance = Npc.WorldPosition.Distance( TargetPosition.Value );
		var reached = distance <= StopDistance;

		if ( reached )
		{
			Log.Info( $"FlightLayer: {Npc.GameObject.Name} reached target! Distance: {distance}" );
		}

		return reached;
	}

	/// <summary>
	/// Update steering towards target with 3D movement (pitch and yaw)
	/// </summary>
	private void UpdateSteering()
	{
		if ( !TargetPosition.HasValue )
			return;

		var currentPos = Npc.WorldPosition;
		var targetPos = TargetPosition.Value;

		// Calculate desired direction (3D)
		var desiredDirection = (targetPos - currentPos).Normal;
		var currentDir = FlightDirection;

		// Calculate horizontal (yaw) and vertical (pitch) angles separately
		var currentDirHorizontal = currentDir.WithZ( 0 ).Normal;
		var desiredDirHorizontal = desiredDirection.WithZ( 0 ).Normal;

		// Handle yaw (horizontal turning)
		var yawAngle = Vector3.GetAngle( currentDirHorizontal, desiredDirHorizontal );
		if ( yawAngle > 1f && currentDirHorizontal.Length > 0.1f && desiredDirHorizontal.Length > 0.1f )
		{
			var cross = Vector3.Cross( currentDirHorizontal, desiredDirHorizontal );
			var turnDirection = cross.z > 0 ? 1f : -1f;

			var maxTurnThisFrame = TurnSpeed * Time.Delta;
			var actualTurn = MathF.Min( yawAngle, maxTurnThisFrame );

			var turnRotation = Rotation.FromAxis( Vector3.Up, actualTurn * turnDirection );
			FlightDirection = turnRotation * FlightDirection;

			// Set banking for visual effect
			_currentBankAngle = MathX.Lerp( _currentBankAngle, -turnDirection * MaxBankAngle, Time.Delta * 3f );
		}
		else
		{
			// Straighten out banking
			_currentBankAngle = MathX.Lerp( _currentBankAngle, 0f, Time.Delta * 2f );
		}

		// Handle pitch (vertical movement)
		var currentPitch = MathF.Asin( currentDir.z ) * 180f / MathF.PI;
		var desiredPitch = MathF.Asin( desiredDirection.z ) * 180f / MathF.PI;
		var pitchDifference = desiredPitch - currentPitch;

		if ( MathF.Abs( pitchDifference ) > 1f )
		{
			var maxPitchThisFrame = PitchSpeed * Time.Delta;
			var actualPitch = MathF.Sign( pitchDifference ) * MathF.Min( MathF.Abs( pitchDifference ), maxPitchThisFrame );

			// Apply pitch rotation around the right axis
			var rightAxis = Vector3.Cross( FlightDirection, Vector3.Up ).Normal;
			if ( rightAxis.Length > 0.1f ) // Make sure we have a valid right axis
			{
				var pitchRotation = Rotation.FromAxis( rightAxis, actualPitch );
				FlightDirection = pitchRotation * FlightDirection;
			}
		}

		// Ensure FlightDirection stays normalized
		FlightDirection = FlightDirection.Normal;

		// Update velocity to match new direction
		_velocity = FlightDirection * FlightSpeed;
	}

	/// <summary>
	/// Update position based on velocity
	/// </summary>
	private void UpdateMovement()
	{
		var oldPosition = Npc.WorldPosition;
		var newPosition = oldPosition + _velocity * Time.Delta;

		Npc.Transform.ClearInterpolation();
		Npc.WorldPosition = newPosition;
	}

	/// <summary>
	/// Update rotation to face flight direction with banking
	/// </summary>
	private void UpdateRotation()
	{
		// Create rotation facing flight direction
		var targetRotation = Rotation.LookAt( FlightDirection );

		// Add banking (roll) for turns
		var bankingRotation = Rotation.FromAxis( FlightDirection, _currentBankAngle );
		var finalRotation = targetRotation * bankingRotation;

		Npc.WorldRotation = Rotation.Slerp( Npc.WorldRotation, finalRotation, Time.Delta * 5f );
	}

	public override void Reset()
	{
		StopFlying();
	}
}
