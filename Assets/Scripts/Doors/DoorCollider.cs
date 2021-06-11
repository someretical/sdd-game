using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
	public int id;
	void OnTriggerEnter2D(Collider2D c2d)
	{
		//Upon collision, if the other collider is the player and the door isn't opened, the method "OnPlayerEnter" is called.
		//The method is run upon all Monobehaviours of the gameobject.
		if (
			c2d.gameObject.CompareTag("Player") &&
			!gameObject.transform.parent.GetComponent<DoorController>().opened
		)
			gameObject.SendMessageUpwards("OnPlayerEnter", id, SendMessageOptions.RequireReceiver);
	}
}
