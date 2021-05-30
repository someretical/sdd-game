using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public GameObject[] enemyLootTable;
	public GameObject[] roomClearLootTable;
	public GameObject[] shopLootTable;
	private DungeonManager dungeonManager;
	private DungeonGenerator dungeonGenerator;
	private PlayerController player;
	private GameManager gameManager;
	void Start()
	{
		dungeonManager = transform.parent.gameObject.GetComponent<DungeonManager>();
		dungeonGenerator = dungeonManager.dungeonGenerator;
		player = transform.parent.parent.GetChild(0).GetComponent<PlayerController>();
		gameManager = transform.parent.parent.parent.gameObject.GetComponent<GameManager>();

		SpawnShopItems();
	}
	public GameObject GetRandomItem(LootTypes type)
	{
		var chances = new List<int>();
		var lootTable = type == LootTypes.Enemy
			? enemyLootTable
			: type == LootTypes.RoomClear
			? roomClearLootTable
			: shopLootTable;

		for (var i = 0; i < lootTable.Length; ++i)
		{
			// Quite possibly one of memiest lines of code in this entire project
			dynamic correctClass;
			if ((correctClass = lootTable[i].GetComponent<ItemTrigger>()) == null)
				if ((correctClass = lootTable[i].GetComponent<ItemCollider>()) == null)
					correctClass = lootTable[i].GetComponent<BuyableItem>();

			if (correctClass != null)
			{
				var weight = correctClass is ItemCollider
					? correctClass.roomClearWeight
					: correctClass.weight;

				for (var l = 0; l < weight; ++l)
					chances.Add(i);
			}
		}

		return lootTable[Util.GetListRandom(chances)];
	}
	public void SpawnShopItems()
	{
		for (var x = 0; x < dungeonManager.mapWidth; ++x)
			for (var y = 0; y < dungeonManager.mapHeight; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.ShopItem)
					Instantiate(
						GetRandomItem(LootTypes.Shop),
						new Vector3(
							x + 0.5f,
							dungeonManager.mapHeight - 0.5f - y,
							0f
						),
						Quaternion.identity,
						transform
					);
	}
	public Vector3 GetRandomOffset(Vector3 position)
	{
		var x = UnityEngine.Random.Range(-0.4f, 0.4f);
		var y = UnityEngine.Random.Range(-0.4f, 0.4f);

		return position + new Vector3(x, y, 0f);
	}
	public void SpawnEnemyDrops(Vector3 position, int baseReward)
	{
		var scaled = baseReward + (int)Math.Ceiling((gameManager.levelCounter - 1) * baseReward * gameManager.itemPriceIncreasePercentage);
		// Visual studio code says int casts are redundant but they aren't lmao
		// Thanks omnisharp
		var fifties = (int)scaled / 50;
		var twenties = (int)(scaled - (50 * fifties)) / 20;
		var tens = (int)(scaled - (50 * fifties) - (20 * twenties)) / 10;
		var fives = (int)(scaled - (50 * fifties) - (20 * twenties) - (10 * tens)) / 5;
		var twos = (int)(scaled - (50 * fifties) - (20 * twenties) - (10 * tens) - (5 * fives)) / 2;
		var ones = (int)(scaled - (50 * fifties) - (20 * twenties) - (10 * tens) - (5 * fives) - (2 * twos));

		for (int i = 0; i < fifties; ++i)
			Instantiate(enemyLootTable[5], GetRandomOffset(position), Quaternion.identity, transform);
		for (int i = 0; i < twenties; ++i)
			Instantiate(enemyLootTable[4], GetRandomOffset(position), Quaternion.identity, transform);
		for (int i = 0; i < tens; ++i)
			Instantiate(enemyLootTable[3], GetRandomOffset(position), Quaternion.identity, transform);
		for (int i = 0; i < fives; ++i)
			Instantiate(enemyLootTable[2], GetRandomOffset(position), Quaternion.identity, transform);
		for (int i = 0; i < twos; ++i)
			Instantiate(enemyLootTable[1], GetRandomOffset(position), Quaternion.identity, transform);
		for (int i = 0; i < ones; ++i)
			Instantiate(enemyLootTable[0], GetRandomOffset(position), Quaternion.identity, transform);
	}
	public void SpawnRoomClearReward(Vector3 position)
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
			GetRandomItem(LootTypes.RoomClear),
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