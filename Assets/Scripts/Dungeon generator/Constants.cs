using System;
using System.Collections.Generic;
using UnityEngine;

// Contains constants and util methods
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
		SecretChest,
		SecretGround,
		SecretDoor,
		SecretPath,
		SecretWall,
		SecretPathWall,
		DestroyableWall,
		Entrance,
		Exit,
		ShopItem,
		Shop
	}
	public enum RoomTypes
	{
		Treasure,
		Secret,
		Intersection,
		Default,
		Entrance,
		Exit,
		Shop
	}
	public enum LootTypes
	{
		Shop,
		Enemy,
		RoomClear
	}
	public enum ChestType
	{
		Common,
		Rare
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
			int p_roomID = -1,
			int p_pathID = -1
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
		public static Vector3Int RoundPosition(Vector3 position)
		{
			return new Vector3Int((int)Math.Floor(position.x), (int)Math.Floor(position.y), 0);
		}
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
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T GetArrayRandom<T>(this T[] array)
		{
			return array[UnityEngine.Random.Range(0, array.Length)];
		}
	}
}