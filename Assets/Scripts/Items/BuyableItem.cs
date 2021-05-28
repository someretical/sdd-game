using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuyableItem : MonoBehaviour
{
	public string id;
	public int basePrice;
	public int scaledPrice;
	// public Text priceTag;
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	public Sprite lockedSprite;
	public SpriteRenderer spriteRenderer;
	private Guid uid = Guid.NewGuid();
	private bool tempLocked = false;
	private PlayerController player;
	private GameManager gameManager;
	void Start()
	{
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
		scaledPrice = basePrice + (int)Math.Ceiling((gameManager.levelCounter - 1) * basePrice * gameManager.itemPriceIncreasePercentage);

		// Holy SHIT this piece of code worked in one try 
		// First time that's happened in so long
		Physics2D.IgnoreCollision(player.gameObject.GetComponent<Collider2D>(), transform.gameObject.GetComponent<Collider2D>());
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player") || tempLocked;
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

			if (gameManager.coins < scaledPrice)
			{
				tempLocked = true;

				spriteRenderer.sprite = lockedSprite;

				StartCoroutine(TempLock());
			}
			else
			{
				player.OnItemPickup(id);
				gameManager.coins -= scaledPrice;

				Destroy(gameObject);
			}
		}
	}
	IEnumerator TempLock()
	{
		yield return new WaitForSeconds(0.5f);

		spriteRenderer.sprite = defaultSprite;
		tempLocked = false;
	}
}
