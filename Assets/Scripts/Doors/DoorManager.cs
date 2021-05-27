using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneratorNamespace;

public class DoorManager : MonoBehaviour
{
	public GameObject door;
	public void Start()
	{
		var dungeonGenerator = transform.parent.gameObject.GetComponent<DungeonManager>().dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Door)
				{
					var newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
					newDoor.transform.SetParent(transform);

					switch (dungeonGenerator.Map[x, y].rotation)
					{
						case Rotations.East:
							newDoor.transform.Rotate(new Vector3Int(0, 0, 270));
							break;
						case Rotations.South:
							newDoor.transform.Rotate(new Vector3Int(0, 0, 180));
							break;
						case Rotations.West:
							newDoor.transform.Rotate(new Vector3Int(0, 0, 90));
							break;
					}
				}
	}
}
