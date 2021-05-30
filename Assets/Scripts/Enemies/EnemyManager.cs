using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public GameObject[] enemies;
	private DungeonManager dungeonManager;
	private DungeonGenerator dungeonGenerator;
	void Start()
	{
		dungeonManager = transform.parent.gameObject.GetComponent<DungeonManager>();
		dungeonGenerator = dungeonManager.dungeonGenerator;
	}
	public GameObject GetRandomEnemy()
	{
		var chances = new List<int>();

		for (var i = 0; i < enemies.Length; ++i)
		{
			var weight = enemies[i].GetComponent<EnemyController>().weight;

			for (var l = 0; l < weight; ++l)
				chances.Add(i);
		}

		return enemies[Util.GetListRandom(chances)];
	}
	public void SpawnEnemy(Vector3 position)
	{
		var rounded = Util.RoundPosition(position);

		var roomID = dungeonGenerator.Map[rounded.x, dungeonManager.mapHeight - 1 - rounded.y].roomID;
		if (roomID == -1)
			return;

		var suitableTiles = new List<Vector2Int>();

		for (int i = 0; i < dungeonGenerator.RoomPoints[roomID].Count; ++i)
		{
			var p = dungeonGenerator.RoomPoints[roomID][i];
			var realTilePosition = new Vector3(p.x + 0.5f, dungeonManager.mapHeight - 0.5f - p.y, 0f);

			if (
				dungeonGenerator.Map[p.x, p.y].type == TileTypes.Ground &&
				Vector3.Distance(realTilePosition, position) > 2
			)
				suitableTiles.Add(p);
		}

		var tile = Util.GetListRandom(suitableTiles);
		Instantiate(
			GetRandomEnemy(),
			new Vector3(
				 tile.x + 0.5f,
				 dungeonManager.mapHeight - 0.5f - tile.y,
				 0f
				),
			Quaternion.identity,
			transform
		);
	}
}
