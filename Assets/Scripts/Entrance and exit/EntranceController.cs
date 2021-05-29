using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceController : MonoBehaviour
{
	public Sprite closedState;
	private BoxCollider2D bc2d;
	private SpriteRenderer spriteRenderer;
	void Start()
	{
		bc2d = gameObject.GetComponent<BoxCollider2D>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			bc2d.size = new Vector2(1f, 1f);
			bc2d.isTrigger = false;
			spriteRenderer.sprite = closedState;
		}
	}
}
