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
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb2d;
	private GameManager gameManager;
	private LevelManager levelManager;
	private DungeonManager dungeonManager;
	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();
		levelManager = transform.parent.gameObject.GetComponent<LevelManager>();
		dungeonManager = transform.parent.GetChild(1).gameObject.GetComponent<DungeonManager>();
	}
	void Update()
	{
		if (!levelManager.ready)
			return;

		ProcessMovement();
		CheckBlank();
		CheckInteract();
	}
	void ProcessMovement()
	{
		var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		var speed = Mathf.Clamp(direction.magnitude, 0f, 1f);
		direction.Normalize();

		rb2d.velocity = canMove ? baseSpeed * speed * direction : new Vector2();
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
		// else if (Input.GetKey(KeyCode.E) && canInteract)
		// {
		// 	canInteract = false;

		// 	// Temporary very unoptimised code
		// 	transform.parent.GetChild(1).GetChild(9).gameObject.GetComponent<ItemManager>().SpawnItem(transform.position);
		// }
	}
	public void OnItemPickup(string id)
	{

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

		Debug.Log("damage taken");

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
