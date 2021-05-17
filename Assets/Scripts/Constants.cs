// This file holds all of the room presets and functions for rotating rooms
// It also holds a bunch of enums that are used in DungeonGenerator.cs
// Finally it holds the declarations of a few other classes

using System.Collections.Generic;

namespace DungeonGenerator
{
	public enum Rotations
	{
		North,
		East,
		South,
		West,
		None
	}
	public enum TileTypes
	{
		Void,
		Any,
		OuterWall,
		Wall,
		Ground,
		Door,
		Path,
		PathWall,
		Pillar,
		Pit,
		Goo,
		Chest,
		SecretGround,
		SecretDoor,
		SecretPath,
		SecretWall,
		SecretPathWall,
		DestroyableWall,
		Entrance,
		Exit
	}
	public enum RoomTypes
	{
		Treasure,
		Secret,
		Intersection,
		Default,
		Entrance,
		Exit
	}
	public class Point
	{
		public int x;
		public int y;
		public Point(int X, int Y)
		{
			x = X;
			y = Y;
		}
	}
	public class Tile
	{
		public TileTypes type;
		public Rotations rotation;
		public int roomID;
		public int pathID;
		public Tile(TileTypes p_type, Rotations p_rotation, int p_roomID = 0, int p_pathID = 0)
		{
			type = p_type;
			rotation = p_rotation;
			roomID = p_roomID;
			pathID = p_pathID;
		}
	}
	public class Room
	{
		public Tile[,] tiles;
		public int width
		{
			get => tiles.GetLength(1);
		}
		public int height
		{
			get => tiles.GetLength(0);
		}
		public Room(Tile[,] t)
		{
			tiles = t;
		}
	}
	public class Tiles
	{
		// Ease of access
		public static readonly Tile V_ = new Tile(TileTypes.Void, Rotations.None);
		public static readonly Tile A_ = new Tile(TileTypes.Any, Rotations.None);
		public static readonly Tile P_ = new Tile(TileTypes.Pillar, Rotations.None);
		public static readonly Tile G_ = new Tile(TileTypes.Ground, Rotations.None);
		public static readonly Tile W_ = new Tile(TileTypes.Wall, Rotations.None);
		public static readonly Tile DN = new Tile(TileTypes.Door, Rotations.North);
		public static readonly Tile DE = new Tile(TileTypes.Door, Rotations.East);
		public static readonly Tile DS = new Tile(TileTypes.Door, Rotations.South);
		public static readonly Tile DW = new Tile(TileTypes.Door, Rotations.West);
		public static readonly Tile PI = new Tile(TileTypes.Pit, Rotations.None);
		public static readonly Tile GO = new Tile(TileTypes.Goo, Rotations.None);
		public static readonly Tile C_ = new Tile(TileTypes.Chest, Rotations.North);
		public static readonly Tile SG = new Tile(TileTypes.SecretGround, Rotations.None);
		public static readonly Tile SW = new Tile(TileTypes.SecretWall, Rotations.None);
		public static readonly Tile SP = new Tile(TileTypes.SecretPath, Rotations.None);
		public static readonly Tile SD = new Tile(TileTypes.SecretDoor, Rotations.North);
		public static readonly Tile EN = new Tile(TileTypes.Entrance, Rotations.None);
		public static readonly Tile EX = new Tile(TileTypes.Exit, Rotations.None);
	}
	public class Rooms
	{
		private static bool alreadyRotated = false;
		// Room presets
		public static readonly List<Room> Entrances = new List<Room>
		{
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.EN, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			})
		};
		public static readonly List<Room> Exits = new List<Room>
		{
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.EX, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			})
		};
		public static readonly List<Room> Treasures = new List<Room>
		{
			// Thin chest room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.C_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
			// Diamond shaped chest room
			new Room(new Tile[,] {
				{Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_},
				{Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.C_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_},
				{Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_},
				{Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
			}),
		};
		public static readonly List<Room> Secret = new List<Room>
		{
			// Small secret room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SW, Tiles.SD, Tiles.SW, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.C_, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
			// Large secret room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SD, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.C_, Tiles.SG, Tiles.C_, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.C_, Tiles.SG, Tiles.C_, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SG, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.SW, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
		};
		public static readonly List<Room> Intersections = new List<Room>
		{
			// Square room with multiple connections per side
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.DE, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.DE, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_},
			}),
			// Origin room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.P_, Tiles.G_, Tiles.G_, Tiles.DE, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
		};
		public static readonly List<Room> Default = new List<Room>
		{
			// L shaped room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.DE, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_}
			}),
			// L shaped room with goo
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.A_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.GO, Tiles.GO, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.GO, Tiles.GO, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.DE, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.A_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_}
			}),
			// Long rectangular room
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DS, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
			// Square room with only 2 doorways
			new Room(new Tile[,] {
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.A_, Tiles.A_, Tiles.A_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.DN, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.A_, Tiles.DW, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.A_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.G_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.W_, Tiles.V_},
				{Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_, Tiles.V_},
			}),
		};
		private static Tile[,] RotateRoom(Room baseRoom, Rotations direction)
		{
			// Rotate a room, code shamelessly ripped off from https://stackoverflow.com/a/18035050
			Tile[,] copy = new Tile[baseRoom.width, baseRoom.height];

			switch (direction)
			{
				case Rotations.East:
					// Rotate 90

					var newRow = 0;
					for (var oldColumn = 0; oldColumn < baseRoom.width; ++oldColumn)
					{
						var newColumn = 0;

						for (var oldRow = baseRoom.height - 1; oldRow >= 0; --oldRow)
						{
							copy[newRow, newColumn] = baseRoom.tiles[oldRow, oldColumn];
							newColumn++;
						}

						newRow++;
					}

					// Fix doors
					for (var row = 0; row < copy.GetLength(0); ++row)
						for (var column = 0; column < copy.GetLength(1); ++column)
							switch (copy[row, column].rotation)
							{
								case Rotations.North:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.East);
									break;
								case Rotations.East:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.South);
									break;
								case Rotations.South:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.West);
									break;
								case Rotations.West:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.North);
									break;
							}
					break;
				case Rotations.West:
					// Rotate -90

					newRow = 0;
					for (var oldColumn = baseRoom.width - 1; oldColumn >= 0; --oldColumn)
					{
						var newColumn = 0;

						for (var oldRow = 0; oldRow < baseRoom.height; ++oldRow)
						{
							copy[newRow, newColumn] = baseRoom.tiles[oldRow, oldColumn];
							newColumn++;
						}

						newRow++;
					}

					// Fix doors
					for (var row = 0; row < copy.GetLength(0); ++row)
						for (var column = 0; column < copy.GetLength(1); ++column)
							switch (copy[row, column].rotation)
							{
								case Rotations.North:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.West);
									break;
								case Rotations.East:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.North);
									break;
								case Rotations.South:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.East);
									break;
								case Rotations.West:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.South);
									break;
							}
					break;
				default:
					// Rotate 180
					copy = new Tile[baseRoom.height, baseRoom.width];

					// Reverse rows
					for (var row = 0; row < copy.GetLength(0); ++row)
						for (var column = 0; column < copy.GetLength(1); ++column)
							copy[row, copy.GetLength(1) - 1 - column] = baseRoom.tiles[row, column];

					// Reverse columns
					for (var column = 0; column < copy.GetLength(1); ++column)
						for (var row = 0; row < (int)(copy.GetLength(0) / 2); ++row)
						{
							// Create copies in order to not mess up references
							TileTypes type = copy[row, column].type, type2 = copy[copy.GetLength(0) - 1 - row, column].type;
							Rotations rotation = copy[row, column].rotation, rotation2 = copy[copy.GetLength(0) - 1 - row, column].rotation;

							copy[row, column] = new Tile(type2, rotation2);
							copy[copy.GetLength(0) - 1 - row, column] = new Tile(type, rotation);
						}

					// Fix doors
					for (var row = 0; row < copy.GetLength(0); ++row)
						for (var column = 0; column < copy.GetLength(1); ++column)
							switch (copy[row, column].rotation)
							{
								case Rotations.North:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.South);
									break;
								case Rotations.East:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.West);
									break;
								case Rotations.South:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.North);
									break;
								case Rotations.West:
									copy[row, column] = new Tile(copy[row, column].type, Rotations.East);
									break;
							}
					break;
			}

			return copy;
		}
		public static void RotateRoomPresets()
		{
			// Rotate rooms since the presets only provide the north orientation
			// Intersection rooms are not yet rotated for obvious reasons...

			if (alreadyRotated)
				return;

			// Rotate entrance rooms
			var c = Rooms.Entrances.Count;
			for (var i = 0; i < c; ++i)
			{
				Rooms.Entrances.Add(new Room(RotateRoom(Rooms.Entrances[i], Rotations.East)));
				Rooms.Entrances.Add(new Room(RotateRoom(Rooms.Entrances[i], Rotations.South)));
				Rooms.Entrances.Add(new Room(RotateRoom(Rooms.Entrances[i], Rotations.West)));
			}

			// Rotate exit rooms
			c = Rooms.Exits.Count;
			for (var i = 0; i < c; ++i)
			{
				Rooms.Exits.Add(new Room(RotateRoom(Rooms.Exits[i], Rotations.East)));
				Rooms.Exits.Add(new Room(RotateRoom(Rooms.Exits[i], Rotations.South)));
				Rooms.Exits.Add(new Room(RotateRoom(Rooms.Exits[i], Rotations.West)));
			}

			// Rotate default rooms
			c = Rooms.Default.Count;
			for (var i = 0; i < c; ++i)
			{
				Rooms.Default.Add(new Room(RotateRoom(Rooms.Default[i], Rotations.East)));
				Rooms.Default.Add(new Room(RotateRoom(Rooms.Default[i], Rotations.South)));
				Rooms.Default.Add(new Room(RotateRoom(Rooms.Default[i], Rotations.West)));
			}

			// Rotate treasure rooms
			c = Rooms.Treasures.Count;
			for (var i = 0; i < c; ++i)
			{
				Rooms.Treasures.Add(new Room(RotateRoom(Rooms.Treasures[i], Rotations.East)));
				Rooms.Treasures.Add(new Room(RotateRoom(Rooms.Treasures[i], Rotations.South)));
				Rooms.Treasures.Add(new Room(RotateRoom(Rooms.Treasures[i], Rotations.West)));
			}

			// Rotate secret rooms
			c = Rooms.Secret.Count;
			for (var i = 0; i < c; ++i)
			{
				Rooms.Secret.Add(new Room(RotateRoom(Rooms.Secret[i], Rotations.East)));
				Rooms.Secret.Add(new Room(RotateRoom(Rooms.Secret[i], Rotations.South)));
				Rooms.Secret.Add(new Room(RotateRoom(Rooms.Secret[i], Rotations.West)));
			}

			alreadyRotated = true;
		}
	}
}