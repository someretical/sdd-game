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
	[Space]
	[Header("Sprites")]
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
		spriteRenderer = GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();
		gameManager = transform.parent.parent.parent.parent.GetComponent<GameManager>();

		scaledPrice = basePrice + (int)Math.Ceiling((gameManager.levelCounter - 1) * basePrice * gameManager.itemPriceIncreasePercentage);
		textMeshPro = transform.GetChild(0).GetComponent<TextMeshPro>();
		textMeshPro.text = scaledPrice.ToString();

		// These have rigid bodies but I don't want them to interact with the player
		// Could do it in Physics2D options with different layers but then the
		// Box colliders wouldn't interact either which IS a problem
		Physics2D.IgnoreCollision(player.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>(), GetComponent<Collider2D>());
		Physics2D.IgnoreCollision(player.transform.GetChild(0).GetChild(1).GetComponent<Collider2D>(), GetComponent<Collider2D>());
	}
	void Update()
	{
		if (!tempLocked && !playerTouching && !onDefaultSprite)
			spriteRenderer.sprite = defaultSprite;
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox");
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		//Checks the tag of the object that has collided.
		if (PrelimCheck(other))
			return;

		playerTouching = true;

		if (tempLocked)
			return;
		//Sets the currently touching item to the uid.
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
		//The player is touching the item, with the option to purchase if they have sufficient coins.
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
		//The player is no longer touching the item.
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
		if (Input.GetButton("Interact") && player.canInteract)
		{
			player.canInteract = false;
			player.currentlyTouchingItem = Guid.Empty;

			if (gameManager.coins < scaledPrice)
			{
				//Item cannot be purchased.
				tempLocked = true;

				spriteRenderer.sprite = lockedSprite;
				onDefaultSprite = false;

				StartCoroutine(TempLock());
			}
			else
			{
				//Item is purchased
				gameManager.coins -= scaledPrice;
				player.OnItemPickup(id);

				Destroy(gameObject);
			}
		}
	}
	IEnumerator TempLock()
	{
		// Display red outline if the player is too poor to buy the item

		yield return new WaitForSeconds(0.5f);

		tempLocked = false;
	}
}
