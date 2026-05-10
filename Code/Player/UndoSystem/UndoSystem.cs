using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

public class UndoSystem : GameObjectSystem<UndoSystem>
{
	Dictionary<long, PlayerStack> stacks = new();

	public UndoSystem( Scene scene ) : base( scene )
	{
	}

	public PlayerStack For( long steamId )
	{
		if ( !stacks.TryGetValue( steamId, out var stack ) )
		{
			stack = new PlayerStack( steamId );
			stacks[steamId] = stack;
		}
		return stack;
	}

	public void RemovePlayer( long steamId )
	{
		stacks.Remove( steamId );
	}

	public void Remove( GameObject go )
	{
		foreach ( var stack in stacks.Values )
		{
			stack.Remove( go );
		}
	}

	public class PlayerStack
	{
		long steamId;
		List<Entry> entries = new();
		List<Entry> redoStack = new();
		const int MaxUndoSteps = 128;

		public PlayerStack( long steamId )
		{
			this.steamId = steamId;
		}

		public Entry Create()
		{
			var entry = new Entry( steamId );
			entries.Add( entry );
			redoStack.Clear();

			if ( entries.Count > MaxUndoSteps )
			{
				entries.RemoveAt( 0 );
			}

			return entry;
		}

		public void Undo()
		{
			while ( entries.Count > 0 )
			{
				var entry = entries[^1];
				entries.RemoveAt( entries.Count - 1 );

				if ( entry.Run() )
				{
					redoStack.Add( entry );
					return;
				}
			}
		}

		public void Redo()
		{
			while ( redoStack.Count > 0 )
			{
				var entry = redoStack[^1];
				redoStack.RemoveAt( redoStack.Count - 1 );

				if ( entry.Run( sendNotice: false ) )
				{
					entries.Add( entry );
					if ( entries.Count > MaxUndoSteps )
						entries.RemoveAt( 0 );

					var c = Connection.All.FirstOrDefault( x => x.SteamId == steamId );
					if ( c is not null )
					{
						using ( Rpc.FilterInclude( c ) )
						{
							RedoNotice( entry.Name );
						}
					}
					return;
				}
			}
		}

		public void Remove( GameObject go )
		{
			foreach ( var entry in entries )
				entry.Remove( go );
			foreach ( var entry in redoStack )
				entry.Remove( go );
		}

		[Rpc.Broadcast]
		public static void RedoNotice( string title )
		{
			Notices.AddNotice( "cached", "#3273eb", $"Redo {title}".Trim(), 5 );
			Sound.Play( "sounds/ui/ui.undo.sound" );
		}
	}

	public class Entry
	{
		public string Name { get; set; }
		public string Icon { get; set; }

		long steamId;

		HashSet<GameObject> gameObjects = new();

		internal Entry( long steamId )
		{
			this.steamId = steamId;
		}

		public void Add( GameObject go )
		{
			gameObjects.Add( go );
		}

		public void Add( params IEnumerable<GameObject> gos )
		{
			foreach ( var go in gos )
			{
				Add( go );
			}
		}

		public void Remove( GameObject go )
		{
			gameObjects.Remove( go );
		}

		public bool Run( bool sendNotice = true )
		{
			var actioned = false;

			foreach ( var go in gameObjects )
			{
				if ( go.IsValid() )
				{
					go.Destroy();
					actioned = true;
				}
			}

			if ( !actioned )
				return false;

			if ( sendNotice )
			{
				var c = Connection.All.FirstOrDefault( x => x.SteamId == steamId );
				if ( c is not null )
				{
					using ( Rpc.FilterInclude( c ) )
					{
						UndoNotice( Name );
					}
				}
			}

			return true;
		}

		[Rpc.Broadcast]
		public static void UndoNotice( string title )
		{
			Notices.AddNotice( "cached", "#3273eb", $"Undo {title}".Trim(), 5 );
			Sound.Play( "sounds/ui/ui.undo.sound" );
		}
	}
}
