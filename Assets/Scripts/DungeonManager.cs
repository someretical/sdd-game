using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DungeonGeneratorNamespace;
public class DungeonManager : MonoBehaviour
{
	public int mapWidth = 150;
	public int mapHeight = 150;
	public int maximumRooms = 30;
	public int intersectionRoomProbability = 20;
	public int minimumChestRooms = 2;
	public int maximumChestRooms = 2;
	public int minimumSecretRooms = 2;
	public int maximumSecretRooms = 2;
	public int minimumSinglePathLength = 1;
	public int maximumSinglePathLength = 1;
	public int minimumMultiPathSegmentLength = 2;
	public int maximumMultiPathSegmentLength = 5;
	public int minimumMultiPathTotal = 6;
	public int maximumMultiPathTotal = 10;
	public int minimumPathTurns = 1;
	public int maximumPathTurns = 2;
	public int pathTurnProbability = 30;
	public int maximumAttempts = 100;
	public Tilemap groundTilemap;
	public Tilemap wallsTilemap;
	public Tilemap doorsTilemap;
	public Tilemap tileEntitiesTilemap;
	public Tilemap decorationsTilemap;
	public TileBase entranceTile;
	public TileBase exitTile;
	public TileBase[] groundTiles;
	public TileBase[] pathTiles;
	public TileBase[] northWallTiles;
	public TileBase[] eastWallTiles;
	public TileBase[] southWallTiles;
	public TileBase[] westWallTiles;
	public TileBase[] innerCornerTiles;
	public TileBase[] outerCornerTiles;
	private DungeonGenerator dungeonGenerator;
	public bool CheckIfGround(int x, int y)
	{
		if (x < 1 || x > mapWidth - 1 || y < 1 || y > mapHeight - 1)
			return false;

		switch (dungeonGenerator.Map[x, y].type)
		{
			case TileTypes.Ground:
			case TileTypes.SecretGround:
			case TileTypes.Path:
			case TileTypes.SecretPath:
				return true;
		}

		return false;
	}
	public bool CheckIfWall(int x, int y)
	{
		if (x < 1 || x > mapWidth - 1 || y < 1 || y > mapHeight - 1)
			return false;

		switch (dungeonGenerator.Map[x, y].type)
		{
			case TileTypes.Wall:
			case TileTypes.PathWall:
			case TileTypes.SecretWall:
			case TileTypes.SecretPathWall:
				return true;
		}

		return false;
	}
	public void Init(RoomManager roomManager)
	{
		dungeonGenerator = new DungeonGenerator(
			roomManager,
			mapWidth,
			mapHeight,
			maximumRooms,
			intersectionRoomProbability,
			minimumChestRooms,
			maximumChestRooms,
			minimumSecretRooms,
			maximumSecretRooms,
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

		PlaceGroundTiles();

		PlaceWallTiles();

		PlaceDoorTiles();

		PlaceTileEntities();

		PlaceDecorations();
	}
	public void PlaceGroundTiles()
	{
		for (int x = 0; x < mapWidth; ++x)
			for (int y = 0; y < mapHeight; ++y)
				switch (dungeonGenerator.Map[x, y].type)
				{
					case TileTypes.Ground:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(groundTiles));
						break;
					case TileTypes.SecretGround:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(groundTiles));
						break;
					case TileTypes.Path:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(pathTiles));
						break;
					case TileTypes.SecretPath:
						groundTilemap.SetTile(new Vector3Int(x, mapHeight - 1 - y, 0), Util.GetArrayRandom(pathTiles));
						break;
				}
	}
	public void PlaceWallTiles()
	{
		for (int x = 0; x < mapWidth; ++x)
			for (int y = 0; y < mapHeight; ++y)
				if (CheckIfWall(x, y))
				{
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
	public void PlaceDoorTiles()
	{

	}
	public void PlaceTileEntities()
	{

	}
	public void PlaceDecorations()
	{

	}
}
