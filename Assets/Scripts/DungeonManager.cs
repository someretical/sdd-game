using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
public class DungeonManager : MonoBehaviour
{
	[Header("Level components")]
	public DoorManager doorManager;
	public ChestManager chestManager;
	public EntranceExitManager entranceExitManager;
	public ItemManager itemManager;
	public TrapManager trapManager;
	public EnemyManager enemyManager;
	[Space]
	[Header("Dungeon generation values")]
	public int mapWidth = 150;
	public int mapHeight = 150;
	public int maximumRooms = 30;
	public int intersectionRoomProbability = 20;
	public int minimumChestRooms = 2;
	public int maximumChestRooms = 2;
	public int minimumSecretRooms = 2;
	public int maximumSecretRooms = 2;
	public int minimumShopRooms = 1;
	public int maximumShopRooms = 1;
	public int minimumSinglePathLength = 1;
	public int maximumSinglePathLength = 1;
	public int minimumMultiPathSegmentLength = 3;
	public int maximumMultiPathSegmentLength = 5;
	public int minimumMultiPathTotal = 8;
	public int maximumMultiPathTotal = 10;
	public int minimumPathTurns = 1;
	public int maximumPathTurns = 2;
	public int pathTurnProbability = 30;
	public int maximumAttempts = 100;
	[Space]
	[Header("Tilemaps")]
	public Tilemap groundTilemap;
	public Tilemap wallsTilemap;
	public Tilemap decorationsTilemap;
	public Tilemap darknessTilemap;
	public Tilemap minimapTilemap;
	[Space]
	[Header("Tile assets")]
	public TileBase darknessTile;
	public TileBase pillarTile;
	public TileBase[] minimapTiles;
	public TileBase[] groundTiles;
	public TileBase[] pathTiles;
	public TileBase[] northWallTiles;
	public TileBase[] eastWallTiles;
	public TileBase[] southWallTiles;
	public TileBase[] westWallTiles;
	public TileBase[] innerCornerTiles;
	public TileBase[] outerCornerTiles;
	[HideInInspector]
	public DungeonGenerator dungeonGenerator;
	[HideInInspector]
	public NavMeshSurface2d navMesh;
	[HideInInspector]
	public GameObject bulletManager;
	void Awake()
	{
		Instantiate(doorManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(chestManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(entranceExitManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(itemManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(trapManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(enemyManager, Vector3.zero, Quaternion.identity, transform);
		bulletManager = new GameObject("BulletManager");
		bulletManager.transform.parent = transform;
	}
	void Start()
	{
		GenerateDungeon();

		PlacePlayer();
		PlaceGroundTiles();
		PlaceWallTiles();
		PlaceDestroyableWalls();
		PlaceDecorations();
		PlaceDarkness();
		BuildNavMesh();

		// Reveal original room
		UpdateDarkness(new Vector3(mapWidth / 2 + 4, mapHeight / 2 - 3, 0f));
	}
	public void GenerateDungeon()
	{
		var gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();

		// Bandage scaling code lmao
		mapWidth += (int)Math.Ceiling((gameManager.levelCounter - 1) * mapWidth * gameManager.levelSizeIncreasePercentage);
		mapHeight += (int)Math.Ceiling((gameManager.levelCounter - 1) * mapHeight * gameManager.levelSizeIncreasePercentage);
		maximumRooms += (int)Math.Ceiling((gameManager.levelCounter - 1) * maximumRooms * gameManager.maxRoomIncreasePercentage);

		dungeonGenerator = new DungeonGenerator(
			gameManager.roomManager,
			mapWidth,
			mapHeight,
			maximumRooms,
			intersectionRoomProbability,
			minimumChestRooms,
			maximumChestRooms,
			minimumSecretRooms,
			maximumSecretRooms,
			minimumShopRooms,
			maximumShopRooms,
			minimumSinglePathLength,
			maximumSinglePathLength,
			minimumMultiPathSegmentLength,
			maximumMultiPathSegmentLength,
			minimumMultiPathTotal,
			maximumMultiPathTotal,
			minimumPathTurns,
			maximumPathTurns,
			pathTurnProbability,
			maximumAttempts
		);

		dungeonGenerator.Generate();
	}
	public void PlacePlayer()
	{
		for (var x = 0; x < mapWidth; ++x)
			for (var y = 0; y < mapHeight; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Entrance)
					transform.parent.GetChild(0).position = new Vector3(x + 0.5f, mapHeight - 0.5f - y, 0f);
	}
	public bool CheckIfGround(int x, int y)
	{
		if (x < 1 || x > mapWidth - 1 || y < 1 || y > mapHeight - 1)
			return false;

		switch (dungeonGenerator.Map[x, y].type)
		{
			case TileTypes.Any:
			// FALL THROUGH
			case TileTypes.Void:
			// FALL THROUGH
			case TileTypes.Pillar:
			// FALL THROUGH
			case TileTypes.DestroyableWall:
			// FALL THROUGH
			case TileTypes.Wall:
			// FALL THROUGH
			case TileTypes.PathWall:
			// FALL THROUGH
			case TileTypes.SecretWall:
			// FALL THROUGH
			case TileTypes.SecretPathWall:
				return false;
		}

		return true;
	}
	public bool CheckIfWall(int x, int y)
	{
		if (x < 1 || x > mapWidth - 1 || y < 1 || y > mapHeight - 1)
			return false;

		switch (dungeonGenerator.Map[x, y].type)
		{
			case TileTypes.Pillar:
			// FALL THROUGH
			case TileTypes.DestroyableWall:
			// FALL THROUGH
			case TileTypes.Wall:
			// FALL THROUGH
			case TileTypes.PathWall:
			// FALL THROUGH
			case TileTypes.SecretWall:
			// FALL THROUGH
			case TileTypes.SecretPathWall:
				return true;
		}

		return false;
	}
	public void PlaceGroundTiles()
	{
		for (var x = 0; x < mapWidth; ++x)
			for (var y = 0; y < mapHeight; ++y)
				switch (dungeonGenerator.Map[x, y].type)
				{
					case TileTypes.Any:
					// FALL THROUGH
					case TileTypes.Void:
					// FALL THROUGH
					case TileTypes.Pillar:
					// FALL THROUGH
					case TileTypes.DestroyableWall:
					// FALL THROUGH
					case TileTypes.Wall:
					// FALL THROUGH
					case TileTypes.PathWall:
					// FALL THROUGH
					case TileTypes.SecretWall:
					// FALL THROUGH
					case TileTypes.SecretPathWall:
						// This is a product of my laziness
						break;
					case TileTypes.Path:
					// FALL THROUGH
					case TileTypes.SecretPath:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(pathTiles));
						break;
					case TileTypes.Door:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), groundTiles[0]);
						break;
					default:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(groundTiles));
						break;
				}
	}
	public void PlaceWallTiles()
	{
		for (var x = 0; x < mapWidth; ++x)
			for (var y = 0; y < mapHeight; ++y)
				if (CheckIfWall(x, y))
				{
					if (dungeonGenerator.Map[x, y].type == TileTypes.Pillar)
					{
						wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), pillarTile);
						continue;
					}

					if (CheckIfWall(x - 1, y) && CheckIfWall(x + 1, y))
						if (CheckIfGround(x, y + 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(northWallTiles));
						else if (CheckIfGround(x, y - 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(southWallTiles));

					if (CheckIfWall(x, y - 1) && CheckIfWall(x, y + 1))
						if (CheckIfGround(x - 1, y))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(eastWallTiles));
						else if (CheckIfGround(x + 1, y))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(westWallTiles));

					// Check if wall above + left
					if (CheckIfWall(x, y - 1) && CheckIfWall(x - 1, y))
						// Check if there is ground below right (there are two types of walls that fulfill these conditions)
						// Hence this check is needed to make sure the right one is placed
						if (CheckIfGround(x + 1, y + 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), innerCornerTiles[0]);
						else
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), outerCornerTiles[0]);

					// Check if wall above + right
					if (CheckIfWall(x, y - 1) && CheckIfWall(x + 1, y))
						// Check if there is ground below left (there are two types of walls that fulfill these conditions)
						// Hence this check is needed to make sure the right one is placed
						if (CheckIfGround(x - 1, y + 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), innerCornerTiles[1]);
						else
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), outerCornerTiles[1]);

					// Check if wall below + left
					if (CheckIfWall(x, y + 1) && CheckIfWall(x - 1, y))
						// Check if there is ground above right (there are two types of walls that fulfill these conditions)
						// Hence this check is needed to make sure the right one is placed
						if (CheckIfGround(x + 1, y - 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), innerCornerTiles[2]);
						else
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), outerCornerTiles[2]);

					// Check if wall below + right
					if (CheckIfWall(x, y + 1) && CheckIfWall(x + 1, y))
						// Check if there is ground above left (there are two types of walls that fulfill these conditions)
						// Hence this check is needed to make sure the right one is placed
						if (CheckIfGround(x - 1, y - 1))
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), innerCornerTiles[3]);
						else
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), outerCornerTiles[3]);
				}
	}
	public void PlaceDestroyableWalls()
	{
		for (var x = 0; x < mapWidth; ++x)
			for (var y = 0; y < mapHeight; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.DestroyableWall)
					switch (dungeonGenerator.Map[x, y].rotation)
					{
						case Rotations.North:
							// Set tiles east and west
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(northWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x + 1, mapHeight - 1 - y, 0), Util.GetArrayRandom(northWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x - 1, mapHeight - 1 - y, 0), Util.GetArrayRandom(northWallTiles));
							break;
						case Rotations.East:
							// Set tiles north and south
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(eastWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - y, 0), Util.GetArrayRandom(eastWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 2 - y, 0), Util.GetArrayRandom(eastWallTiles));
							break;
						case Rotations.South:
							// Set tiles east and west
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(southWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x + 1, mapHeight - 1 - y, 0), Util.GetArrayRandom(southWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x - 1, mapHeight - 1 - y, 0), Util.GetArrayRandom(southWallTiles));
							break;
						case Rotations.West:
							// Set tiles north and south
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(westWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - y, 0), Util.GetArrayRandom(westWallTiles));
							wallsTilemap.SetTile(new Vector3Int(x, mapHeight - 2 - y, 0), Util.GetArrayRandom(westWallTiles));
							break;
					}
	}
	public void PlaceDecorations()
	{
		// Might get around to doing this if I have enough time left
		// Probably not going to happen
	}
	public void PlaceMinimapTile(Vector2Int internalCoords, Vector3Int visibleCoords)
	{
		switch (dungeonGenerator.Map[internalCoords.x, internalCoords.y].type)
		{
			case TileTypes.DestroyableWall:
			// FALL THROUGH
			case TileTypes.PathWall:
			// FALL THROUGH
			case TileTypes.Pillar:
			// FALL THROUGH
			case TileTypes.SecretPathWall:
			// FALL THROUGH
			case TileTypes.SecretWall:
			// FALL THROUGH
			case TileTypes.Wall:
				minimapTilemap.SetTile(visibleCoords, minimapTiles[1]);
				break;
			case TileTypes.Door:
			// FALL THROUGH
			case TileTypes.Goo:
			// FALL THROUGH
			case TileTypes.Ground:
			// FALL THROUGH
			case TileTypes.Path:
			// FALL THROUGH
			case TileTypes.Pit:
			// FALL THROUGH
			case TileTypes.SecretDoor:
			// FALL THROUGH
			case TileTypes.SecretGround:
			// FALL THROUGH
			case TileTypes.SecretPath:
			// FALL THROUGH
			case TileTypes.ShopItem:
				minimapTilemap.SetTile(visibleCoords, minimapTiles[0]);
				break;
			case TileTypes.Entrance:
				minimapTilemap.SetTile(visibleCoords, minimapTiles[2]);
				break;
			case TileTypes.Exit:
				minimapTilemap.SetTile(visibleCoords, minimapTiles[3]);
				break;
			case TileTypes.Chest:
			// FALL THROUGH
			case TileTypes.SecretChest:
			// FALL THROUGH
			case TileTypes.Shop:
				minimapTilemap.SetTile(visibleCoords, minimapTiles[4]);
				break;
		}
	}
	public void PlaceDarkness()
	{
		for (var i = 0; i < dungeonGenerator.PathPoints.Count; ++i)
			for (var j = 0; j < dungeonGenerator.PathPoints[i].Count; ++j)
			{
				var position = new Vector3Int(dungeonGenerator.PathPoints[i][j].x, mapHeight - 1 - dungeonGenerator.PathPoints[i][j].y, 0);
				darknessTilemap.SetTile(position, darknessTile);
			}

		for (var i = 0; i < dungeonGenerator.RoomPoints.Count; ++i)
			for (var j = 0; j < dungeonGenerator.RoomPoints[i].Count; ++j)
			{
				var position = new Vector3Int(dungeonGenerator.RoomPoints[i][j].x, mapHeight - 1 - dungeonGenerator.RoomPoints[i][j].y, 0);
				darknessTilemap.SetTile(position, darknessTile);
			}
	}
	public void UpdateDarkness(Vector3 position, bool revealRoom = true)
	{
		var rounded = Util.RoundPosition(position);

		var roomID = dungeonGenerator.Map[rounded.x, mapHeight - 1 - rounded.y].roomID;
		if (roomID != -1 && revealRoom)
			for (var i = 0; i < dungeonGenerator.RoomPoints[roomID].Count; ++i)
			{
				var _position = new Vector3Int(dungeonGenerator.RoomPoints[roomID][i].x, mapHeight - 1 - dungeonGenerator.RoomPoints[roomID][i].y, 0);
				darknessTilemap.SetTile(_position, null);
				PlaceMinimapTile(dungeonGenerator.RoomPoints[roomID][i], _position);
			}

		var pathID = dungeonGenerator.Map[rounded.x, mapHeight - 1 - rounded.y].pathID;
		if (pathID != -1)
			for (var i = 0; i < dungeonGenerator.PathPoints[pathID].Count; ++i)
			{
				var _position = new Vector3Int(dungeonGenerator.PathPoints[pathID][i].x, mapHeight - 1 - dungeonGenerator.PathPoints[pathID][i].y, 0);
				darknessTilemap.SetTile(_position, null);
				PlaceMinimapTile(dungeonGenerator.PathPoints[pathID][i], _position);
			}
	}
	public void ProcessBlank(Vector3 position)
	{
		var rounded = Util.RoundPosition(position);

		var roomID = dungeonGenerator.Map[rounded.x, mapHeight - 1 - rounded.y].roomID;
		if (roomID == -1)
			return;

		var secretRoomExists = false;
		for (int i = 0; i < dungeonGenerator.RoomPoints[roomID].Count; ++i)
		{
			var p = dungeonGenerator.RoomPoints[roomID][i];

			if (dungeonGenerator.Map[p.x, p.y].type == TileTypes.DestroyableWall)
			{
				var coords = new Vector3Int(p.x, mapHeight - 1 - p.y, 0);
				if (wallsTilemap.GetTile(coords) == null)
					return;

				wallsTilemap.SetTile(coords, null);
				groundTilemap.SetTile(coords, Util.GetArrayRandom(pathTiles));

				switch (dungeonGenerator.Map[p.x, p.y].rotation)
				{
					case Rotations.North:
						wallsTilemap.SetTile(new Vector3Int(p.x + 1, mapHeight - 1 - p.y, 0), innerCornerTiles[1]);
						wallsTilemap.SetTile(new Vector3Int(p.x - 1, mapHeight - 1 - p.y, 0), innerCornerTiles[0]);
						break;
					case Rotations.East:
						wallsTilemap.SetTile(new Vector3Int(p.x, mapHeight - 2 - p.y, 0), innerCornerTiles[3]);
						wallsTilemap.SetTile(new Vector3Int(p.x, mapHeight - p.y, 0), innerCornerTiles[1]);
						break;
					case Rotations.South:
						wallsTilemap.SetTile(new Vector3Int(p.x + 1, mapHeight - 1 - p.y, 0), innerCornerTiles[3]);
						wallsTilemap.SetTile(new Vector3Int(p.x - 1, mapHeight - 1 - p.y, 0), innerCornerTiles[2]);
						break;
					case Rotations.West:
						wallsTilemap.SetTile(new Vector3Int(p.x, mapHeight - 2 - p.y, 0), innerCornerTiles[2]);
						wallsTilemap.SetTile(new Vector3Int(p.x, mapHeight - p.y, 0), innerCornerTiles[0]);
						break;
				}

				UpdateDarkness(new Vector3(p.x, mapHeight - 1 - p.y, 0f), false);
				minimapTilemap.SetTile(coords, minimapTiles[0]);

				secretRoomExists = true;
			}
		}

		// Compute as few times as possible
		if (secretRoomExists)
			navMesh.UpdateNavMesh(navMesh.navMeshData);
	}
	public void BuildNavMesh()
	{
		navMesh = transform.parent.GetChild(3).GetComponent<NavMeshSurface2d>();
		navMesh.BuildNavMesh();
	}
}
