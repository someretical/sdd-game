using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	[Header("General information")]
	public int weight;
	public int coinReward;
	public int health;
	public bool jammed;
	[Space]
	[Header("Colour tinting")]
	public Color defaultColor;
	public Color damageColor;
	[Space]
	[Header("Bullets")]
	public GameObject regularBullet;
	public GameObject jammedBullet;
	public GameObject deathAnimation;
	[HideInInspector]
	public NavMeshAgent agent;
	[HideInInspector]
	public NavMeshObstacle obstacle;
	[HideInInspector]
	public int boundRoomID;
	private bool firing = false;
	private bool stunned = false;
	private int fireCounter = 0;
	private PlayerController player;
	private SpriteRenderer sprite;
	private ItemManager itemManager;
	private DungeonManager dungeonManager;
	void Start()
	{
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		agent = transform.GetChild(1).GetComponent<NavMeshAgent>();
		obstacle = transform.GetChild(1).GetComponent<NavMeshObstacle>();
		player = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();
		itemManager = transform.parent.parent.GetChild(8).GetComponent<ItemManager>();
		dungeonManager = transform.parent.parent.GetComponent<DungeonManager>();

		var position = new Vector3(transform.position.x, transform.position.y, 0f);
		transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		sprite.transform.position = position;
		agent.transform.position = position;
	}
	void Update()
	{
		CheckTarget();
	}
	void FixedUpdate()
	{
		// Runs every ~.2 seconds
		++fireCounter;

		if (!firing && Random.Range(0, 10) == 0 && fireCounter > 5)
			StartCoroutine(Shoot());
	}
	IEnumerator Shoot()
	{
		firing = true;
		var bullet = Instantiate(
			jammed ? jammedBullet : regularBullet,
			sprite.transform.position,
			Quaternion.identity,
			dungeonManager.bulletManager.transform
		);
		var vec3Rotation = (player.transform.position - sprite.transform.position).normalized * (jammed ? 5 : 2.5f);
		bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(vec3Rotation.x, vec3Rotation.y);

		yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

		firing = false;
	}
	public void CheckHealth()
	{
		if (health == 0)
		{
			// I probably astracted this code to the wrong class
			// Perhaps it would have been better to set the loot tables individually for each type of enemy controller
			// Then each individual child class could override the SpawnEnemyDrops function or whatever
			// But that's too bothersome because the current setup already does the bare minimum
			itemManager.SpawnEnemyDrops(sprite.transform.position, coinReward);

			// Setting actualRoomID to boundRoomID
			// Then changing boundRoomID to -1 
			// Have to create and pass a copy because registerEnemyDeath checks the boundRoomID
			// So if I change it, pass it, then the function checks the changed boundRoomID
			// which will always return true (and thus fail)
			// because I can't call registerEnemyDeath AFTER destroy(gameObject)
			// because at that point this instance will no longer exist
			var actualRoomID = boundRoomID;
			boundRoomID = -1;
			dungeonManager.RegisterEnemyDeath(actualRoomID, sprite.transform.position);

			var death = Instantiate(deathAnimation, sprite.transform.position, Quaternion.identity);
			Destroy(death, 0.25f);

			Destroy(gameObject);
		}
	}
	public void CheckTarget()
	{
		if (stunned)
			return;
		// Make the enemy an obstacle if it is too close to the player
		var adjustedAgentPosition = new Vector3(agent.transform.position.x, agent.transform.position.y, 0f);
		if ((player.transform.position - adjustedAgentPosition).sqrMagnitude < Mathf.Pow(agent.stoppingDistance, 2))
		{
			agent.enabled = false;
			obstacle.enabled = true;
		}
		else
		{
			obstacle.enabled = false;
			agent.enabled = true;
			agent.SetDestination(player.transform.position);
		}

		// Move the rigid body associated with this gameobject
		var adjustedPosition = new Vector3(agent.transform.position.x, agent.transform.position.y, 0f);
		sprite.transform.position = Vector3.Lerp(sprite.transform.position, adjustedPosition, Time.deltaTime * 2);
	}
	public void InflictDamage(int damage)
	{
		health -= damage;

		StartCoroutine(DisplayDamage());
		StartCoroutine(Stun());

		CheckHealth();
	}
	IEnumerator DisplayDamage()
	{
		sprite.color = damageColor;

		yield return new WaitForSeconds(0.2f);

		sprite.color = defaultColor;
	}
	IEnumerator Stun()
	{
		stunned = true;
		agent.enabled = false;
		obstacle.enabled = true;

		yield return new WaitForSeconds(0.15f);

		stunned = false;
	}
}
