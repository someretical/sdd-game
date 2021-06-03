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
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		_topEdgeCollider = topEdgeCollider.GetComponent<EdgeCollider2D>();
		_bottomEdgeCollider = bottomEdgeCollider.GetComponent<EdgeCollider2D>();
		navMeshCollider = GetComponent<NavMeshObstacle>();
		lockCollider = GetComponent<BoxCollider2D>();
		dungeonManager = transform.parent.parent.GetComponent<DungeonManager>();
	}
	void OnPlayerEnter(int direction)
	{
		opened = true;

		dungeonManager.UpdateDarkness(transform.position);

		_topEdgeCollider.enabled = false;
		_bottomEdgeCollider.enabled = false;
		navMeshCollider.enabled = false;

		// Debug.Log(direction);

		if (direction == 0)
			transform.Rotate(new Vector3Int(0, 0, 180));

		spriteRenderer.sprite = openedState;

		// There is a whole lot of logic that needs to be done here
		// Check if the player is coming from a path or not
		// If they are, push them along in the right direction into the room
		// (disable them moving)
		// Then lock the doors and begin spawning enemies
		if (!dungeonManager.completedRoomIDs.Contains(roomID))
		{
			dungeonManager.LockRoom(roomID);
		}
	}
	public void Lock()
	{
		previousSprite = spriteRenderer.sprite;
		lockCollider.enabled = true;
		navMeshCollider.enabled = true;
		spriteRenderer.sprite = lockedState;
	}
	public void Unlock()
	{
		lockCollider.enabled = false;
		spriteRenderer.sprite = previousSprite;

		if (opened)
			navMeshCollider.enabled = false;
	}
}
