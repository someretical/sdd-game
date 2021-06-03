using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
	public GameObject spikeTrap;
	public void Start()
	{
		var dungeonGenerator = transform.parent.GetComponent<DungeonManager>().dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Pit)
					Instantiate(spikeTrap, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity, transform);
	}
}
