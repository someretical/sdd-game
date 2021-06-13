using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
[System.Serializable]
public class PlayerController : MonoBehaviour
{
	[Header("Speed")]
	public float baseSpeed = 1f;
	[Space]
	[Header("Sprites")]
	public Sprite defaultState;
	public Sprite invulnerableState;
	public Sprite dodgeRollingState;
	[Space]
	[Header("Bullets")]
	public Transform firePoint;
	public GameObject bulletPrefab;
	public float bulletForce = 10f;
	[HideInInspector]
	public bool canBlank = true;
	[HideInInspector]
	public bool canInteract = true;
	[HideInInspector]
	public bool canShoot = true;
	[HideInInspector]
	public bool canMove = true;
	[HideInInspector]
	public bool canDodgeRoll = true;
	[HideInInspector]
	public bool dodgeRolling = false;
	[HideInInspector]
	public bool invulnerable = false;
	[HideInInspector]
	public Guid currentlyTouchingItem = Guid.Empty;
	[HideInInspector]
	public bool inCombat = false;
	[Space]
	[Header("Helper references")]
	public Rigidbody2D rb2d;
	public SpriteRenderer spriteRenderer;
	public LevelManager levelManager;
	public DungeonManager dungeonManager;
	public GameObject miniMap;
	public Camera fullScreenMapCamera;
	public Camera cam;
	public CameraController cameraController;
	public GameObject fullScreenMap;
	public PauseMenu pauseMenu;
	private GameManager gameManager;
	private bool mapOpen = false;
	void Start()
	{
		gameManager = transform.parent.parent.GetComponent<GameManager>();
	}
	void Update()
	{
		//If the level manager is ready, it doesn't return the value and runs the functions instead.
		if (!levelManager.ready || pauseMenu.paused)
			return;

		CheckInteract();
		CheckFullScreenMap();
		CheckBlank();
		ProcessRotation();
		CheckFire();
		ProcessMovement();
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
			fullScreenMap.SetActive(true);
		}
		else if (Input.GetButtonUp("Map"))
		{
			mapOpen = false;

			fullScreenMap.SetActive(false);
			miniMap.SetActive(true);
		}

		// Zoom out
		if (mapOpen && Input.GetAxis("Scroll") > 0f)
			fullScreenMapCamera.orthographicSize = Math.Max(20, fullScreenMapCamera.orthographicSize - 1);

		// Zoom in
		if (mapOpen && Input.GetAxis("Scroll") < 0f)
			fullScreenMapCamera.orthographicSize = Math.Min(100, fullScreenMapCamera.orthographicSize + 1);
	}
	public float GetScaledSpeed()
	{
		// Different speed for different actions
		float scaledSpeed = baseSpeed;

		if (mapOpen)
			scaledSpeed *= 0.5f;

		if (dodgeRolling)
			scaledSpeed *= 4f;

		if (!inCombat)
			scaledSpeed *= 1.3f;
		else
			scaledSpeed *= 0.75f;

		return scaledSpeed;
	}
	Vector3 GetLookDirection()
	{
		return cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
	}
	IEnumerator DodgeRoll()
	{
		canDodgeRoll = false;
		dodgeRolling = true;
		spriteRenderer.sprite = dodgeRollingState;

		var lookDir = GetLookDirection();
		rb2d.velocity = GetScaledSpeed() * new Vector2(lookDir.x, lookDir.y).normalized;

		yield return new WaitForSeconds(inCombat ? 0.16f : 0.12f);

		dodgeRolling = false;
		spriteRenderer.sprite = defaultState;

		yield return new WaitForSeconds(0.3f);

		canDodgeRoll = true;
	}
	void ProcessRotation()
	{
		// Fancy piece of code that makes the player rotate towards the cursor
		var lookDir = GetLookDirection();
		spriteRenderer.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f);
	}
	IEnumerator ShootCooldown()
	{
		canShoot = false;

		yield return new WaitForSeconds(0.2f);

		canShoot = true;
	}
	void CheckFire()
	{
		if (!Input.GetButtonDown("Attack") || !canShoot)
			return;

		var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, dungeonManager.bulletManager.transform);
		bullet.GetComponent<Rigidbody2D>().velocity = firePoint.up * bulletForce;

		StartCoroutine(ShootCooldown());
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
		if (Input.GetButtonDown("DodgeRoll") && canDodgeRoll)
		{
			StartCoroutine(DodgeRoll());

			return;
		}

		rb2d.velocity = GetScaledSpeed() * new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
	}
	IEnumerator BlankCooldown()
	{
		yield return new WaitForSeconds(1f);

		canBlank = true;
	}
	void CheckBlank()
	{
		if (Input.GetButtonDown("Blank") && canBlank && gameManager.blanks > 0)
		{
			canBlank = false;
			--gameManager.blanks;
			cameraController.UpdateHUD();

			dungeonManager.ProcessBlank(transform.position);

			StartCoroutine(BlankCooldown());
		}
	}
	public void OnItemPickup(string id)
	{
		//When items are picked up, the ID is checked through a switch loop and then adds the object to the player.
		switch (id)
		{
			case "CoinStack":
				var scaling = (int)Math.Ceiling((gameManager.levelCounter - 1) * 15 * gameManager.itemPriceIncreasePercentage);

				gameManager.coins += 15 + scaling;
				gameManager.totalCoins += 15 + scaling;
				break;
			case "CoinPickup":
				++gameManager.coins;
				break;
			case "2CoinPickup":
				gameManager.coins += 2;
				gameManager.totalCoins += 2;
				break;
			case "5CoinPickup":
				gameManager.coins += 5;
				gameManager.totalCoins += 5;
				break;
			case "10CoinPickup":
				gameManager.coins += 10;
				gameManager.totalCoins += 10;
				break;
			case "20CoinPickup":
				gameManager.coins += 20;
				gameManager.totalCoins += 20;
				break;
			case "50CoinPickup":
				gameManager.coins += 50;
				gameManager.totalCoins += 50;
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

		cameraController.UpdateHUD();
	}
	public void InflictDamage(int damage)
	{
		//If the player is invulnerable, the code stops and doesn't proceed with checks.
		if (invulnerable)
			return;
		//If invulnerable was set to false, then the armour is checked.
		if (gameManager.armour > 0)
		{
			//Subtracts from the armour and stops after.
			--gameManager.armour;
			return;
		}
		//After the player has been hit, the player is invulnerable for a short period of time.
		rb2d.velocity = Vector2.zero;
		spriteRenderer.sprite = invulnerableState;
		invulnerable = true;
		canMove = false;

		StartCoroutine(IFramesCooldown());

		StartCoroutine(FreezePlayer());
		//The HP is then decreased after getting hit, given that the player isn't in an invulnerable state and that the player had no armour.\
		// Make sure hp counter doesn't go negative
		gameManager.hp = Mathf.Max(0, gameManager.hp - damage);
		cameraController.UpdateHUD();
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
	void OnCollisionStay2D(Collision2D other)
	{
		//When the gameObject is touching the enemy, it calls the CheckEnemy function.
		CheckEnemy(other);
	}
	void CheckEnemy(Collision2D other)
	{
		//Calls the inflict damage function when one of the conditions is satisfied.
		if (other.collider.CompareTag("Enemy"))
			InflictDamage(1);
		else if (other.collider.CompareTag("JammedEnemy"))
			InflictDamage(2);
	}
}
