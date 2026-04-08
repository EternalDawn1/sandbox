using Sandbox.UI;

/// <summary>
/// ULX-inspired admin system. All methods require the caller to be the host.
/// Uses an in-memory action log accessible via AdminSystem.Current.ActionLog.
/// </summary>
public sealed class AdminSystem : GameObjectSystem<AdminSystem>
{
	public record struct LogEntry( DateTime Time, string Admin, string Action, string Target );

	/// <summary>In-memory admin action log (last 300 entries).</summary>
	public List<LogEntry> ActionLog { get; } = new();

	public AdminSystem( Scene scene ) : base( scene ) { }

	// ─────────────────────────── Logging ───────────────────────────

	private void AddLog( string action, string target = null )
	{
		var admin = Connection.Local.DisplayName;
		ActionLog.Add( new LogEntry( DateTime.Now, admin, action, target ?? "" ) );
		if ( ActionLog.Count > 300 )
			ActionLog.RemoveAt( 0 );

		var msg = target is not null ? $"[Admin] {admin}: {action} → {target}" : $"[Admin] {admin}: {action}";
		Log.Info( msg );
	}

	// ─────────────────────── Player Commands ───────────────────────

	/// <summary>Instantly kills a player.</summary>
	public void SlayPlayer( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		player.OnDamage( new DamageInfo( 999999, player.GameObject, null ) );
		AddLog( "Slay", player.DisplayName );
	}

