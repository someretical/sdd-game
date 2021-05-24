using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed;
	public float maxSquaredSpeed;
	public Rigidbody2D rb2d;
	void Awake()
	{
		transform.position = new Vector3Int(70, 70, 0);
	}
	void FixedUpdate()
	{
		if (rb2d.velocity.sqrMagnitude < maxSquaredSpeed)
		{
			var movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
			rb2d.AddForce(speed * Time.fixedDeltaTime * movement);
		}
	}
}
