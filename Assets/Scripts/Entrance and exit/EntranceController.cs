using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceController : MonoBehaviour
{
	public Sprite closedState;
	private CircleCollider2D c2d;
	private SpriteRenderer spriteRenderer;
	void Start()
	{
		c2d = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
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
