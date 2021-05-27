using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
	public int id;
	void OnTriggerEnter2D(Collider2D c2d)
	{
		if (
			c2d.gameObject.CompareTag("Player") &&
			!gameObject.transform.parent.gameObject.GetComponent<DoorController>().playerEntered
		)
			gameObject.SendMessageUpwards("OnPlayerEnter", id, SendMessageOptions.RequireReceiver);
	}
}
