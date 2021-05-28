using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceController : MonoBehaviour
{
	public BoxCollider2D bc2d;
	public SpriteRenderer spriteRenderer;
	public Sprite closedState;
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
