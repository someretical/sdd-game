using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public GameObject[] lootTable;
	private DungeonManager dungeonManager;
	private DungeonGenerator dungeonGenerator;
	void Start()
	{
		dungeonManager = transform.parent.gameObject.GetComponent<DungeonManager>();
		dungeonGenerator = dungeonManager.dungeonGenerator;
	}
	public void SpawnItem(Vector3 position)
	{
		var rounded = Util.RoundPosition(position);

		var roomID = dungeonGenerator.Map[rounded.x, dungeonManager.mapHeight - 1 - rounded.y].roomID;
		if (roomID == -1)
			return;

		var suitableTiles = new List<Vector2Int>();

		for (int i = 0; i < dungeonGenerator.RoomPoints[roomID].Count; ++i)
		{
			var p = dungeonGenerator.RoomPoints[roomID][i];

			if (dungeonGenerator.Map[p.x, p.y].type == TileTypes.Ground)
				suitableTiles.Add(p);
		}

		var tile = Util.GetListRandom(suitableTiles);

		var newDrop = Instantiate(
			Util.GetArrayRandom(lootTable),
			new Vector3(
				 tile.x + 0.5f,
				 dungeonManager.mapHeight - 0.5f - tile.y,
				 0f
				),
			Quaternion.identity
		);
		newDrop.transform.SetParent(transform);
	}
}