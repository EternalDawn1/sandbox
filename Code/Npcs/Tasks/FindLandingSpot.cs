using Sandbox.Npcs.Layers;

namespace Sandbox.Npcs.Tasks;

/// <summary>
/// Task that finds and flies to a safe landing location
/// </summary>
public class FindLandingSpot : TaskBase
{
    public float MinDistance { get; set; } = 512f;
    public float MaxDistance { get; set; } = 2048f;
    public Vector3 AvoidPosition { get; set; }

    private FlightLayer _flight;
    private Vector3? _landingSpot;

    public FindLandingSpot( Vector3 avoidPosition, float minDistance = 512f, float maxDistance = 2048f )
    {
        AvoidPosition = avoidPosition;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }

    protected override void OnStart()
    {
        _flight = Layer<FlightLayer>();
        _landingSpot = SearchForLandingSpot();

        if ( _landingSpot.HasValue && _flight is not null )
        {
            _flight.FlyTowards( _landingSpot.Value, 20f );
        }
    }

    protected override TaskStatus OnUpdate()
    {
        if ( _flight is null || !_landingSpot.HasValue )
            return TaskStatus.Failed;

        return _flight.HasReachedTarget() ? TaskStatus.Success : TaskStatus.Running;
    }

    /// <summary>
    /// Search for a valid landing spot on the NavMesh
    /// </summary>
    private Vector3? SearchForLandingSpot()
    {
        if ( !Npc.IsValid() ) return null;

        for ( int attempts = 0; attempts < 20; attempts++ )
        {
            var direction = Vector3.Random.WithZ( 0 ).Normal;
            var distance = Game.Random.Float( MinDistance, MaxDistance );
            var testPosition = AvoidPosition + (direction * distance);

            var navMeshPoint = Behavior.Scene.NavMesh.GetClosestPoint( testPosition );
            if ( navMeshPoint.HasValue )
            {
                var landingSpot = navMeshPoint.Value;
                if ( landingSpot.Distance( AvoidPosition ) >= MinDistance )
                {
                    return landingSpot;
                }
            }
        }

        return null;
    }

    public Vector3? GetLandingSpot() => _landingSpot;
}
