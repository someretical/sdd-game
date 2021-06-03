using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float radius;
	private Transform player;
	private Camera cam;
	void Start()
	{
		player = transform.parent.GetChild(0);
		cam = GetComponent<Camera>();
	}

	public void LateUpdate()
	{
		// This piece of fancy code offets the camera location
		// to 10% of the distance and direction from the player to the cursor
		// This makes the game feel more dynamic I think
		var mouseOffset = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - cam.transform.position.z)) - player.position;
		var mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -transform.position.z));

		transform.position = new Vector3(
			(mousePos.x - player.position.x) * 0.1f + player.position.x,
			(mousePos.y - player.position.y) * 0.1f + player.position.y,
			transform.position.z
		);

		// Should have max radius that the camera should stay WITHIN
		var dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y));
		if (dist > radius)
		{
			var norm = mouseOffset.normalized;
			transform.position = new Vector3(norm.x * radius + player.position.x, norm.y * radius + player.position.y, transform.position.z);
		}
	}
}
