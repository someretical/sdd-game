using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public GameObject[] enemies;
	public GameObject spawnAnimation;
	private DungeonManager dungeonManager;
	private DungeonGenerator dungeonGenerator;
	void Start()
	{
		dungeonManager = transform.parent.GetComponent<DungeonManager>();
		dungeonGenerator = dungeonManager.dungeonGenerator;
	}
	public GameObject GetRandomEnemy()
	{
		var chances = new List<int>();

		// Very quick and dirty method of implementing probability that doesn't care about memory
		for (var i = 0; i < enemies.Length; ++i)
		{
			var weight = enemies[i].GetComponent<EnemyController>().weight;

			for (var l = 0; l < weight; ++l)
				chances.Add(i);
		}

		return enemies[Util.GetListRandom(chances)];
	}
	public void SpawnEnemies(Vector3 position)
	{
		var rounded = Util.RoundPosition(position);

		var roomID = dungeonGenerator.Map[rounded.x, dungeonManager.mapHeight - 1 - rounded.y].roomID;
		if (roomID == -1)
			return;

		var suitableTiles = new List<Vector2Int>();

		for (var i = 0; i < dungeonGenerator.RoomPoints[roomID].Count; ++i)
		{
			var p = dungeonGenerator.RoomPoints[roomID][i];
			var realTilePosition = new Vector3(p.x + 0.5f, dungeonManager.mapHeight - 0.5f - p.y, 0f);

			// Don't want to spawn the enemy too close to the player position
			// passed as Vector3 position into this function
			if (
				dungeonGenerator.Map[p.x, p.y].type == TileTypes.Ground &&
				Vector3.Distance(realTilePosition, position) > 2
			)
				suitableTiles.Add(p);
		}

		// Spawn an enemy for every 8 tiles
		// I have no idea if this is balanced or not
		for (var i = 0; i < dungeonGenerator.RoomPoints[roomID].Count / 16; ++i)
			StartCoroutine(SpawnEnemy(Util.GetListRandom(suitableTiles), roomID));
	}
	IEnumerator SpawnEnemy(Vector2Int tile, int roomID)
	{
		var animation = Instantiate(
			spawnAnimation,
			new Vector3(
				tile.x + 0.5f,
				dungeonManager.mapHeight - 0.5f - tile.y,
				0f
			),
			Quaternion.identity
		);

		yield return new WaitForSeconds(1f);

		Destroy(animation);

		var newEnemy = Instantiate(
			GetRandomEnemy(),
			new Vector3(
				 tile.x + 0.5f,
				 dungeonManager.mapHeight - 0.5f - tile.y,
				 0f
				),
			Quaternion.identity,
			transform
		);
		newEnemy.GetComponent<EnemyController>().boundRoomID = roomID;
	}
}
