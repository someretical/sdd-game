using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float speed = 5f;
	public Rigidbody2D rigidBody;
	private Vector2 movement;
	void Update()
	{
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");
		rigidBody.MovePosition(rigidBody.position + movement * speed * Time.fixedDeltaTime);
	}
}
