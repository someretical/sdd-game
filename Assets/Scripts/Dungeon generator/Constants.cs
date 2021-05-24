using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneratorNamespace
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
	public class Tile
	{
		public TileTypes type;
		public Rotations rotation;
		public int roomID;
		public int pathID;
		public Tile(
			TileTypes p_type,
			Rotations p_rotation = Rotations.None,
			int p_roomID = 0,
			int p_pathID = 0
		)
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
		public int Width
		{
			get => tiles.GetLength(1);
		}
		public int Height
		{
			get => tiles.GetLength(0);
		}
		public Room(Tile[,] t)
		{
			tiles = t;
		}
	}
	public class JSONRoomData
	{
		public List<string[,]> rooms;
	}
	public static class Util
	{
		public static int GetListSum(List<int> list)
		{
			// Basically reduce/aggregate
			var sum = 0;

			for (var i = 0; i < list.Count; ++i)
				sum += list[i];

			return sum;
		}
		public static T GetListRandom<T>(this List<T> list)
		{
			return list[Random.Range(0, list.Count)];
		}

		public static T GetArrayRandom<T>(this T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}
	}
}