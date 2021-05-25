using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneratorNamespace;

public class DoorManager : MonoBehaviour
{
	public GameObject door;
	public void Init(DungeonGenerator dungeonGenerator)
	{
		for (int x = 0; x < dungeonGenerator.columns; ++x)
			for (int y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Door)
				{
					GameObject newDoor = null;

					switch (dungeonGenerator.Map[x, y].rotation)
					{
						case Rotations.North:
							newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
							break;
						case Rotations.East:
							newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
							newDoor.transform.Rotate(new Vector3Int(0, 0, -90));
							break;
						case Rotations.South:
							newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
							newDoor.transform.Rotate(new Vector3Int(0, 0, 180));
							break;
						case Rotations.West:
							newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity);
							newDoor.transform.Rotate(new Vector3Int(0, 0, 90));
							break;
					}

					if (newDoor != null)
						newDoor.transform.SetParent(transform);
				}
	}
}
