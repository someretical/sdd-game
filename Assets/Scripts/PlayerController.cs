using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float baseSpeed = 1f;
	public Sprite defaultState;
	public Sprite invulnerableState;
	public bool canBlank = true;
	public bool canInteract = true;
	public bool canShoot = true;
	public bool canMove = true;
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
	private List<GameObject> fullScreenMap = new List<GameObject>();
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();
		levelManager = transform.parent.gameObject.GetComponent<LevelManager>();
		dungeonManager = transform.parent.GetChild(2).gameObject.GetComponent<DungeonManager>();
		miniMap = transform.GetChild(0).GetChild(0).gameObject;

		var c = transform.GetChild(0).GetChild(1).childCount;
		for (int i = 0; i < c; ++i)
			fullScreenMap.Add(transform.GetChild(0).GetChild(1).GetChild(i).gameObject);
	}
	void Update()
	{
		if (!levelManager.ready)
			return;

		CheckFullScreenMap();
		CheckBlank();
		CheckInteract();
		ProcessMovement();

		// Temporary debugging code
		if (Input.GetKeyDown(KeyCode.G))
			transform.parent.GetChild(2).GetChild(9).gameObject.GetComponent<ItemManager>().SpawnRoomClearReward(transform.position);
		if (Input.GetKeyDown(KeyCode.H))
			transform.parent.GetChild(2).GetChild(11).gameObject.GetComponent<EnemyManager>().SpawnEnemy(transform.position);
	}
	void CheckFullScreenMap()
	{
		if (!mapOpen && Input.GetKey(KeyCode.Tab))
		{
			mapOpen = true;

			miniMap.SetActive(false);
			for (var i = 0; i < fullScreenMap.Count; ++i)
				fullScreenMap[i].SetActive(true);
		}
		else if (mapOpen && !Input.GetKey(KeyCode.Tab))
		{
			mapOpen = false;

			for (var i = 0; i < fullScreenMap.Count; ++i)
				fullScreenMap[i].SetActive(false);
			miniMap.SetActive(true);
		}
	}
	float GetScaledSpeed()
	{
		float scaledSpeed = baseSpeed;

		if (mapOpen)
			scaledSpeed *= 0.5f;

		if (!inCombat)
			scaledSpeed *= 1.5f;

		return scaledSpeed;
	}
	void ProcessMovement()
	{
		var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		var speed = Mathf.Clamp(direction.magnitude, 0f, 1f);
		direction.Normalize();

		rb2d.velocity = canMove ? GetScaledSpeed() * speed * direction : new Vector2();
	}
	void CheckBlank()
	{
		if (!Input.GetKey(KeyCode.Q) && !canBlank)
			canBlank = true;
		else if (Input.GetKey(KeyCode.Q) && canBlank)
		{
			canBlank = false;
			--gameManager.blanks;

			dungeonManager.ProcessBlank(transform.position);
		}
	}
	void CheckInteract()
	{
		if (!Input.GetKey(KeyCode.E) && !canInteract)
			canInteract = true;
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
