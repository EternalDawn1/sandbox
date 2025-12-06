namespace Sandbox.Npcs.Tasks;

public sealed class LookAt : TaskBase
{
	public Vector3? Target { get; set; }
	public GameObject Object { get; set; }
	public float Speed { get; set; } = 8f;

	public LookAt( Vector3 target, float speed = 8f )
	{
		Target = target;
		Speed = speed;
	}

	public LookAt( GameObject target, float speed = 8f )
	{
		Object = target;
		Speed = speed;
	}

	public override async Task Execute()
	{
		while ( !IsLookingAtTarget() && !IsCancelled )
		{
			var targetPos = GetTargetPosition();
			if ( !targetPos.HasValue ) return;

			var direction = (targetPos.Value - Npc.WorldPosition).Normal;
			var targetRotation = Rotation.LookAt( direction );

			var lerpSpeed = Speed * Time.Delta;
			Npc.SetBodyTarget( Rotation.Lerp( Npc.WorldRotation, targetRotation, lerpSpeed ) );

			await FrameEnd();
		}
	}

	private bool IsLookingAtTarget()
	{
		var targetPos = GetTargetPosition();
		if ( !targetPos.HasValue ) return true;

		var direction = (targetPos.Value - Npc.WorldPosition).Normal;
		var targetRotation = Rotation.LookAt( direction );

		return Npc.WorldRotation.Forward.Dot( targetRotation.Forward ) > 0.999f; // crude but it works
	}

	private Vector3? GetTargetPosition()
	{
		if ( Object.IsValid() ) return Object.WorldPosition;
		if ( Target.HasValue ) return Target.Value;
		return null;
	}
}
