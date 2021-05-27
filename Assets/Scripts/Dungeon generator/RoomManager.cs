using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DungeonGeneratorNamespace
{
	public class RoomManager
	{
		public List<Room> defaultRooms = new List<Room>();
		public List<Room> entranceRooms = new List<Room>();
		public List<Room> exitRooms = new List<Room>();
		public List<Room> intersectionRooms = new List<Room>();
		public List<Room> secretRooms = new List<Room>();
		public List<Room> treasureRooms = new List<Room>();
		public void Init(GameManager gameManager)
		{
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.defaultRoomsJSON.text).rooms, defaultRooms);
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.entranceRoomsJSON.text).rooms, entranceRooms);
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.exitRoomsJSON.text).rooms, exitRooms);
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.intersectionRoomsJSON.text).rooms, intersectionRooms);
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.secretRoomsJSON.text).rooms, secretRooms);
			RotateRooms(JsonConvert.DeserializeObject<JSONRoomData>(gameManager.treasureRoomsJSON.text).rooms, treasureRooms);
		}
		public static Tile[,] ConvertToTiles(string[,] matrix)
		{
			// Rows come first, then columns
			Tile[,] tiles = new Tile[matrix.GetLength(0), matrix.GetLength(1)];

			for (int y = 0; y < matrix.GetLength(0); ++y)
				for (int x = 0; x < matrix.GetLength(1); ++x)
					switch (matrix[y, x])
					{
						case "V_":
							tiles[y, x] = new Tile(TileTypes.Void);
							break;
						case "W_":
							tiles[y, x] = new Tile(TileTypes.Wall);
							break;
						case "G_":
							tiles[y, x] = new Tile(TileTypes.Ground);
							break;
						case "DN":
							tiles[y, x] = new Tile(TileTypes.Door, Rotations.North);
							break;
						case "DE":
							tiles[y, x] = new Tile(TileTypes.Door, Rotations.East);
							break;
						case "DS":
							tiles[y, x] = new Tile(TileTypes.Door, Rotations.South);
							break;
						case "DW":
							tiles[y, x] = new Tile(TileTypes.Door, Rotations.West);
							break;
						case "PL":
							tiles[y, x] = new Tile(TileTypes.Pillar);
							break;
						case "PI":
							tiles[y, x] = new Tile(TileTypes.Pit);
							break;
						case "GO":
							tiles[y, x] = new Tile(TileTypes.Goo);
							break;
						case "SG":
							tiles[y, x] = new Tile(TileTypes.SecretGround);
							break;
						case "SW":
							tiles[y, x] = new Tile(TileTypes.SecretWall);
							break;
						case "SDN":
							tiles[y, x] = new Tile(TileTypes.SecretDoor, Rotations.North);
							break;
						case "SDE":
							tiles[y, x] = new Tile(TileTypes.SecretDoor, Rotations.East);
							break;
						case "SDS":
							tiles[y, x] = new Tile(TileTypes.SecretDoor, Rotations.South);
							break;
						case "SDW":
							tiles[y, x] = new Tile(TileTypes.SecretDoor, Rotations.West);
							break;
						case "EN":
							tiles[y, x] = new Tile(TileTypes.Entrance);
							break;
						case "EX":
							tiles[y, x] = new Tile(TileTypes.Exit);
							break;
						case "CN":
							tiles[y, x] = new Tile(TileTypes.Chest, Rotations.North);
							break;
						case "CE":
							tiles[y, x] = new Tile(TileTypes.Chest, Rotations.East);
							break;
						case "CS":
							tiles[y, x] = new Tile(TileTypes.Chest, Rotations.South);
							break;
						case "CW":
							tiles[y, x] = new Tile(TileTypes.Chest, Rotations.West);
							break;
						default:
							tiles[y, x] = new Tile(TileTypes.Any);
							break;
					}

			return tiles;
		}
		public static void RotateRooms(List<string[,]> roomList, List<Room> rooms)
		{
			for (int i = 0; i < roomList.Count; ++i)
			{
				var room = new Room(ConvertToTiles(roomList[i]));
				rooms.Add(room);
				rooms.Add(RotateRoom(room, Rotations.East));
				rooms.Add(RotateRoom(room, Rotations.South));
				rooms.Add(RotateRoom(room, Rotations.West));
			}
		}
		public static Room RotateRoom(Room baseRoom, Rotations direction)
		{
			// Rotate a room, code shamelessly ripped off from https://stackoverflow.com/a/18035050
			Tile[,] copy = new Tile[baseRoom.Width, baseRoom.Height];

			switch (direction)
			{
				case Rotations.East:
					// Rotate 90

					var newRow = 0;
					for (var oldColumn = 0; oldColumn < baseRoom.Width; ++oldColumn)
					{
						var newColumn = 0;

						for (var oldRow = baseRoom.Height - 1; oldRow >= 0; --oldRow)
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
					for (var oldColumn = baseRoom.Width - 1; oldColumn >= 0; --oldColumn)
					{
						var newColumn = 0;

						for (var oldRow = 0; oldRow < baseRoom.Height; ++oldRow)
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
					copy = new Tile[baseRoom.Height, baseRoom.Width];

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

			return new Room(copy);
		}

	}
}