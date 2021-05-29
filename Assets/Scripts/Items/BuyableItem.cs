using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class BuyableItem : MonoBehaviour
{
	public string id;
	public int weight;
	public int basePrice;
	public int scaledPrice;
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	public Sprite lockedSprite;
	private Guid uid = Guid.NewGuid();
	private bool onDefaultSprite = true;
	private bool playerTouching = false;
	private bool tempLocked = false;
	private SpriteRenderer spriteRenderer;
	private TextMeshPro textMeshPro;
	private PlayerController player;
	private GameManager gameManager;
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();

		scaledPrice = basePrice + (int)Math.Ceiling((gameManager.levelCounter - 1) * basePrice * gameManager.itemPriceIncreasePercentage);
		textMeshPro = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		textMeshPro.text = scaledPrice.ToString();

		// Holy SHIT this piece of code worked in one try 
		// First time that's happened in so long
		Physics2D.IgnoreCollision(player.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
	}
	void Update()
	{
		if (!tempLocked && !playerTouching && !onDefaultSprite)
			spriteRenderer.sprite = defaultSprite;
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		playerTouching = true;

		if (tempLocked)
			return;

		if (player.currentlyTouchingItem == Guid.Empty)
		{
			player.currentlyTouchingItem = uid;

			spriteRenderer.sprite = highlightedSprite;
			onDefaultSprite = false;

			CheckInteract();
		}
	}
	void OnTriggerStay2D(Collider2D other)
	{
		if (PrelimCheck(other) || tempLocked)
			return;

		if (player.currentlyTouchingItem == uid)
			CheckInteract();
		else if (player.currentlyTouchingItem == Guid.Empty)
		{
			player.currentlyTouchingItem = uid;

			spriteRenderer.sprite = highlightedSprite;
			onDefaultSprite = false;

			CheckInteract();
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (PrelimCheck(other))
			return;

		playerTouching = false;

		if (tempLocked)
			return;

		if (player.currentlyTouchingItem == uid)
		{
			player.currentlyTouchingItem = Guid.Empty;

			spriteRenderer.sprite = defaultSprite;
			onDefaultSprite = true;
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
				onDefaultSprite = false;

				StartCoroutine(TempLock());
			}
			else
			{
				gameManager.coins -= scaledPrice;
				player.OnItemPickup(id);

				Destroy(gameObject);
			}
		}
	}
	IEnumerator TempLock()
	{
		yield return new WaitForSeconds(0.5f);

		tempLocked = false;
	}
}
