using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemCollider : MonoBehaviour
{
	public string id;
	public int chestWeight = 0;
	public int roomClearWeight = 0;
	[Space]
	[Header("Sprites")]
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	private Guid uid = Guid.NewGuid();
	private SpriteRenderer spriteRenderer;
	private PlayerController player;
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();

		// These have rigid bodies but I don't want them to interact with the player
		// Could do it in Physics2D options with different layers but then the
		// Box colliders wouldn't interact either which IS a problem
		Physics2D.IgnoreCollision(player.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>(), GetComponent<Collider2D>());
		Physics2D.IgnoreCollision(player.transform.GetChild(0).GetChild(1).GetComponent<Collider2D>(), GetComponent<Collider2D>());
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (player.currentlyTouchingItem == Guid.Empty)
		{
			//Sets the player that is touching the item to the item uid.
			player.currentlyTouchingItem = uid;

			spriteRenderer.sprite = highlightedSprite;

			CheckInteract();
		}
	}
	void OnTriggerStay2D(Collider2D other)
	{
		//Checks to see if the player is still touching the item or not.
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
		//Sets the player to not interact with any items once the player isn't touching the collider.
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
		if (Input.GetButton("Interact") && player.canInteract)
		{
			player.canInteract = false;
			player.currentlyTouchingItem = Guid.Empty;
			player.OnItemPickup(id);

			Destroy(gameObject);
		}
	}
}
