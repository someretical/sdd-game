using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public Sprite openedState;
	public GameObject topEdgeCollider;
	public GameObject bottomEdgeCollider;
	public bool playerEntered = false;
	void OnPlayerEnter(int direction)
	{
		playerEntered = true;

		transform.parent.parent.gameObject
			.GetComponent<DungeonManager>()
			.UpdateDarkness(transform.position);

		Destroy(topEdgeCollider.GetComponent<EdgeCollider2D>());
		Destroy(bottomEdgeCollider.GetComponent<EdgeCollider2D>());

		// 0 = top
		// 1 = bottom
		// Not gonna lie, my brain isn't big enough to handle all these rotations
		// So I literally just placed Debug.Logs and tweaked the Vector3Int values
		// until everything worked
		switch (transform.rotation.eulerAngles.z)
		{
			case 0:
				// Already facing North
				if (direction == 0)
					transform.Rotate(new Vector3Int(0, 0, 180));
				break;
			case 270:
				// Already facing East
				// Top becomes right
				// Bottom becomes left
				if (direction == 0)
					transform.Rotate(new Vector3Int(0, 0, 180));
				break;
			case 180:
				// Already facing South
				// Top becomes bottom
				// Bottom becomes top
				if (direction == 0)
					transform.Rotate(new Vector3Int(0, 0, 180));
				break;
			case 90:
				// Already facing West
				// Top becomes left
				// Bottom becomes right
				if (direction == 0)
					transform.Rotate(new Vector3Int(0, 0, 180));
				break;
		}

		gameObject.GetComponent<SpriteRenderer>().sprite = openedState;
	}
}
