using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
public class ChestController : MonoBehaviour
{
	public Sprite defaultSprite;
	public Sprite highlightedSprite;
	public Sprite lockedSprite;
	public Sprite openedSprite;
	public SpriteRenderer spriteRenderer;
	public GameObject[] lootTable;
	public bool needsKey;
	private bool tempLocked = false;
	private bool opened = false;
	private Guid uid = Guid.NewGuid();
	private PlayerController player;
	private GameManager gameManager;
	void Start()
	{
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player") || opened || tempLocked;
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

			if (gameManager.keys == 0 && needsKey)
			{
				tempLocked = true;

				spriteRenderer.sprite = lockedSprite;

				StartCoroutine(TempLock());
			}
			else
			{
				if (needsKey)
					--gameManager.keys;

				spriteRenderer.sprite = openedSprite;

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
	void DropLoot()
	{
		var chestManager = transform.parent.parent.GetChild(8);

		for (var i = 0; i < 2; ++i)
		{
			var offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			var item = Util.GetArrayRandom(lootTable);
			var newItem = Instantiate(item, transform.position + offset, Quaternion.identity);
			newItem.transform.SetParent(chestManager);
		}
	}
}
