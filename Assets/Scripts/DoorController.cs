using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public Sprite openedState;
	private bool deleting = false;
	void OnCollisionEnter2D(Collision2D c2d)
	{
		if (c2d.gameObject.CompareTag("Player") && !deleting)
		{
			deleting = true;

			StartCoroutine(DeleteDoorCollider());
		}
	}
	IEnumerator DeleteDoorCollider()
	{
		yield return new WaitForSeconds(0.005f);

		transform.parent.parent.gameObject.GetComponent<DungeonManager>().UpdateAdjacentWalls();

		Destroy(transform.gameObject.GetComponent<BoxCollider2D>());

		deleting = false;

		transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedState;
	}
}
