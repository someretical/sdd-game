using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace DungeonGenerator
{
	public class MapGenerator
	{
		private int columns;
		private int rows;
		private int maximumRooms;
		private int intersectionRoomProbability;
		private int minimumChestRooms;
		private int maximumChestRooms;
		private int minimumSecretRooms;
		private int maximumSecretRooms;
		private int minimumSinglePathLength;
		private int maximumSinglePathLength;
		private int minimumMultiPathSegmentLength;
		private int maximumMultiPathSegmentLength;
		private int minimumMultiPathTotal;
		private int maximumMultiPathTotal;
		private int minimumPathTurns;
		private int maximumPathTurns;
		private int pathTurnProbability;
		private int maximumAttempts;
		public Tile[,] map
		{
			get;
		}
		public MapGenerator(
			int p_columns,
			int p_rows,
			int p_maximumRooms,
			int p_intersectionRoomProbability,
			int p_minimumChestRooms,
			int p_maximumChestRooms,
			int p_minimumSecretRooms,
			int p_maximumSecretRooms,
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
			columns = p_columns;
			rows = p_rows;
			maximumRooms = p_maximumRooms;
			intersectionRoomProbability = p_intersectionRoomProbability;
			minimumChestRooms = p_minimumChestRooms;
			maximumChestRooms = p_maximumChestRooms;
			minimumSecretRooms = p_minimumSecretRooms;
			maximumSecretRooms = p_maximumSecretRooms;
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

			map = new Tile[p_columns, p_rows];
		}
		public static int GetListSum(List<int> list)
		{
			// Basically reduce/aggregate
			var sum = 0;

			for (var i = 0; i < list.Count; ++i)
				sum += list[i];

			return sum;
		}
		public List<int> GeneratePathLengths(int turnCount)
		{
			// Generates a list of fixed length where the elements' sum is maximumPathLength
			// This approach removes the need to shuffle the list afterwards as there is roughly no bias introduced
			// other than the cryptographically insecure Random.Range() of course
			// Code adapted from https://stackoverflow.com/a/473383 :)
			List<int> lengths = new List<int> { };
			for (var i = 0; i < turnCount; ++i)
				lengths.Add(minimumMultiPathSegmentLength);

			var length = Random.Range(minimumMultiPathTotal, maximumMultiPathTotal + 1);

			while (GetListSum(lengths) < length)
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
		public void BuildRoom(Point topLeft, Room room)
		{
			// Put a room preset onto the map
			for (var x = 0; x < room.width; ++x)
				for (var y = 0; y < room.height; ++y)
					if (room.tiles[y, x].type != TileTypes.Any)
						map[x + topLeft.x, y + topLeft.y] = new Tile(room.tiles[y, x].type, room.tiles[y, x].rotation);
			// VERY VERY VERY IMPORTANT THAT THIS IS y, x NOT x, y
			// Also the tile instance has to be a new one, 
			// otherwise it copies by reference which is not what we want
			// The room instance is static which is why this must be prevented
			// Spent like 3 hours fixing these two elusive bugs
		}
		public void InstantiateMap()
		{
			// Fill map with Any tiles and create the centre room
			for (int x = 0; x < columns; ++x)
				for (int y = 0; y < rows; ++y)
				{
					// Set the outer boundaries
					if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1)
						map[x, y] = new Tile(TileTypes.OuterWall, Rotations.None);
					else
						map[x, y] = new Tile(TileTypes.Any, Rotations.None);
				}

			BuildRoom(new Point(columns / 2, rows / 2), Rooms.Entrances[Random.Range(0, Rooms.Entrances.Count)]);
		}
		public List<Tuple<Tile, Point>> GetAvailableDoors()
		{
			// Get a list of all unconnected doors on the map
			List<Tuple<Tile, Point>> doors = new List<Tuple<Tile, Point>> { };

			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (map[x, y].type == TileTypes.Door)
					{
						switch (map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && map[x, y - 1].type != TileTypes.Path && map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(map[x, y], new Point(x, y)));
								break;
							case Rotations.South:
								if (y != rows - 1 && map[x, y + 1].type != TileTypes.Path && map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(map[x, y], new Point(x, y)));
								break;
							case Rotations.East:
								if (x != columns - 1 && map[x + 1, y].type != TileTypes.Path && map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(map[x, y], new Point(x, y)));
								break;
							case Rotations.West:
								if (x != 0 && map[x - 1, y].type != TileTypes.Path && map[x, y - 1].type != TileTypes.SecretPath)
									doors.Add(Tuple.Create(map[x, y], new Point(x, y)));
								break;
						}
					}

			return doors;
		}
		public Tuple<bool, List<Point>> CheckPathLocations(Point topLeft, Point bottomRight)
		{
			// Actually check that path can be placed
			var placable = true;
			List<Point> points = new List<Point> { };

			for (var x = topLeft.x; x <= bottomRight.x; ++x)
				for (var y = topLeft.y; y <= bottomRight.y; ++y)
					if (!InBounds(x, y) || map[x, y].type != TileTypes.Any)
						placable = false;
					else
						points.Add(new Point(x, y));

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
		public Tuple<List<Point>, List<Tuple<Point, Rotations, int>>> LookaheadPath(Tuple<Tile, Point> doorTuple)
		{
			// Check that path can be placed
			var (door, point) = doorTuple;
			List<Tuple<Point, Rotations, int>> segments = new List<Tuple<Point, Rotations, int>> { };
			List<Point> pathPoints = new List<Point> { };

			var createTurns = Random.Range(0, 100) < pathTurnProbability;
			var turnCount = createTurns ? Random.Range(minimumPathTurns, maximumPathTurns + 1) : 0;

			// Create copies of doorTuple data since they are passed to the function by reference NOT value
			// Copy enum https://stackoverflow.com/a/17878912
			var currentDirection = (Rotations)((int)door.rotation);
			var currentPoint = new Point(point.x, point.y);

			var pathLengths = createTurns ?
				GeneratePathLengths(turnCount + 1) :
				new List<int> { Random.Range(minimumSinglePathLength, maximumSinglePathLength + 1) };

			var breakLoop = false;
			for (var i = 0; (i < pathLengths.Count) && !breakLoop; ++i)
			{
				switch (currentDirection)
				{
					case Rotations.North:
						// ++^++
						//  S.
						//   .
						//   .E
						// ++^++
						// S = top left
						// E = bottom right
						// Different points for all 4 directions
						// Need the range to check that it is all clear so the path can be placed
						var topLeft = new Point(currentPoint.x - 1, currentPoint.y - pathLengths[i]);
						var bottomRight = new Point(currentPoint.x + 1, currentPoint.y - 1);

						var (placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight);

						if (placable)
						{
							segments.Add(Tuple.Create(new Point(currentPoint.x, currentPoint.y - pathLengths[i]), Rotations.South, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.East:
						topLeft = new Point(currentPoint.x + 1, currentPoint.y - 1);
						bottomRight = new Point(currentPoint.x + pathLengths[i], currentPoint.y + 1);

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight);

						if (placable)
						{
							segments.Add(Tuple.Create(new Point(currentPoint.x + pathLengths[i], currentPoint.y), Rotations.West, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.South:
						topLeft = new Point(currentPoint.x - 1, currentPoint.y + 1);
						bottomRight = new Point(currentPoint.x + 1, currentPoint.y + pathLengths[i]);

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight);

						if (placable)
						{
							segments.Add(Tuple.Create(new Point(currentPoint.x, currentPoint.y + pathLengths[i]), Rotations.North, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
					case Rotations.West:
						topLeft = new Point(currentPoint.x - pathLengths[i], currentPoint.y - 1);
						bottomRight = new Point(currentPoint.x - 1, currentPoint.y + 1);

						(placable, pointsToCheck) = CheckPathLocations(topLeft, bottomRight);

						if (placable)
						{
							segments.Add(Tuple.Create(new Point(currentPoint.x - pathLengths[i], currentPoint.y), Rotations.East, pathLengths[i]));

							pathPoints.AddRange(pointsToCheck);
						}
						else
							breakLoop = true;

						break;
				}

				if (!breakLoop && pathLengths.Count > 1)
				{
					// Update point
					currentPoint = new Point(segments[segments.Count - 1].Item1.x, segments[segments.Count - 1].Item1.y);
					// Get new direction for path
					currentDirection = GetNewDirection(currentDirection);
				}
			}

			return Tuple.Create(pathPoints, segments);
		}
		public List<Point> GetDirectionalDoors(Room room, Rotations facing)
		{
			// Get a list of relative points of doors in a preset that are facing a certain direction
			List<Point> doors = new List<Point> { };

			for (var y = 0; y < room.height; ++y)
				for (var x = 0; x < room.width; ++x)
					if (
						room.tiles[y, x].rotation == facing &&
						// Could expand but nah
						(room.tiles[y, x].type == TileTypes.Door || room.tiles[y, x].type == TileTypes.SecretDoor)
						)
						doors.Add(new Point(x, y));
			// VERY IMPORTANT THAT IT IS y, x because of the way the presets are made
			// Presets are ordered y then x for readability (and pain when handling :| )

			return doors;
		}
		public Tuple<bool, Room, Point> LookaheadRoom(List<Room> possibleRooms, List<Point> pathPoints, Point pathEndPoint, Rotations doorDirection)
		{
			// Makes sure that the room can be placed
			var failed = Tuple.Create(false, Rooms.Entrances[0], new Point(-1, -1));
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
			// to the coordinates on the real map where we will ultimately place the room
			// ... which is what this (mind bending) switch statement does :)
			Point topLeft;
			switch (doorDirection)
			{
				case Rotations.North:
					// Real map door location is pathEndPoint.x, pathEndPoint.y + 1
					topLeft = new Point(pathEndPoint.x - randomDoor.x, pathEndPoint.y + 1 - randomDoor.y);
					break;
				case Rotations.East:
					// Real map door location is pathEndPoint.x - 1, pathEndPoint.y
					topLeft = new Point(pathEndPoint.x - 1 - randomDoor.x, pathEndPoint.y - randomDoor.y);
					break;
				case Rotations.South:
					// Real map door location is pathEndPoint.x, pathEndPoint.y - 1
					topLeft = new Point(pathEndPoint.x - randomDoor.x, pathEndPoint.y - 1 - randomDoor.y);
					break;
				case Rotations.West:
					// Real map door location is pathEndPoint.x + 1, pathEndPoint.y
					topLeft = new Point(pathEndPoint.x + 1 - randomDoor.x, pathEndPoint.y - randomDoor.y);
					break;
				default:
					return failed;
			}

			for (var x = topLeft.x; x < topLeft.x + randomRoom.width; ++x)
			{
				for (var y = topLeft.y; y < topLeft.y + randomRoom.height; ++y)
				{
					if (!InBounds(x, y))
						return failed;

					var pass = true;

					// This logic was a pain to do since it needs 2 passes
					// It probably could be combined into one if statement
					// but I'm not big brained enough to figure it all out
					if (
						map[x, y].type != TileTypes.Any &&
						randomRoom.tiles[y - topLeft.y, x - topLeft.x].type != TileTypes.Any
					)
						pass = false;

					// Basically this allows for a 1 long path between rooms
					// since it doesn't matter if a void tile overwrites another void tile
					// (same for the 'any' tile)
					if (
						map[x, y].type == TileTypes.Any &&
						randomRoom.tiles[y - topLeft.y, x - topLeft.x].type == TileTypes.Any ||
						map[x, y].type == TileTypes.Void &&
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
		public void BuildPath(Point start, Rotations direction, int length, bool secret)
		{
			// Put all the path segments onto the map
			var type = secret ? TileTypes.SecretPath : TileTypes.Path;

			switch (direction)
			{
				case Rotations.North:
					for (var y = start.y; y > start.y - length; --y)
					{
						map[start.x, y].type = type;
						map[start.x, y].rotation = Rotations.None;
					}
					break;
				case Rotations.East:
					for (var x = start.x; x < start.x + length; ++x)
					{
						map[x, start.y].type = type;
						map[x, start.y].rotation = Rotations.None;
					}
					break;
				case Rotations.South:
					for (var y = start.y; y < start.y + length; ++y)
					{
						map[start.x, y].type = type;
						map[start.x, y].rotation = Rotations.None;
					}
					break;
				case Rotations.West:
					for (var x = start.x; x > start.x - length; --x)
					{
						map[x, start.y].type = type;
						map[x, start.y].rotation = Rotations.None;
					}
					break;
			}
		}
		public List<Point> GetAdjacentEmpty(Point p)
		{
			// Get a list of tiles that are empty AND adjacent to the provided coordinate
			List<Point> empty = new List<Point> { };

			// North
			if (map[p.x, p.y - 1].type == TileTypes.Any)
				empty.Add(new Point(p.x, p.y - 1));

			// East
			if (map[p.x + 1, p.y].type == TileTypes.Any)
				empty.Add(new Point(p.x + 1, p.y));

			// South
			if (map[p.x, p.y + 1].type == TileTypes.Any)
				empty.Add(new Point(p.x, p.y + 1));

			// West
			if (map[p.x - 1, p.y].type == TileTypes.Any)
				empty.Add(new Point(p.x - 1, p.y));

			return empty;
		}
		public void WallPaths()
		{
			// Surround all paths with walls

			for (var x = 0; x < columns; ++x)
				for (var y = 0; y < rows; ++y)
					if (map[x, y].type == TileTypes.Path || map[x, y].type == TileTypes.SecretPath)
					{
						var surrounding = GetAdjacentEmpty(new Point(x, y));

						for (var i = 0; i < surrounding.Count; ++i)
							if (map[x, y].type == TileTypes.SecretPath)
								map[surrounding[i].x, surrounding[i].y].type = TileTypes.SecretPathWall;
							else
								map[surrounding[i].x, surrounding[i].y].type = TileTypes.PathWall;
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
				var (finalSegmentStartPoint, finalSegmentDirection, l) = pathSegments[pathSegments.Count - 1];

				var possibleRooms = type == RoomTypes.Default ?
					Rooms.Default :
					type == RoomTypes.Intersection ?
					Rooms.Intersections :
					type == RoomTypes.Treasure ?
					Rooms.Treasures :
					type == RoomTypes.Secret ?
					Rooms.Secret :
					type == RoomTypes.Entrance ?
					Rooms.Entrances :
					Rooms.Exits;

				var (roomIsBuildable, room, topLeftPoint) = LookaheadRoom(possibleRooms, pathPoints, finalSegmentStartPoint, finalSegmentDirection);
				if (!roomIsBuildable)
					continue;

				BuildRoom(topLeftPoint, room);

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
			var chestRoomCounter = 0;
			var secretRoomCounter = 0;

			// Add in default rooms
			// -1 at the end because will need to add in exit too
			var roomsLeft = maximumRooms - chestRoomCount - secretRoomCount - 1;
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
					if (map[x, y].type == TileTypes.Door)
					{
						switch (map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && map[x, y - 1].type == TileTypes.SecretPath)
								{
									map[x, y].type = TileTypes.DestroyableWall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.South:
								if (y == rows - 1 || map[x, y + 1].type == TileTypes.SecretPath)
								{
									map[x, y].type = TileTypes.DestroyableWall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.East:
								if (x == columns - 1 || map[x + 1, y].type == TileTypes.SecretPath)
								{
									map[x, y].type = TileTypes.DestroyableWall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.West:
								if (x == 0 || map[x - 1, y].type == TileTypes.SecretPath)
								{
									map[x, y].type = TileTypes.DestroyableWall;
									map[x, y].rotation = Rotations.None;
								}
								break;
						}
					}
					else if (map[x, y].type == TileTypes.SecretDoor)
					{
						map[x, y].type = TileTypes.SecretPath;
						map[x, y].rotation = Rotations.None;
					}
		}
		public void BlockDoors()
		{
			// Checks for doors that are not attached to paths
			// Replaces those doors with walls instead
			for (var x = 0; x < columns; x++)
				for (var y = 0; y < rows; y++)
					if (map[x, y].type == TileTypes.Door)
					{
						switch (map[x, y].rotation)
						{
							case Rotations.North:
								if (y != 0 && map[x, y - 1].type != TileTypes.Path)
								{
									map[x, y].type = TileTypes.Wall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.South:
								if (y == rows - 1 || map[x, y + 1].type != TileTypes.Path)
								{
									map[x, y].type = TileTypes.Wall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.East:
								if (x == columns - 1 || map[x + 1, y].type != TileTypes.Path)
								{
									map[x, y].type = TileTypes.Wall;
									map[x, y].rotation = Rotations.None;
								}
								break;
							case Rotations.West:
								if (x == 0 || map[x - 1, y].type != TileTypes.Path)
								{
									map[x, y].type = TileTypes.Wall;
									map[x, y].rotation = Rotations.None;
								}
								break;
						}
					}
		}
		public void Generate()
		{
			while (true)
			{
				InstantiateMap();
				Console.WriteLine("Instantiated map");

				if (AddRooms())
				{
					Console.WriteLine("Added rooms");

					HideSecretRooms();
					Console.WriteLine("Hidden secret rooms");

					BlockDoors();
					Console.WriteLine("Removed oblivion doors");

					break;
				}
			}
		}
	}
}