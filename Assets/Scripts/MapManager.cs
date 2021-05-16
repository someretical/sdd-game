using UnityEngine;
using DungeonGenerator;

public class MapManager : MonoBehaviour
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
	public int maximumSinglePathLength = 3;
	public int minimumMultiPathSegmentLength = 2;
	public int maximumMultiPathSegmentLength = 5;
	public int minimumMultiPathTotal = 6;
	public int maximumMultiPathTotal = 10;
	public int minimumPathTurns = 1;
	public int maximumPathTurns = 2;
	public int pathTurnProbability = 30;
	public int maximumAttempts = 100;
	public GameObject entranceTile;
	public GameObject exitTile;
	public GameObject northDoor;
	public GameObject eastDoor;
	public GameObject southDoor;
	public GameObject westDoor;
	public GameObject[] wallTiles;
	public GameObject[] secretWallTiles;
	public GameObject[] groundTiles;
	public GameObject[] secretGroundTiles;
	public GameObject[] pathTiles;
	public GameObject[] secretPathTiles;
	public GameObject[] destroyableWallTiles;
	private Transform tileHolder;
	public void SetupMap(int level)
	{
		Rooms.RotateRoomPresets();

		MapGenerator mapGenerator = new MapGenerator(
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

		mapGenerator.Generate();

		for (var y = 0; y < mapGenerator.map.GetLength(0); ++y)
			for (var x = 0; x < mapGenerator.map.GetLength(1); ++x)
			{
				GameObject toInstantiate = entranceTile;

				switch (mapGenerator.map[y, x].type)
				{
					case TileTypes.Void:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.Any:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.Pillar:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.OuterWall:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.Wall:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.PathWall:
						toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
						break;
					case TileTypes.Ground:
						toInstantiate = groundTiles[Random.Range(0, groundTiles.Length)];
						break;
					case TileTypes.Door:
						if (mapGenerator.map[y, x].rotation == Rotations.North)
							toInstantiate = northDoor;

						if (mapGenerator.map[y, x].rotation == Rotations.East)
							toInstantiate = eastDoor;

						if (mapGenerator.map[y, x].rotation == Rotations.South)
							toInstantiate = southDoor;

						if (mapGenerator.map[y, x].rotation == Rotations.West)
							toInstantiate = westDoor;
						break;
					case TileTypes.Path:
						toInstantiate = pathTiles[Random.Range(0, pathTiles.Length)];
						break;
					case TileTypes.Pit:
						toInstantiate = groundTiles[Random.Range(0, groundTiles.Length)];
						break;
					case TileTypes.Goo:
						toInstantiate = groundTiles[Random.Range(0, groundTiles.Length)];
						break;
					case TileTypes.Chest:
						toInstantiate = groundTiles[Random.Range(0, groundTiles.Length)];
						break;
					case TileTypes.DestroyableWall:
						toInstantiate = destroyableWallTiles[Random.Range(0, destroyableWallTiles.Length)];
						break;
					case TileTypes.SecretPath:
						toInstantiate = secretPathTiles[Random.Range(0, secretPathTiles.Length)];
						break;
					case TileTypes.SecretGround:
						toInstantiate = secretGroundTiles[Random.Range(0, secretGroundTiles.Length)];
						break;
					case TileTypes.SecretWall:
						toInstantiate = secretWallTiles[Random.Range(0, secretWallTiles.Length)];
						break;
					case TileTypes.SecretPathWall:
						toInstantiate = secretWallTiles[Random.Range(0, secretWallTiles.Length)];
						break;
				}

				GameObject instance = Instantiate(toInstantiate, new Vector3(x, mapHeight - y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(tileHolder);
			}
	}
}
