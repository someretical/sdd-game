using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
	public GameObject door;
	public void Start()
	{
		var dungeonManager = transform.parent.GetComponent<DungeonManager>();
		var dungeonGenerator = dungeonManager.dungeonGenerator;

		for (var x = 0; x < dungeonGenerator.columns; ++x)
			for (var y = 0; y < dungeonGenerator.rows; ++y)
				if (dungeonGenerator.Map[x, y].type == TileTypes.Door)
				{
					// Spawn door and rotate it if necessary
					var newDoor = Instantiate(door, new Vector3(x + 0.5f, dungeonGenerator.rows - 0.5f - y, 0f), Quaternion.identity, transform);
					var doorController = newDoor.GetComponent<DoorController>();
					dungeonManager.doors.Add(doorController);

					doorController.roomID = dungeonGenerator.Map[x, y].roomID;
					// The rotation is from the room to the path
					doorController.rotation = dungeonGenerator.Map[x, y].rotation;

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
