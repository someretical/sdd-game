using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
public class ChestManager : MonoBehaviour
{
	public GameObject[] chests;
	void Start()
	{
		var dungeonGenerator = transform.parent.GetComponent<DungeonManager>().dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (
					dungeonGenerator.Map[x, y].type == TileTypes.Chest ||
					dungeonGenerator.Map[x, y].type == TileTypes.SecretChest
				)
				{
					// There is a 20% chance of generating a rare chest vs a common chest
					var num = Random.Range(0, 10);
					var rare =
						num == 0 && dungeonGenerator.Map[x, y].type == TileTypes.Chest ||
						num < 8 && dungeonGenerator.Map[x, y].type == TileTypes.SecretChest;
					var newChest = Instantiate(
						chests[rare ? 0 : 1],
						new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f),
						Quaternion.identity,
						transform
					);

					// TileTypes.SecretChest is NOT equal to TileTypes.chest
					newChest.GetComponent<ChestController>().needsKey = dungeonGenerator.Map[x, y].type == TileTypes.Chest;
					if (rare)
						newChest.GetComponent<ChestController>().rare = true;

					switch (dungeonGenerator.Map[x, y].rotation)
					{
						case Rotations.North:
							newChest.transform.Rotate(new Vector3Int(0, 0, 180));
							break;
						case Rotations.East:
							newChest.transform.Rotate(new Vector3Int(0, 0, 90));
							break;
						case Rotations.West:
							newChest.transform.Rotate(new Vector3Int(0, 0, 270));
							break;
					}
				}
	}
}
