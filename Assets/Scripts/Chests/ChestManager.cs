using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
	public GameObject[] chests;
	void Start()
	{
		var dungeonGenerator = transform.parent.gameObject.GetComponent<DungeonManager>().dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Chest)
				{
					var selectedChest = chests[Random.Range(0, chests.Length)];
					var newChest = Instantiate(selectedChest, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
					newChest.transform.SetParent(transform);

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
