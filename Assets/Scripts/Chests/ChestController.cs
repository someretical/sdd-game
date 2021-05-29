﻿using System;
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
	public GameObject[] lootTable;
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
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();
		chestManagerTransform = transform.parent.parent.GetChild(9);
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
	}
	void Update()
	{
		if (!tempLocked && !playerTouching && !onDefaultSprite)
			spriteRenderer.sprite = defaultSprite;
	}
	bool PrelimCheck(Collider2D other)
	{
		return !other.gameObject.CompareTag("Player") || opened;
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
					--gameManager.keys;

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
	void DropLoot()
	{
		for (var i = 0; i < 2; ++i)
		{
			var offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			var item = Util.GetArrayRandom(lootTable);
			Instantiate(item, transform.position + offset, Quaternion.identity, chestManagerTransform);
		}
	}
}