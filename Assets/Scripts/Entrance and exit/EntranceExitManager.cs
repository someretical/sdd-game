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
		var dungeonGenerator = transform.parent.gameObject.GetComponent<DungeonManager>().dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Entrance)
				{
					var newEntrance = Instantiate(entrance, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
					newEntrance.transform.SetParent(transform);
				}
				else if (dungeonGenerator.Map[x, y].type == TileTypes.Exit)
				{
					var newExit = Instantiate(exit, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
					newExit.transform.SetParent(transform);
				}
	}
}
