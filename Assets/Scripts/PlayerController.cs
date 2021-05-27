using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float baseSpeed = 1f;
	public Rigidbody2D rb2d;
	public bool canBlank = true;
	public bool canInteract = true;
	public Guid currentlyTouchingItem = Guid.Empty;
	private DungeonManager dungeonManager;
	void Awake()
	{
		transform.position = new Vector3Int(79, 71, 0);
	}
	void Update()
	{
		ProcessMovement();
		CheckBlank();
		CheckInteract();
	}
	void ProcessMovement()
	{
		var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		var speed = Mathf.Clamp(direction.magnitude, 0f, 1f);
		direction.Normalize();

		rb2d.velocity = baseSpeed * speed * direction;
	}
	void CheckBlank()
	{
		if (!Input.GetKey(KeyCode.Q) && !canBlank)
			canBlank = true;
		else if (Input.GetKey(KeyCode.Q) && canBlank)
		{
			canBlank = false;

			// Dungeon is always instantiated after player
			if (!dungeonManager)
				dungeonManager = transform.parent.GetChild(1).gameObject.GetComponent<DungeonManager>();

			dungeonManager.ProcessBlank(transform.position);
		}
	}
	void CheckInteract()
	{
		if (!Input.GetKey(KeyCode.E) && !canInteract)
			canInteract = true;
	}
}
