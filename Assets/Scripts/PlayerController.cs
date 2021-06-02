﻿using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
[System.Serializable]
public class PlayerController : MonoBehaviour
{
	public float baseSpeed = 1f;
	public Sprite defaultState;
	public Sprite invulnerableState;
	public bool canBlank = true;
	public bool canInteract = true;
	public bool canShoot = true;
	public bool canMove = true;
	public bool canDodgeRoll = true;
	public bool dodgeRolling = false;
	public bool invulnerable = false;
	public Guid currentlyTouchingItem = Guid.Empty;
	public bool inCombat = false;
	private bool mapOpen = false;
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb2d;
	private GameManager gameManager;
	private LevelManager levelManager;
	private DungeonManager dungeonManager;
	private GameObject miniMap;
	private Camera fullScreenMapCamera;
	private BoxCollider2D dodgeRollCollider;
	private readonly List<GameObject> fullScreenMap = new List<GameObject>();
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();
		levelManager = transform.parent.gameObject.GetComponent<LevelManager>();
		dungeonManager = transform.parent.GetChild(2).gameObject.GetComponent<DungeonManager>();
		miniMap = transform.GetChild(1).GetChild(0).gameObject;
		fullScreenMapCamera = transform.GetChild(3).GetChild(0).gameObject.GetComponent<Camera>();
		dodgeRollCollider = transform.GetChild(0).GetChild(1).gameObject.GetComponent<BoxCollider2D>();

