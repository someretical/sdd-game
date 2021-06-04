using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
	[Header("Sprites")]
	public Sprite defaultState;
	public Sprite lockedState;
	public Sprite openedState;
	[Space]
	[Header("Colliders")]
	public GameObject topEdgeCollider;
	public GameObject bottomEdgeCollider;
	[HideInInspector]
	public bool opened = false;
	[HideInInspector]
	public int roomID;
	[HideInInspector]
	public Rotations rotation;
	private SpriteRenderer spriteRenderer;
	private EdgeCollider2D _topEdgeCollider;
	private EdgeCollider2D _bottomEdgeCollider;
	private NavMeshObstacle navMeshCollider;
	private BoxCollider2D lockCollider;
	private DungeonManager dungeonManager;
	private Sprite previousSprite;
	private PlayerController playerController;
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		_topEdgeCollider = topEdgeCollider.GetComponent<EdgeCollider2D>();
		_bottomEdgeCollider = bottomEdgeCollider.GetComponent<EdgeCollider2D>();
		navMeshCollider = GetComponent<NavMeshObstacle>();
		lockCollider = GetComponent<BoxCollider2D>();
		dungeonManager = transform.parent.parent.GetComponent<DungeonManager>();
		playerController = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();
	}
	IEnumerator LockRoom()
	{
		playerController.canMove = false;

		switch (rotation)
		{
			case Rotations.North:
				playerController.rb2d.velocity = playerController.GetScaledSpeed() * Vector3.down;
				break;
			case Rotations.East:
				playerController.rb2d.velocity = playerController.GetScaledSpeed() * Vector3.left;
				break;
			case Rotations.South:
				playerController.rb2d.velocity = playerController.GetScaledSpeed() * Vector3.up;
				break;
			case Rotations.West:
				playerController.rb2d.velocity = playerController.GetScaledSpeed() * Vector3.right;
				break;
		}

		yield return new WaitForSeconds(0.2f);

		dungeonManager.LockRoom(roomID);

		playerController.canMove = true;
	}
	void OnPlayerEnter(int direction)
	{
		opened = true;

		dungeonManager.UpdateDarkness(transform.position);

		_topEdgeCollider.enabled = false;
		_bottomEdgeCollider.enabled = false;
		navMeshCollider.enabled = false;

		if (direction == 0)
			transform.Rotate(new Vector3Int(0, 0, 180));

		spriteRenderer.sprite = openedState;

		if (
			!dungeonManager.completedRoomIDs.Contains(roomID) &&
			dungeonManager.dungeonGenerator.CombatRoomIDs.Contains(roomID) &&
			direction == 0
		)
			StartCoroutine(LockRoom());
	}
	public void Lock()
	{
		_topEdgeCollider.enabled = false;
		_bottomEdgeCollider.enabled = false;

		lockCollider.enabled = true;
		navMeshCollider.enabled = true;

		previousSprite = spriteRenderer.sprite;
		spriteRenderer.sprite = lockedState;
	}
	public void Unlock()
	{
		lockCollider.enabled = false;
		spriteRenderer.sprite = previousSprite;

		if (opened)
			navMeshCollider.enabled = false;
		else
		{
			_topEdgeCollider.enabled = true;
			_bottomEdgeCollider.enabled = true;
		}
	}
}
