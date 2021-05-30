using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
	public Sprite openedState;
	public GameObject topEdgeCollider;
	public GameObject bottomEdgeCollider;
	public bool playerEntered = false;
	private SpriteRenderer spriteRenderer;
	private EdgeCollider2D _topEdgeCollider;
	private EdgeCollider2D _bottomEdgeCollider;
	private NavMeshObstacle navMeshCollider;
	private DungeonManager dungeonManager;
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		_topEdgeCollider = topEdgeCollider.GetComponent<EdgeCollider2D>();
		_bottomEdgeCollider = bottomEdgeCollider.GetComponent<EdgeCollider2D>();
		navMeshCollider = gameObject.GetComponent<NavMeshObstacle>();
		dungeonManager = transform.parent.parent.gameObject.GetComponent<DungeonManager>();
	}
	void OnPlayerEnter(int direction)
	{
		playerEntered = true;

		dungeonManager.UpdateDarkness(transform.position);

		Destroy(_topEdgeCollider);
		Destroy(_bottomEdgeCollider);
		Destroy(navMeshCollider);

		// 0 = top
		// 1 = bottom
		// Not gonna lie, my brain isn't big enough to handle all these rotations
		// So I literally just placed Debug.Logs and tweaked the Vector3Int values
		// until everything worked
		// Previous code is commented out below
		// switch (transform.rotation.eulerAngles.z)
		// {
		// 	case 0:
		// 		// Already facing North
		// 		break;
		// 	case 270:
		// 		// Already facing East
		// 		// Top becomes right
		// 		// Bottom becomes left
		// 		if (direction == 0)
		// 			transform.Rotate(new Vector3Int(0, 0, 180));
		// 		break;
		// 	case 180:
		// 		// Already facing South
		// 		// Top becomes bottom
		// 		// Bottom becomes top
		// 		if (direction == 0)
		// 			transform.Rotate(new Vector3Int(0, 0, 180));
		// 		break;
		// 	case 90:
		// 		// Already facing West
		// 		// Top becomes left
		// 		// Bottom becomes right
		// 		if (direction == 0)
		// 			transform.Rotate(new Vector3Int(0, 0, 180));
		// 		break;
		// }

		// Turns out I just needed this lmao
		if (direction == 0)
			transform.Rotate(new Vector3Int(0, 0, 180));

		spriteRenderer.sprite = openedState;
	}
}
