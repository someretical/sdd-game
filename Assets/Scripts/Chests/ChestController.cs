using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
public class ChestController : MonoBehaviour
{
	[Header("Sprites")]
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	public Sprite lockedSprite;
	public Sprite openedSprite;
	[Space]
	[Header("Loot table")]
	public GameObject[] lootTable;
	[HideInInspector]
	public bool rare = false;
	[HideInInspector]
	public bool needsKey;
	private bool onDefaultSprite = true;
	private bool tempLocked = false;
	private bool opened = false;
	private bool playerTouching = false;
	private Guid uid = Guid.NewGuid();
	private SpriteRenderer spriteRenderer;
	private PlayerController player;
	private Transform chestManagerTransform;
	private GameManager gameManager;
	private CameraController cameraController;
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();
		chestManagerTransform = transform.parent.parent.GetChild(8);
		gameManager = transform.parent.parent.parent.parent.GetComponent<GameManager>();
		cameraController = transform.parent.parent.parent.parent.GetChild(1).GetComponent<CameraController>();
	}
	void Update()
	{
		if (!tempLocked && !playerTouching && !onDefaultSprite)
			spriteRenderer.sprite = defaultSprite;
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("PlayerDodgeRollHitbox") || opened;
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		//Upon collision, checks for the player.
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
		//Checks to see if the player is touching or if the chest is locked.
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
		//The item is deselected once the player isn't touching the items.
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

			// Chests in secret rooms do not need keys to open
			if (gameManager.keys == 0 && needsKey)
			{
				tempLocked = true;

				spriteRenderer.sprite = lockedSprite;
				onDefaultSprite = false;

				StartCoroutine(TempLock());
			}
			else
			{
				if (needsKey)
				{
					--gameManager.keys;
					cameraController.UpdateHUD();
				}

				spriteRenderer.sprite = openedSprite;
				onDefaultSprite = true;

				opened = true;

				DropLoot();
			}
		}
	}
	IEnumerator TempLock()
	{
		yield return new WaitForSeconds(0.5f);

		tempLocked = false;
	}
	public GameObject GetRandomItem()
	{
		var chances = new List<int>();

		// Using weight relative to the total weights of all the items
		// Quick and dirty way that doesn't care about memory
		for (var i = 0; i < lootTable.Length; ++i)
			for (var l = 0; l < lootTable[i].GetComponent<ItemCollider>().chestWeight; ++l)
				chances.Add(i);

		return lootTable[Util.GetListRandom(chances)];
	}
	public void DropLoot()
	{
		//The max number of items instantiated depends on chest rarity.
		//If it is a rare chest, there are 4 items.
		//If it isn't a rare chest, only 2 items are spawned.
		var max = rare ? 4 : 2;

		for (var i = 0; i < max; ++i)
			Instantiate(
				GetRandomItem(),
				transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f),
				Quaternion.identity,
				chestManagerTransform
			);
	}
}
