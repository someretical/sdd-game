using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorColliderTop : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D c2d)
	{
		if (
			c2d.gameObject.CompareTag("Player") &&
			!gameObject.transform.parent.gameObject.GetComponent<DoorController>().playerEntered
		)
		{
			gameObject.transform.parent.gameObject.GetComponent<DoorController>().playerEntered = true;

			gameObject.SendMessageUpwards("OnPlayerEnter", 0, SendMessageOptions.RequireReceiver);
		}
	}
}
