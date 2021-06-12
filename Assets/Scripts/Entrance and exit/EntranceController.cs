using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceController : MonoBehaviour
{
	public Sprite closedState;
	public CircleCollider2D c2d;
	public SpriteRenderer spriteRenderer;
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// Make player unable to enter the entrance after leaving its hitbox
			c2d.radius = 0.55f;
			c2d.isTrigger = false;
			spriteRenderer.sprite = closedState;
		}
	}
}