		var c = transform.GetChild(1).GetChild(1).childCount;
		for (int i = 0; i < c; ++i)
			fullScreenMap.Add(transform.GetChild(1).GetChild(1).GetChild(i).gameObject);
	}
	void Update()
	{
		if (!levelManager.ready)
			return;

		CheckInteract();
		CheckFullScreenMap();
		CheckBlank();
		ProcessMovement();

		// Temporary debugging code
		if (Input.GetKeyDown(KeyCode.K))
			currentlyTouchingItem = Guid.Empty;
		if (Input.GetKeyDown(KeyCode.G))
			transform.parent.GetChild(2).GetChild(9).gameObject.GetComponent<ItemManager>().SpawnRoomClearReward(transform.position);
		if (Input.GetKeyDown(KeyCode.H))
			transform.parent.GetChild(2).GetChild(11).gameObject.GetComponent<EnemyManager>().SpawnEnemy(transform.position);
	}
	void CheckInteract()
	{
		// Had to use GetButton because for some reason this was
		// the only way it would work with the item system I
		// implemented
		if (!Input.GetButton("Interact") && !canInteract)
			canInteract = true;
	}
	void CheckFullScreenMap()
	{
		if (Input.GetButtonDown("Map"))
		{
			mapOpen = true;

			miniMap.SetActive(false);
			for (var i = 0; i < fullScreenMap.Count; ++i)
				fullScreenMap[i].SetActive(true);
		}
		else if (Input.GetButtonUp("Map"))
		{
			mapOpen = false;

			for (var i = 0; i < fullScreenMap.Count; ++i)
				fullScreenMap[i].SetActive(false);
			miniMap.SetActive(true);
		}

		if (mapOpen && Input.GetAxis("Scroll") > 0f)
			fullScreenMapCamera.orthographicSize = Math.Min(100, fullScreenMapCamera.orthographicSize + 1);

		if (mapOpen && Input.GetAxis("Scroll") < 0f)
			fullScreenMapCamera.orthographicSize = Math.Max(20, fullScreenMapCamera.orthographicSize - 1);
	}
	float GetScaledSpeed()
	{
		float scaledSpeed = baseSpeed;

		if (mapOpen)
			scaledSpeed *= 0.5f;

		if (dodgeRolling)
			scaledSpeed *= 0.75f;

		if (!inCombat)
			scaledSpeed *= 1.5f;

		return scaledSpeed;
	}
	IEnumerator DodgeRoll()
	{
		canDodgeRoll = false;
		dodgeRollCollider.enabled = false;
		dodgeRolling = true;
		spriteRenderer.sprite = invulnerableState;

		yield return new WaitForSeconds(0.5f);

		dodgeRollCollider.enabled = true;
		dodgeRolling = false;
		spriteRenderer.sprite = defaultState;

		yield return new WaitForSeconds(0.5f);
		canDodgeRoll = true;
	}
	void ProcessMovement()
	{
		if (!canMove || dodgeRolling)
			return;

		// Rigid body will continue moving if the velocity is NOT set to zero
		// This is why the movement is snappy, because it is being updated every frame by this statement
		// (If no movement keys are pressed the direction will be zero)
		// However, during a dodgeroll, the player cannot be stopped until they land
		// Hence, we just need to set the velocity and prevent it from being updated
		// for a certain amount of time
		if (
			Input.GetButtonDown("DodgeRoll") &&
			(Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
			&& canDodgeRoll
		)
			StartCoroutine(DodgeRoll());

		rb2d.velocity = GetScaledSpeed() * new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
	}
	IEnumerator BlankCooldown()
	{
		yield return new WaitForSeconds(1f);

		canBlank = true;
	}
	void CheckBlank()
	{
		if (Input.GetButtonDown("Blank") && canBlank)
		{
			canBlank = false;
			--gameManager.blanks;

			dungeonManager.ProcessBlank(transform.position);

			StartCoroutine(BlankCooldown());
		}
	}
	public void OnItemPickup(string id)
	{
		switch (id)
		{
			case "CoinStack":
				gameManager.coins += 15 + (int)Math.Ceiling((gameManager.levelCounter - 1) * 15 * gameManager.itemPriceIncreasePercentage);
				break;
			case "CoinPickup":
				++gameManager.coins;
				break;
			case "2CoinPickup":
				gameManager.coins += 2;
				break;
			case "5CoinPickup":
				gameManager.coins += 5;
				break;
			case "10CoinPickup":
				gameManager.coins += 10;
				break;
			case "20CoinPickup":
				gameManager.coins += 20;
				break;
			case "50CoinPickup":
				gameManager.coins += 50;
				break;
			case "KeyPickup":
			// FALL THROUGH
			case "KeyShopItem":
				++gameManager.keys;
				break;
			case "BlankPickup":
			// FALL THROUGH
			case "BlankShopItem":
				++gameManager.blanks;
				break;
			case "ArmourPickup":
			// FALL THROUGH
			case "ArmourShopItem":
				++gameManager.armour;
				break;
			case "HeartPickup":
			// FALL THROUGH
			case "HeartShopItem":
				gameManager.hp = Math.Min(gameManager.maxHp, gameManager.hp + 2);
				break;
			case "HalfHeartPickup":
			// FALL THROUGH
			case "HalfHeartShopItem":
				if (gameManager.hp < gameManager.maxHp)
					++gameManager.hp;
				break;
			case "MapItem":
			// FALL THROUGH
			case "MapShopItem":
				var dungeonGenerator = dungeonManager.dungeonGenerator;

				for (var i = 0; i < dungeonGenerator.PathPoints.Count; ++i)
					for (var j = 0; j < dungeonGenerator.PathPoints[i].Count; ++j)
					{
						var position = new Vector3Int(dungeonGenerator.PathPoints[i][j].x, dungeonManager.mapHeight - 1 - dungeonGenerator.PathPoints[i][j].y, 0);
						dungeonManager.PlaceMinimapTile(dungeonGenerator.PathPoints[i][j], position);
					}

				for (var i = 0; i < dungeonGenerator.RoomPoints.Count; ++i)
					for (var j = 0; j < dungeonGenerator.RoomPoints[i].Count; ++j)
					{
						var position = new Vector3Int(dungeonGenerator.RoomPoints[i][j].x, dungeonManager.mapHeight - 1 - dungeonGenerator.RoomPoints[i][j].y, 0);
						dungeonManager.PlaceMinimapTile(dungeonGenerator.RoomPoints[i][j], position);
					}
				break;
		}
	}
	public void InflictDamage(int damage)
	{
		if (invulnerable)
			return;

		if (gameManager.armour > 0)
		{
			--gameManager.armour;
			return;
		}

		rb2d.velocity = Vector2.zero;
		spriteRenderer.sprite = invulnerableState;
		invulnerable = true;
		canMove = false;

		StartCoroutine(IFramesCooldown());

		StartCoroutine(FreezePlayer());

		gameManager.hp -= damage;
	}
	IEnumerator IFramesCooldown()
	{
		yield return new WaitForSeconds(1f);

		invulnerable = false;
		spriteRenderer.sprite = defaultState;
	}
	IEnumerator FreezePlayer()
	{
		yield return new WaitForSeconds(0.1f);

		canMove = true;
	}
}
