using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestCollider : MonoBehaviour
{
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	public Sprite openedSprite;
	public SpriteRenderer spriteRenderer;
	public GameObject coin;
	private bool opened = false;
	private Guid uid = Guid.NewGuid();
	private PlayerController player;
	// This is probably beyond the scope of the assessment
	// but if I do end up working on this project after
	// this note is just here to remind me to add loot tables for chests
	public static Vector3Int RoundPosition(Vector3 position)
	{
		return new Vector3Int((int)Math.Floor(position.x), (int)Math.Floor(position.y), 0);
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player") || opened;
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		if (!player)
			player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();

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

			spriteRenderer.sprite = openedSprite;

			opened = true;

			for (var i = 0; i < 2; ++i)
			{
				var newCoin = Instantiate(coin, RoundPosition(transform.position), Quaternion.identity);

				newCoin.transform.SetParent(transform.parent.parent.GetChild(8));
			}
		}
	}

}
