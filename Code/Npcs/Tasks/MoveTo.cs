namespace Sandbox.Npcs;

/// <summary>
/// Move to a location
/// </summary>
public sealed class MoveTo : TaskBase
{
	public Vector3 Target { get; set; }
	public float Distance { get; set; } = 10f;

	public MoveTo( Vector3 targetPosition, float stopDistance = 10f )
	{
		Target = targetPosition;
		Distance = stopDistance;
	}

	public override async Task Execute()
	{
		Npc.Agent.MoveTo( Target );

		while ( Npc.WorldPosition.Distance( Target ) > Distance )
		{
			await FrameEnd();
		}
	}
}
