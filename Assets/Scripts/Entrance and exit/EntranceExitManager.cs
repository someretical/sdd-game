using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class EntranceExitManager : MonoBehaviour
{
	public GameObject entrance;
	public GameObject exit;
	void Start()
	{
		var dungeonGenerator = transform.parent.GetComponent<DungeonManager>().dungeonGenerator;

		// Place entrance and exit nodes
		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Entrance)
					Instantiate(entrance, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity, transform);
				else if (dungeonGenerator.Map[x, y].type == TileTypes.Exit)
					Instantiate(exit, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity, transform);
	}
}
