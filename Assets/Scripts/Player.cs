using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
	public float speed = 5f;
	private Vector3 lastMoveDir;
	private bool deletingDoor = false;
	void Update()
	{
		HandleMovement();
	}
	void HandleMovement()
	{
		var moveX = 0f;
		var moveY = 0f;

		if (Input.GetKey(KeyCode.W))
			moveY += 1f;
		if (Input.GetKey(KeyCode.D))
			moveX += 1f;
		if (Input.GetKey(KeyCode.S))
			moveY -= 1f;
		if (Input.GetKey(KeyCode.A))
			moveX -= 1f;

		if (moveX != 0f || moveY != 0f)
		{
			var moveDir = new Vector3(moveX, moveY, 0f).normalized;
			lastMoveDir = moveDir;
			var target = transform.position + moveDir * speed * Time.deltaTime;

			var collider = GetComponent<BoxCollider2D>();
			collider.enabled = false;
			var raycastHit = Physics2D.Raycast(transform.position, moveDir, speed * Time.deltaTime);

			if (raycastHit.collider == null)
				transform.position = target;
			else
			{
				var testMoveDir = new Vector3(moveDir.x, 0f, 0f).normalized;
				target = transform.position + testMoveDir * speed * Time.deltaTime;
				raycastHit = Physics2D.Raycast(transform.position, testMoveDir, speed * Time.deltaTime);

				if (raycastHit.collider == null)
				{
					lastMoveDir = testMoveDir;
					transform.position = target;
				}
				else
				{
					testMoveDir = new Vector3(0f, moveDir.y, 0f).normalized;
					target = transform.position + testMoveDir * speed * Time.deltaTime;
					raycastHit = Physics2D.Raycast(transform.position, testMoveDir, speed * Time.deltaTime);

					if (raycastHit.collider == null)
					{
						lastMoveDir = testMoveDir;
						transform.position = target;
					}
				}
			}

			collider.enabled = true;
		}
	}
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Door" && !deletingDoor)
		{
			deletingDoor = true;

			StartCoroutine(DeleteDoorCollider(collider.gameObject.GetComponent<BoxCollider2D>()));
		}
	}
	IEnumerator DeleteDoorCollider(BoxCollider2D collider)
	{
		yield return new WaitForSeconds(0.1f);

		Destroy(collider);

		deletingDoor = false;
	}
}