using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemCollider : MonoBehaviour
{
	public string id;
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	private Guid uid = Guid.NewGuid();
	private SpriteRenderer spriteRenderer;
	private PlayerController player;
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();

		// Holy SHIT this piece of code worked in one try 
		// First time that's happened in so long
		Physics2D.IgnoreCollision(player.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (player.currentlyTouchingItem == Guid.Empty)
		{
			player.currentlyTouchingItem = uid;

			spriteRenderer.sprite = highlightedSprite;

			CheckInteract();
		}
	}
	void OnTriggerStay2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (player.currentlyTouchingItem == uid)
			CheckInteract();
		else if (player.currentlyTouchingItem == Guid.Empty)
		{
			player.currentlyTouchingItem = uid;

			spriteRenderer.sprite = highlightedSprite;

			CheckInteract();
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (player.currentlyTouchingItem == uid)
		{
			player.currentlyTouchingItem = Guid.Empty;

			spriteRenderer.sprite = defaultSprite;
		}
	}
	void CheckInteract()
	{
		if (Input.GetKey(KeyCode.E) && player.canInteract)
		{
			player.canInteract = false;

			player.currentlyTouchingItem = Guid.Empty;

			player.OnItemPickup(id);

			Destroy(gameObject);
		}
	}
}
