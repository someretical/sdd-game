// This file contains all of the code used to generate the dungeon

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGeneratorNamespace
{
	public class DungeonGenerator
	{
		public readonly int columns;
		public readonly int rows;
		public readonly int maximumRooms;
		public readonly int intersectionRoomProbability;
		public readonly int minimumChestRooms;
		public readonly int maximumChestRooms;
		public readonly int minimumSecretRooms;
		public readonly int maximumSecretRooms;
		public readonly int minimumShopRooms;
		public readonly int maximumShopRooms;
		public readonly int minimumSinglePathLength;
		public readonly int maximumSinglePathLength;
		public readonly int minimumMultiPathSegmentLength;
		public readonly int maximumMultiPathSegmentLength;
		public readonly int minimumMultiPathTotal;
		public readonly int maximumMultiPathTotal;
		public readonly int minimumPathTurns;
		public readonly int maximumPathTurns;
		public readonly int pathTurnProbability;
		public readonly int maximumAttempts;
		public readonly RoomManager roomManager;
		public Tile[,] Map
		{
			get;
		}
		public List<List<Vector2Int>> RoomPoints
		{
			get;
		}
		public List<List<Vector2Int>> PathPoints
		{
			get;
		}
		public DungeonGenerator(
			RoomManager p_roomManager,
			int p_columns,
			int p_rows,
			int p_maximumRooms,
			int p_intersectionRoomProbability,
			int p_minimumChestRooms,
			int p_maximumChestRooms,
			int p_minimumSecretRooms,
			int p_maximumSecretRooms,
			int p_minimumShopRooms,
			int p_maximumShopRooms,
			int p_minimumSinglePathLength,
			int p_maximumSinglePathLength,
			int p_minimumMultiPathSegmentLength,
			int p_maximumMultiPathSegmentLength,
			int p_minimumMultiPathTotal,
			int p_maximumMultiPathTotal,
			int p_minimumPathTurns,
			int p_maximumPathTurns,
			int p_pathTurnProbability,
			int p_maximumAttempts
		)
		{
			roomManager = p_roomManager;
			columns = p_columns;
			rows = p_rows;
			maximumRooms = p_maximumRooms;
			intersectionRoomProbability = p_intersectionRoomProbability;
			minimumChestRooms = p_minimumChestRooms;
			maximumChestRooms = p_maximumChestRooms;
			minimumSecretRooms = p_minimumSecretRooms;
			maximumSecretRooms = p_maximumSecretRooms;
			minimumShopRooms = p_minimumShopRooms;
			maximumShopRooms = p_maximumShopRooms;
			minimumSinglePathLength = p_minimumSinglePathLength;
			maximumSinglePathLength = p_maximumSinglePathLength;
			minimumMultiPathSegmentLength = p_minimumMultiPathSegmentLength;
			maximumMultiPathSegmentLength = p_maximumMultiPathSegmentLength;
			minimumMultiPathTotal = p_minimumMultiPathTotal;
			maximumMultiPathTotal = p_maximumMultiPathTotal;
			minimumPathTurns = p_minimumPathTurns;
			maximumPathTurns = p_maximumPathTurns;
			pathTurnProbability = p_pathTurnProbability;
			maximumAttempts = p_maximumAttempts;

			Map = new Tile[p_columns, p_rows];
			RoomPoints = new List<List<Vector2Int>>();
			PathPoints = new List<List<Vector2Int>>();
		}
		public List<int> GeneratePathLengths(int turnCount)
		{
			// Generates a list of fixed length where the elements' sum is maximumPathLength
			// This approach removes the need to shuffle the list afterwards as there is roughly no bias introduced
			// other than the cryptographically insecure Random.Range() of course
			// Code adapted from https://stackoverflow.com/a/473383 :)
			var lengths = new List<int>();
			for (var i = 0; i < turnCount; ++i)
				lengths.Add(minimumMultiPathSegmentLength);

			var length = Random.Range(minimumMultiPathTotal, maximumMultiPathTotal + 1);

			while (Util.GetListSum(lengths) < length)
			{
				var index = Random.Range(0, lengths.Count);

				if (lengths[index] < maximumMultiPathSegmentLength)
					++lengths[index];
			}

			return lengths;
		}
		public bool InBounds(int x, int y)
		{
			return !(x < 0 || x >= columns || y < 0 || y >= rows);
		}
		public void BuildRoom(Vector2Int topLeft, Room room, bool secret = false)
		{
			// Put a room preset onto the Map
			for (var x = 0; x < room.Width; ++x)
				for (var y = 0; y < room.Height; ++y)
					if (room.tiles[y, x].type != TileTypes.Any)
					{
						// All secret room tiles only have a path ID
						// Also convert secret doors into a secret path tile
						Tile newTile;
						if (secret)
						{
							// Don't need to reveal void tiles
							int actualPathID = room.tiles[y, x].type == TileTypes.Void
								? -1
								: PathPoints.Count - 1;

							newTile = new Tile(room.tiles[y, x].type, room.tiles[y, x].rotation, 0, actualPathID);

							if (actualPathID != -1)
								PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x + topLeft.x, y + topLeft.y));

							if (newTile.type == TileTypes.SecretDoor)
							{
								newTile.type = TileTypes.SecretPath;
								newTile.rotation = Rotations.None;
							}
						}
						else
						{
							// Don't need to reveal void tiles
							int actualRoomID = room.tiles[y, x].type == TileTypes.Void
								? -1
								: RoomPoints.Count - 1;

							newTile = new Tile(room.tiles[y, x].type, room.tiles[y, x].rotation, actualRoomID);

							if (actualRoomID != -1)
								RoomPoints[RoomPoints.Count - 1].Add(new Vector2Int(x + topLeft.x, y + topLeft.y));
						}

						Map[x + topLeft.x, y + topLeft.y] = newTile;
					}
			// VERY VERY VERY IMPORTANT THAT THIS IS y, x NOT x, y
			// Also the tile instance has to be a new one, 
			// otherwise it copies by reference which is not what we want
			// The room instance is static which is why this must be prevented
			// Spent like 3 hours fixing these two elusive bugs
		}
		public void InstantiateMap()
		{
			RoomPoints.Clear();
			PathPoints.Clear();

			// Fill Map with Any tiles and create the centre room
			for (int x = 0; x < columns; ++x)
				for (int y = 0; y < rows; ++y)
				{
					// Set the outer boundaries
					if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1)
						Map[x, y] = new Tile(TileTypes.OuterWall, Rotations.None);
					else
						Map[x, y] = new Tile(TileTypes.Any, Rotations.None);
				}

			RoomPoints.Add(new List<Vector2Int>());
			BuildRoom(new Vector2Int(columns / 2, rows / 2), Util.GetListRandom(roomManager.entranceRooms));
		}
		public List<Tuple<Tile, Vector2Int>> GetAvailableDoors()
		{
			// Get a list of all unconnected doors on the Map
			var doors = new List<Tuple<Tile, Vector2Int>>();

			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (Map[x, y].type == TileTypes.Door)
					{
						switch (Map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && Map[x, y - 1].type != TileTypes.Path && Map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(Map[x, y], new Vector2Int(x, y)));
								break;
							case Rotations.South:
								if (y != rows - 1 && Map[x, y + 1].type != TileTypes.Path && Map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(Map[x, y], new Vector2Int(x, y)));
								break;
							case Rotations.East:
								if (x != columns - 1 && Map[x + 1, y].type != TileTypes.Path && Map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(Map[x, y], new Vector2Int(x, y)));
								break;
							case Rotations.West:
								if (x != 0 && Map[x - 1, y].type != TileTypes.Path && Map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(Map[x, y], new Vector2Int(x, y)));
								break;
						}
					}

			return doors;
		}
		public Tuple<bool, List<Vector2Int>> CheckPathLocations(Vector2Int topLeft, Vector2Int bottomRight, List<Vector2Int> extraPoints)
		{
			// Actually check that path can be placed
			var placable = true;
			var points = new List<Vector2Int>();

			for (var i = 0; i < extraPoints.Count; i++)
				if (!InBounds(extraPoints[i].x, extraPoints[i].y) || Map[extraPoints[i].x, extraPoints[i].y].type != TileTypes.Any)
					return Tuple.Create(false, points);

			for (var x = topLeft.x; x <= bottomRight.x; ++x)
				for (var y = topLeft.y; y <= bottomRight.y; ++y)
					if (!InBounds(x, y) || Map[x, y].type != TileTypes.Any)
						return Tuple.Create(false, points);
					else
						points.Add(new Vector2Int(x, y));

			return Tuple.Create(placable, points);
		}
		public Rotations GetNewDirection(Rotations current)
		{
			// Pick new direction for a path with turns
			// The new direction has to be perpendicular to the current one

			if (current == Rotations.North || current == Rotations.South)
				return Random.Range(0, 2) == 0 ? Rotations.East : Rotations.West;
			else
				return Random.Range(0, 2) == 0 ? Rotations.North : Rotations.South;
		}
		public Tuple<List<Vector2Int>, List<Tuple<Vector2Int, Rotations, int>>> LookaheadPath(Tuple<Tile, Vector2Int> doorTuple)
		{
			// Check that path can be placed
			var (door, point) = doorTuple;
			var segments = new List<Tuple<Vector2Int, Rotations, int>>();
			var pathPoints = new List<Vector2Int>();

			var createTurns = Random.Range(0, 100) < pathTurnProbability;
			var turnCount = createTurns ? Random.Range(minimumPathTurns, maximumPathTurns + 1) : 0;

			// Create copies of doorTuple data since they are passed to the function by reference NOT value
			// Copy enum https://stackoverflow.com/a/17878912
			var currentDirection = (Rotations)((int)door.rotation);
			var currentPoint = new Vector2Int(point.x, point.y);

			var pathLengths = createTurns ?
				GeneratePathLengths(turnCount + 1) :
				new List<int> { Random.Range(minimumSinglePathLength, maximumSinglePathLength + 1) };

			var breakLoop = false;
			for (var i = 0; (i < pathLengths.Count) && !breakLoop; ++i)
			{
				switch (currentDirection)
				{
					case Rotations.North:
						// ++/++
						//  S.
						//   .
						//   .E
						// ++/++
						// S = top left
						// E = bottom right
						// + = wall
						// . = path
						// / = door
						// Different points for all 4 directions
						// Need the range to check that it is all clear so the path can be placed
						// Top left (for north at least) must also be moved up 1 more in case of a path that turns
						// but also passes close by another room. This is why extraPoints exists
						// Long story short, a path that can't be walled can be formed
						// Since it doesn't matter if the same is done for a path without a turn, 
						// this behaviour becomes the default.
						// I don't know if this will actually fix the rare and annoying bug I keep on seeing
						// but fingers crossed it will.
						// If it doesn't, the game is still functional 99% of the time so it's good enough for me lmao
						var topLeft = new Vector2Int(currentPoint.x - 1, currentPoint.y - pathLengths[i]);
						var bottomRight = new Vector2Int(currentPoint.x + 1, currentPoint.y - 1);
						var extraPoints = new List<Vector2Int> {
							new Vector2Int(currentPoint.x + 1, currentPoint.y - pathLengths[i] - 1),
							new Vector2Int(currentPoint.x, currentPoint.y - pathLengths[i] - 1),
							new Vector2Int(currentPoint.x - 1, currentPoint.y - pathLengths[i] - 1),
						};

						var (placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight, extraPoints);

						if (placable)
						{
							segments.Add(Tuple.Create(new Vector2Int(currentPoint.x, currentPoint.y - pathLengths[i]), Rotations.South, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.East:
						topLeft = new Vector2Int(currentPoint.x + 1, currentPoint.y - 1);
						bottomRight = new Vector2Int(currentPoint.x + pathLengths[i], currentPoint.y + 1);
						extraPoints = new List<Vector2Int> {
							new Vector2Int(currentPoint.x + pathLengths[i] + 1, currentPoint.y + 1),
							new Vector2Int(currentPoint.x + pathLengths[i] + 1, currentPoint.y),
							new Vector2Int(currentPoint.x + pathLengths[i] + 1, currentPoint.y - 1),
						};

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight, extraPoints);

						if (placable)
						{
							segments.Add(Tuple.Create(new Vector2Int(currentPoint.x + pathLengths[i], currentPoint.y), Rotations.West, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.South:
						topLeft = new Vector2Int(currentPoint.x - 1, currentPoint.y + 1);
						bottomRight = new Vector2Int(currentPoint.x + 1, currentPoint.y + pathLengths[i]);
						extraPoints = new List<Vector2Int> {
							new Vector2Int(currentPoint.x + 1, currentPoint.y + pathLengths[i] + 1),
							new Vector2Int(currentPoint.x, currentPoint.y + pathLengths[i] + 1),
							new Vector2Int(currentPoint.x - 1, currentPoint.y + pathLengths[i] + 1),
						};

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight, extraPoints);

						if (placable)
						{
							segments.Add(Tuple.Create(new Vector2Int(currentPoint.x, currentPoint.y + pathLengths[i]), Rotations.North, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.West:
						topLeft = new Vector2Int(currentPoint.x - pathLengths[i], currentPoint.y - 1);
						bottomRight = new Vector2Int(currentPoint.x - 1, currentPoint.y + 1);
						extraPoints = new List<Vector2Int> {
							new Vector2Int(currentPoint.x - pathLengths[i] - 1, currentPoint.y + 1),
							new Vector2Int(currentPoint.x - pathLengths[i] - 1, currentPoint.y),
							new Vector2Int(currentPoint.x - pathLengths[i] - 1, currentPoint.y - 1),
						};

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight, extraPoints);

						if (placable)
						{
							segments.Add(Tuple.Create(new Vector2Int(currentPoint.x - pathLengths[i], currentPoint.y), Rotations.East, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
				}

				if (!breakLoop && pathLengths.Count > 1)
				{
					// Update point
					currentPoint = new Vector2Int(segments[segments.Count - 1].Item1.x, segments[segments.Count - 1].Item1.y);
					// Get new direction for path
					currentDirection = GetNewDirection(currentDirection);
				}
			}

			return Tuple.Create(pathPoints, segments);
		}
		public List<Vector2Int> GetDirectionalDoors(Room room, Rotations facing)
		{
			// Get a list of relative points of doors in a preset that are facing a certain direction
			var doors = new List<Vector2Int>();

			for (var y = 0; y < room.Height; ++y)
				for (var x = 0; x < room.Width; ++x)
					if (
						room.tiles[y, x].rotation == facing &&
						// Could expand but nah
						(room.tiles[y, x].type == TileTypes.Door || room.tiles[y, x].type == TileTypes.SecretDoor)
						)
						doors.Add(new Vector2Int(x, y));
			// VERY IMPORTANT THAT IT IS y, x because of the way the presets are made
			// Presets are ordered y then x for readability (and pain when handling :| )

			return doors;
		}
		public Tuple<bool, Room, Vector2Int> LookaheadRoom(List<Room> possibleRooms, List<Vector2Int> pathPoints, Vector2Int pathEndPoint, Rotations doorDirection)
		{
			// Makes sure that the room can be placed
			var failed = Tuple.Create(false, roomManager.entranceRooms[0], new Vector2Int(-1, -1));
			var randomRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];
			var doors = GetDirectionalDoors(randomRoom, doorDirection);

			// Not all rooms may have doors on a specific side
			// Hence try a certain amount of times to get a suitable room
			for (var i = 0; (i < maximumAttempts) && (doors.Count == 0); ++i)
			{
				randomRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];
				doors = GetDirectionalDoors(randomRoom, doorDirection);
			}

			if (doors.Count == 0)
				return failed;

			var randomDoor = doors[Random.Range(0, doors.Count)];

			// Preset room door location is randomDoor.x, randomDoor.y
			// Since we know where the door location is relative to the PRESET room,
			// We can translate the origin of the room (top left corner of the PRESET) 
			// to the coordinates on the real Map where we will ultimately place the room
			// ... which is what this (mind bending) switch statement does :)
			Vector2Int topLeft;
			switch (doorDirection)
			{
				case Rotations.North:
					// Real Map door location is pathEndPoint.x, pathEndPoint.y + 1
					topLeft = new Vector2Int(pathEndPoint.x - randomDoor.x, pathEndPoint.y + 1 - randomDoor.y);
					break;
				case Rotations.East:
					// Real Map door location is pathEndPoint.x - 1, pathEndPoint.y
					topLeft = new Vector2Int(pathEndPoint.x - 1 - randomDoor.x, pathEndPoint.y - randomDoor.y);
					break;
				case Rotations.South:
					// Real Map door location is pathEndPoint.x, pathEndPoint.y - 1
					topLeft = new Vector2Int(pathEndPoint.x - randomDoor.x, pathEndPoint.y - 1 - randomDoor.y);
					break;
				case Rotations.West:
					// Real Map door location is pathEndPoint.x + 1, pathEndPoint.y
					topLeft = new Vector2Int(pathEndPoint.x + 1 - randomDoor.x, pathEndPoint.y - randomDoor.y);
					break;
				default:
					return failed;
			}

			for (var x = topLeft.x; x < topLeft.x + randomRoom.Width; ++x)
			{
				for (var y = topLeft.y; y < topLeft.y + randomRoom.Height; ++y)
				{
					if (!InBounds(x, y))
						return failed;

					var pass = true;

					// This logic was a pain to do since it needs 2 passes
					// It probably could be combined into one if statement
					// but I'm not big brained enough to figure it all out
					if (
						Map[x, y].type != TileTypes.Any &&
						randomRoom.tiles[y - topLeft.y, x - topLeft.x].type != TileTypes.Any
					)
						pass = false;

					// Basically this allows for a 1 long path between rooms
					// since it doesn't matter if a void tile overwrites another void tile
					// (same for the 'any' tile)
					if (
						Map[x, y].type == TileTypes.Any &&
						randomRoom.tiles[y - topLeft.y, x - topLeft.x].type == TileTypes.Any ||
						Map[x, y].type == TileTypes.Void &&
						randomRoom.tiles[y - topLeft.y, x - topLeft.x].type == TileTypes.Void
					)
						pass = true;

					if (!pass)
						return failed;

					for (var i = 0; i < pathPoints.Count; ++i)
					{
						if (
								pathPoints[i].x == x &&
								pathPoints[i].y == y &&
								// This check below took me like 3 iterations to get right but it works perfectly now
								randomRoom.tiles[y - topLeft.y, x - topLeft.x].type != TileTypes.Any
							)
							return failed;
					}
				}
			}

			return Tuple.Create(true, randomRoom, topLeft);
		}
		public void BuildPathTile(int x, int y, TileTypes type)
		{
			// Actually set the tile
			Map[x, y].type = type;
			Map[x, y].pathID = PathPoints.Count - 1;
			PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x, y));
		}
		public void SetPathIDForDoor(int x, int y)
		{
			// Set the path ID for a door tile
			if (Map[x, y].type == TileTypes.Door)
			{
				Map[x, y].pathID = PathPoints.Count - 1;
				PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x, y));
				switch (Map[x, y].rotation)
				{
					// Place east and west tiles there as well
					case Rotations.North:
					// FALL THROUGH
					case Rotations.South:
						Map[x + 1, y].pathID = PathPoints.Count - 1;
						PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x + 1, y));
						Map[x - 1, y].pathID = PathPoints.Count - 1;
						PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x - 1, y));
						break;

					// Place north and south wall tiles there as well
					case Rotations.East:
					// FALL THROUGH
					case Rotations.West:
						Map[x, y - 1].pathID = PathPoints.Count - 1;
						PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x, y - 1));
						Map[x, y + 1].pathID = PathPoints.Count - 1;
						PathPoints[PathPoints.Count - 1].Add(new Vector2Int(x, y + 1));
						break;
				}
			}
		}
		public void BuildPath(Vector2Int start, Rotations direction, int length, bool secret)
		{
			// Put a path segment onto the Map
			var type = secret ? TileTypes.SecretPath : TileTypes.Path;

			// The direction is relative to END of the current segment
			// which is relative to the NEW room which would have been 
			// just placed before this path will be placed
			// Just realised how confusing all this really is
			switch (direction)
			{
				case Rotations.North:
					SetPathIDForDoor(start.x, start.y + 1);
					SetPathIDForDoor(start.x, start.y - length);

					for (var y = start.y; y > start.y - length; --y)
						BuildPathTile(start.x, y, type);
					break;
				case Rotations.East:
					SetPathIDForDoor(start.x - 1, start.y);
					SetPathIDForDoor(start.x + length, start.y);

					for (var x = start.x; x < start.x + length; ++x)
						BuildPathTile(x, start.y, type);
					break;
				case Rotations.South:
					SetPathIDForDoor(start.x, start.y - 1);
					SetPathIDForDoor(start.x, start.y + length);

					for (var y = start.y; y < start.y + length; ++y)
						BuildPathTile(start.x, y, type);
					break;
				case Rotations.West:
					SetPathIDForDoor(start.x + 1, start.y);
					SetPathIDForDoor(start.x - length, start.y);

					for (var x = start.x; x > start.x - length; --x)
						BuildPathTile(x, start.y, type);
					break;
			}
		}
		public List<Vector2Int> GetSurroundingEmpty(Vector2Int p)
		{
			// Get a list of tiles that are empty AND adjacent to the provided coordinate
			var empty = new List<Vector2Int>();

			// North
			if (Map[p.x, p.y - 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x, p.y - 1));

			// North east
			if (Map[p.x + 1, p.y - 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x + 1, p.y - 1));

			// East
			if (Map[p.x + 1, p.y].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x + 1, p.y));

			// South east
			if (Map[p.x + 1, p.y + 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x + 1, p.y + 1));

			// South
			if (Map[p.x, p.y + 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x, p.y + 1));

			// South west
			if (Map[p.x - 1, p.y + 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x - 1, p.y + 1));

			// West
			if (Map[p.x - 1, p.y].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x - 1, p.y));

			// // North west
			if (Map[p.x - 1, p.y - 1].type == TileTypes.Any)
				empty.Add(new Vector2Int(p.x - 1, p.y - 1));

			return empty;
		}
		public void WallPaths()
		{
			// Surround all paths with walls
			for (var x = 0; x < columns; ++x)
				for (var y = 0; y < rows; ++y)
					if (Map[x, y].type == TileTypes.Path || Map[x, y].type == TileTypes.SecretPath)
					{
						var surrounding = GetSurroundingEmpty(new Vector2Int(x, y));

						for (var i = 0; i < surrounding.Count; ++i)
						{
							var sx = surrounding[i].x;
							var sy = surrounding[i].y;

							if (Map[x, y].type == TileTypes.SecretPath)
								Map[sx, sy].type = TileTypes.SecretPathWall;
							else
								Map[sx, sy].type = TileTypes.PathWall;

							Map[sx, sy].pathID = PathPoints.Count - 1;
							PathPoints[PathPoints.Count - 1].Add(new Vector2Int(sx, sy));
						}
					}
		}
		public bool AddRoom(RoomTypes type, bool secret = false)
		{
			var doors = GetAvailableDoors();

			if (doors.Count == 0)
				return false;

			for (var i = 0; i < maximumAttempts; ++i)
			{
				var randomDoor = doors[Random.Range(0, doors.Count)];

				// LookaheadPath returns the opposite coordinates from where the path will start
				// Technically should be called pathEndPoint, 
				// but it's called pathStartPoint to make consistent with pathDirection
				// This is so that the same parameters can be fed into the LookaheadRoom function
				var (pathPoints, pathSegments) = LookaheadPath(randomDoor);
				if (pathSegments.Count == 0)
					continue;

				// Final path segment data
				var pathSegmentStartPoint = pathSegments[pathSegments.Count - 1].Item1;
				var finalSegmentDirection = pathSegments[pathSegments.Count - 1].Item2;
				var possibleRooms = type == RoomTypes.Default ?
						roomManager.defaultRooms :
					type == RoomTypes.Intersection ?
						roomManager.intersectionRooms :
					type == RoomTypes.Treasure ?
						roomManager.treasureRooms :
					type == RoomTypes.Secret ?
						roomManager.secretRooms :
					type == RoomTypes.Entrance ?
						roomManager.entranceRooms :
					type == RoomTypes.Shop ?
						roomManager.shopRooms :
					roomManager.exitRooms;

				var (roomIsBuildable, room, topLeftPoint) = LookaheadRoom(possibleRooms, pathPoints, pathSegmentStartPoint, finalSegmentDirection);
				if (!roomIsBuildable)
					continue;

				PathPoints.Add(new List<Vector2Int>());
				if (type != RoomTypes.Secret)
					RoomPoints.Add(new List<Vector2Int>());

				BuildRoom(topLeftPoint, room, secret);

				for (var k = 0; k < pathSegments.Count; ++k)
				{
					var (segmentStartPoint, segmentDirection, segmentLength) = pathSegments[k];

					BuildPath(segmentStartPoint, segmentDirection, segmentLength, secret);
				}

				WallPaths();

				return true;
			}

			return false;
		}
		public bool AddRooms()
		{
			// Attach new rooms to existing ones

			// Set to 1 because already generated entrance
			var roomCounter = 1;
			var chestRoomCount = Random.Range(minimumChestRooms, maximumChestRooms);
			var secretRoomCount = Random.Range(minimumSecretRooms, maximumSecretRooms);
			var shopRoomCount = Random.Range(minimumShopRooms, maximumShopRooms);
			var chestRoomCounter = 0;
			var secretRoomCounter = 0;
			var shopRoomCounter = 0;

			// Add in default rooms
			// -1 at the end because will need to add in exit too
			var roomsLeft = maximumRooms - chestRoomCount - secretRoomCount - shopRoomCount - 1;
			for (var i = 0; (roomCounter < roomsLeft) && (i < maximumAttempts); ++i)
			{
				var addIntersection = Random.Range(0, 100) < intersectionRoomProbability;
				var success = AddRoom(addIntersection ? RoomTypes.Intersection : RoomTypes.Default);

				if (success)
					++roomCounter;
			}

			// Add in chest rooms
			for (var i = 0; (chestRoomCounter < chestRoomCount) && (i < maximumAttempts); ++i)
				if (AddRoom(RoomTypes.Treasure))
					++chestRoomCounter;

			// Add in secret rooms
			// This process is really complicated but here it is:
			// Secret rooms aren't supposed to have doors at all
			// They are supposed to be hidden by a destroyable wall which replaces a regular wall
			// in a regular room of course
			// The secret door is placed there just so that secret rooms can be added in
			// using the same process for regular rooms
			// The secret doors will be replaced by secret ground tiles when the HideSecretRooms
			// function is called
			for (var i = 0; (secretRoomCounter < secretRoomCount) && (i < maximumAttempts); ++i)
				if (AddRoom(RoomTypes.Secret, true))
					++secretRoomCounter;

			// Shop rooms were added much later than the rest of the code here
			// I'm so glad I took the time to refactor this crap
			// Otherwise I would have spent literally ages trying to get it to work
			for (var i = 0; (shopRoomCounter < shopRoomCount) && (i < maximumAttempts); ++i)
				if (AddRoom(RoomTypes.Shop))
					++shopRoomCounter;

			// Add in exit
			for (var i = 0; i < maximumAttempts; ++i)
				if (AddRoom(RoomTypes.Exit))
					return true;

			return false;
		}
		public void HideSecretRooms()
		{
			// Removes secret doors and places in destroyable walls where necessary
			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (Map[x, y].type == TileTypes.Door)
					{
						switch (Map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && Map[x, y - 1].type == TileTypes.SecretPath)
									Map[x, y].type = TileTypes.DestroyableWall;
								break;
							case Rotations.South:
								if (y == rows - 1 || Map[x, y + 1].type == TileTypes.SecretPath)
									Map[x, y].type = TileTypes.DestroyableWall;
								break;
							case Rotations.East:
								if (x == columns - 1 || Map[x + 1, y].type == TileTypes.SecretPath)
									Map[x, y].type = TileTypes.DestroyableWall;
								break;
							case Rotations.West:
								if (x == 0 || Map[x - 1, y].type == TileTypes.SecretPath)
									Map[x, y].type = TileTypes.DestroyableWall;
								break;
						}
					}
		}
		public void BlockDoors()
		{
			// Checks for doors that are not attached to paths
			// Replaces those doors with walls instead
			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (Map[x, y].type == TileTypes.Door)
					{
						switch (Map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && Map[x, y - 1].type != TileTypes.Path)
								{
									Map[x, y].type = TileTypes.Wall;
									Map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.South:
								if (y == rows - 1 || Map[x, y + 1].type != TileTypes.Path)
								{
									Map[x, y].type = TileTypes.Wall;
									Map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.East:
								if (x == columns - 1 || Map[x + 1, y].type != TileTypes.Path)
								{
									Map[x, y].type = TileTypes.Wall;
									Map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.West:
								if (x == 0 || Map[x - 1, y].type != TileTypes.Path)
								{
									Map[x, y].type = TileTypes.Wall;
									Map[x, y].rotation = Rotations.None;
								}
								break;
						}
					}
		}
		public void ReplaceOuterWalls()
		{
			// Replaces outer walls with void tiles
			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (Map[x, y].type == TileTypes.OuterWall)
						Map[x, y].type = TileTypes.Void;
		}
		public void Generate()
		{
			while (true)
			{
				InstantiateMap();

				if (AddRooms())
				{
					HideSecretRooms();
					BlockDoors();
					ReplaceOuterWalls();

					break;
				}
			}
		}
	}
}