	/// <summary>Kicks a player from the server.</summary>
	public void KickPlayer( Guid playerId, string reason = "Kicked by admin" )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		var name = player.DisplayName;
		GameManager.Current?.Kick( player.Network.Owner, reason );
		AddLog( $"Kick ({reason})", name );
	}

	/// <summary>Permanently bans a player by Steam ID.</summary>
	public void BanPlayer( Guid playerId, string reason = "Banned by admin" )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		var name = player.DisplayName;
		BanSystem.Current?.Ban( player.Network.Owner, reason );
		AddLog( $"Ban ({reason})", name );
	}

	/// <summary>Enables or disables god mode for a player.</summary>
	public void SetGodMode( Guid playerId, bool enabled )
	{
		if ( !Networking.IsHost ) return;
		var data = PlayerData.For( playerId );
		if ( !data.IsValid() ) return;
		data.IsGodMode = enabled;
		AddLog( enabled ? "God Mode ON" : "God Mode OFF", data.DisplayName );
	}

	/// <summary>Toggles god mode for a player.</summary>
	public void ToggleGodMode( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var data = PlayerData.For( playerId );
		if ( !data.IsValid() ) return;
		SetGodMode( playerId, !data.IsGodMode );
	}

	/// <summary>Freezes or unfreezes a player in place.</summary>
	public void SetFreeze( Guid playerId, bool frozen )
	{
		if ( !Networking.IsHost ) return;
		var data = PlayerData.For( playerId );
		if ( !data.IsValid() ) return;
		data.IsFrozen = frozen;
		AddLog( frozen ? "Freeze" : "Unfreeze", data.DisplayName );
	}

	/// <summary>Toggles freeze state for a player.</summary>
	public void ToggleFreeze( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var data = PlayerData.For( playerId );
		if ( !data.IsValid() ) return;
		SetFreeze( playerId, !data.IsFrozen );
	}

	/// <summary>Enables or disables noclip for a player.</summary>
	public void SetNoclip( Guid playerId, bool enabled )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		var noclip = player.GetComponent<NoclipMoveMode>( true );
		if ( noclip.IsValid() ) noclip.Enabled = enabled;
		AddLog( enabled ? "Noclip ON" : "Noclip OFF", player.DisplayName );
	}

	/// <summary>Teleports a target player to the admin (host).</summary>
	public void TeleportToAdmin( Guid targetPlayerId )
	{
		if ( !Networking.IsHost ) return;
		var target = Player.For( targetPlayerId );
		if ( !target.IsValid() ) return;
		var admin = Player.FindLocalPlayer();
		if ( !admin.IsValid() ) return;
		target.GameObject.WorldPosition = admin.GameObject.WorldPosition + Vector3.Up * 5f;
		AddLog( "Teleport to Admin", target.DisplayName );
	}

	/// <summary>Teleports the admin (host) to a target player.</summary>
	public void TeleportAdminToPlayer( Guid targetPlayerId )
	{
		if ( !Networking.IsHost ) return;
		var target = Player.For( targetPlayerId );
		if ( !target.IsValid() ) return;
		var admin = Player.FindLocalPlayer();
		if ( !admin.IsValid() ) return;
		admin.GameObject.WorldPosition = target.GameObject.WorldPosition + Vector3.Up * 5f;
		AddLog( "Goto", target.DisplayName );
	}

	/// <summary>Restores a player's health to maximum.</summary>
	public void HealFull( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		player.Health = player.MaxHealth;
		AddLog( "Heal", player.DisplayName );
	}

	/// <summary>Restores a player's armour to maximum.</summary>
	public void GiveArmour( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		player.Armour = player.MaxArmour;
		AddLog( "Give Armour", player.DisplayName );
	}

	/// <summary>Destroys all weapons in a player's inventory.</summary>
	public void StripWeapons( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var player = Player.For( playerId );
		if ( !player.IsValid() ) return;
		var inv = player.GetComponent<PlayerInventory>();
		if ( inv.IsValid() )
		{
			foreach ( var weapon in inv.Weapons.ToList() )
				inv.Remove( weapon );
		}
		AddLog( "Strip Weapons", player.DisplayName );
	}

	/// <summary>Respawns a player at a spawn point.</summary>
	public void RespawnPlayer( Guid playerId )
	{
		if ( !Networking.IsHost ) return;
		var data = PlayerData.For( playerId );
		if ( !data.IsValid() ) return;
		var name = data.DisplayName;
		var player = Player.For( playerId );
		if ( player.IsValid() )
			player.OnDamage( new DamageInfo( 999999, player.GameObject, null ) );
		GameManager.Current?.SpawnPlayerDelayed( data );
		AddLog( "Respawn", name );
	}

	// ──────────────────────── Mass Commands ────────────────────────

	/// <summary>Slays all players on the server.</summary>
	public void SlayAll()
	{
		if ( !Networking.IsHost ) return;
		foreach ( var p in Game.ActiveScene.GetAll<Player>().ToList() )
			p.OnDamage( new DamageInfo( 999999, p.GameObject, null ) );
		AddLog( "Slay All" );
	}

	/// <summary>Respawns all players on the server.</summary>
	public void RespawnAll()
	{
		if ( !Networking.IsHost ) return;
		foreach ( var data in PlayerData.All.ToList() )
		{
			var player = Player.For( data.PlayerId );
			if ( player.IsValid() )
				player.OnDamage( new DamageInfo( 999999, player.GameObject, null ) );
			GameManager.Current?.SpawnPlayerDelayed( data );
		}
		AddLog( "Respawn All" );
	}

	/// <summary>Enables or disables god mode for ALL players.</summary>
	public void SetGodAll( bool enabled )
	{
		if ( !Networking.IsHost ) return;
		foreach ( var data in PlayerData.All )
			data.IsGodMode = enabled;
		AddLog( enabled ? "God All ON" : "God All OFF" );
	}

	/// <summary>Freezes or unfreezes ALL players.</summary>
	public void FreezeAll( bool frozen )
	{
		if ( !Networking.IsHost ) return;
		foreach ( var data in PlayerData.All )
			data.IsFrozen = frozen;
		AddLog( frozen ? "Freeze All" : "Unfreeze All" );
	}

	/// <summary>Enables or disables noclip for ALL players.</summary>
	public void SetNoclipAll( bool enabled )
	{
		if ( !Networking.IsHost ) return;
		foreach ( var p in Game.ActiveScene.GetAll<Player>() )
		{
			var noclip = p.GetComponent<NoclipMoveMode>( true );
			if ( noclip.IsValid() ) noclip.Enabled = enabled;
		}
		AddLog( enabled ? "Noclip All ON" : "Noclip All OFF" );
	}

	/// <summary>Heals all players to full health.</summary>
	public void HealAll()
	{
		if ( !Networking.IsHost ) return;
		foreach ( var p in Game.ActiveScene.GetAll<Player>() )
			p.Health = p.MaxHealth;
		AddLog( "Heal All" );
	}

	/// <summary>Strips weapons from ALL players.</summary>
	public void StripAll()
	{
		if ( !Networking.IsHost ) return;
		foreach ( var p in Game.ActiveScene.GetAll<Player>() )
		{
			var inv = p.GetComponent<PlayerInventory>();
			if ( inv.IsValid() )
				foreach ( var w in inv.Weapons.ToList() )
					inv.Remove( w );
		}
		AddLog( "Strip All Weapons" );
	}

	// ─────────────────────── Server Commands ───────────────────────

	/// <summary>
	/// Sets the world gravity scale. 1.0 = normal, 0 = no gravity, negative = reverse.
	/// </summary>
	public void SetGravityScale( float scale )
	{
		if ( !Networking.IsHost ) return;
		Scene.PhysicsWorld.Gravity = new Vector3( 0, 0, -800f * scale );
		AddLog( $"Gravity → {scale:F2}×" );
	}

	/// <summary>Sets the simulation time scale (0.1 – 5.0).</summary>
	public void SetTimescale( float scale )
	{
		if ( !Networking.IsHost ) return;
		scale = Math.Clamp( scale, 0.1f, 5f );
		Scene.TimeScale = scale;
		AddLog( $"Timescale → {scale:F2}×" );
	}

	/// <summary>Sends a system-chat broadcast to all players.</summary>
	public void BroadcastMessage( string message )
	{
		if ( !Networking.IsHost ) return;
		Scene.Get<Chat>()?.AddSystemText( $"[Admin] {message}", "📢" );
		AddLog( $"Broadcast: {message}" );
	}

	/// <summary>Changes the active map.</summary>
	public void ChangeMap( string mapName )
	{
		if ( !Networking.IsHost ) return;
		AddLog( $"Map → {mapName}" );
		ConsoleSystem.Run( "map", mapName );
	}

	/// <summary>Clears the admin action log.</summary>
	public void ClearLog()
	{
		ActionLog.Clear();
	}

	// ──────────────────── ConCmd entry-points ─────────────────────

	[ConCmd( "admin_slay" )]
	public static void CmdSlay( string target )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_slay: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data.IsValid() ) Current?.SlayPlayer( data.PlayerId );
	}

	[ConCmd( "admin_kick" )]
	public static void CmdKick( string target, string reason = "Kicked by admin" )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_kick: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data.IsValid() ) Current?.KickPlayer( data.PlayerId, reason );
	}

	[ConCmd( "admin_ban" )]
	public static void CmdBan( string target, string reason = "Banned by admin" )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_ban: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data.IsValid() ) Current?.BanPlayer( data.PlayerId, reason );
	}

	[ConCmd( "admin_god" )]
	public static void CmdGod( string target )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_god: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data.IsValid() ) Current?.ToggleGodMode( data.PlayerId );
	}

	[ConCmd( "admin_freeze" )]
	public static void CmdFreeze( string target )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_freeze: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data.IsValid() ) Current?.ToggleFreeze( data.PlayerId );
	}

	[ConCmd( "admin_noclip" )]
	public static void CmdNoclip( string target )
	{
		if ( !Networking.IsHost ) return;
		var conn = GameManager.FindPlayerWithName( target );
		if ( conn is null ) { Log.Warning( $"admin_noclip: player '{target}' not found" ); return; }
		var data = PlayerData.For( conn );
		if ( data is null ) return;
		var player = Player.For( data.PlayerId );
		if ( !player.IsValid() ) return;
		var nc = player.GetComponent<NoclipMoveMode>( true );
		if ( nc.IsValid() ) Current?.SetNoclip( data.PlayerId, !nc.Enabled );
	}

	[ConCmd( "admin_gravity" )]
	public static void CmdGravity( float scale = 1f ) => Current?.SetGravityScale( scale );

	[ConCmd( "admin_timescale" )]
	public static void CmdTimescale( float scale = 1f ) => Current?.SetTimescale( scale );

	[ConCmd( "admin_say" )]
	public static void CmdSay( string message ) => Current?.BroadcastMessage( message );
}
