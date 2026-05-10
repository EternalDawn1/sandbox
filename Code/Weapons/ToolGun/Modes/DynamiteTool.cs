[Icon( "💣" )]
[Title( "#tool.name.dynamite" )]
[ClassName( "dynamite" )]
[Group( "#tool.group.building" )]
public class DynamiteTool : ToolMode
{
	[Property, Sync, Range( 1, 100 )]
	public float BlastRadius { get; set; } = 200f;

	[Property, Sync, Range( 1, 10000 )]
	public float BlastForce { get; set; } = 5000f;

	[Property, Sync, Range( 0, 10 )]
	public float FuseTime { get; set; } = 3f;

	[Property, Sync]
	public bool AutoIgnite { get; set; } = true;

	public override string Description => "#tool.hint.dynamite.description";
	public override string PrimaryAction => "#tool.hint.dynamite.place";
	public override string SecondaryAction => "#tool.hint.dynamite.ignite";
	public override string ReloadAction => "#tool.hint.dynamite.remove";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			SpawnDynamite( select, BlastRadius, BlastForce, FuseTime, AutoIgnite );
			ShootEffects( select );
		}

		if ( Input.Pressed( "attack2" ) )
		{
			IgniteDynamite( select.GameObject );
			ShootEffects( select );
		}

		if ( Input.Pressed( "reload" ) )
		{
			RemoveDynamite( select.GameObject );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void SpawnDynamite( SelectionPoint select, float radius, float force, float fuse, bool autoIgnite )
	{
		if ( !select.IsValid() ) return;

		var go = new GameObject( false, "dynamite" );
		go.Tags.Add( "removable" );
		go.WorldTransform = select.WorldTransform();

		var dyn = go.AddComponent<WireDynamiteEntity>();
		dyn.BlastRadius = radius;
		dyn.BlastForce = force;
		dyn.FuseTime = fuse;
		dyn.AutoIgnite = autoIgnite;

		go.NetworkSpawn();

		var undo = Player.Undo.Create();
		undo.Name = "Dynamite";
		undo.Icon = "💣";
		undo.Add( go );
	}

	[Rpc.Host]
	public void IgniteDynamite( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var dyn = go.GetComponent<WireDynamiteEntity>();
		if ( dyn.IsValid() )
			dyn.Ignite();
	}

	[Rpc.Host]
	public void RemoveDynamite( GameObject go )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		if ( !root.Tags.Contains( "removable" ) ) return;

		root.Destroy();
	}
}

public class WireDynamiteEntity : Component
{
	[Property, Sync]
	public float BlastRadius { get; set; } = 200f;

	[Property, Sync]
	public float BlastForce { get; set; } = 5000f;

	[Property, Sync]
	public float FuseTime { get; set; } = 3f;

	[Property, Sync]
	public bool AutoIgnite { get; set; } = true;

	[Property, Sync]
	public bool IsLit { get; set; } = false;

	float _fuseTimer;
	bool _exploded;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		if ( AutoIgnite )
			Ignite();
	}

	protected override void OnUpdate()
	{
		if ( !IsLit || _exploded ) return;

		_fuseTimer += Time.Delta;
		if ( _fuseTimer >= FuseTime )
			Explode();
	}

	public void Ignite()
	{
		IsLit = true;
		_fuseTimer = 0f;
	}

	[Rpc.Broadcast]
	public void Explode()
	{
		if ( _exploded ) return;
		_exploded = true;

		var pos = WorldPosition;

		var trace = Scene.Trace.Ray( pos, Vector3.Zero )
			.Radius( BlastRadius )
			.RunAll();

		foreach ( var result in trace )
		{
			if ( !result.Hit ) continue;

			var go = result.Body?.GameObject;
			if ( !go.IsValid() ) continue;

			var rb = go.GetComponent<Rigidbody>();
			if ( !rb.IsValid() ) continue;

			var dir = ( result.EndPosition - pos ).Normal;
			var dist = ( result.EndPosition - pos ).Length;
			if ( dist < 0.01f ) dist = 0.01f;
			var force = BlastForce * ( 1f - dist / BlastRadius );
			rb.ApplyImpulse( dir * force );
		}

		GameObject.Destroy();
	}
}
