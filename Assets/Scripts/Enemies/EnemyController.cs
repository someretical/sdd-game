using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public int weight;
	public int coinReward;
	public int health;
	public GameObject deathAnimation;
	[HideInInspector]
	public NavMeshAgent agent;
	[HideInInspector]
	public NavMeshObstacle obstacle;
	private PlayerController player;
	private SpriteRenderer sprite;
	private ItemManager itemManager;
	void Start()
	{
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		agent = transform.GetChild(1).GetComponent<NavMeshAgent>();
		obstacle = transform.GetChild(1).GetComponent<NavMeshObstacle>();
		player = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerController>();
		itemManager = transform.parent.parent.GetChild(8).GetComponent<ItemManager>();

		var position = new Vector3(transform.position.x, transform.position.y, 0f);
		transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		sprite.transform.position = position;
		agent.transform.position = position;
	}
	void Update()
	{
		CheckTarget();
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

			var death = Instantiate(deathAnimation, sprite.transform.position, Quaternion.identity);
			Destroy(death, 0.25f);

			Destroy(gameObject);
		}
	}
	public void CheckTarget()
	{
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
		sprite.transform.position = Vector3.Lerp(sprite.transform.position, adjustedPosition, Time.deltaTime * 5);
	}
	public void InflictDamage(int damage)
	{
		health -= damage;

		StartCoroutine(DisplayDamage());

		CheckHealth();
	}
	IEnumerator DisplayDamage()
	{
		sprite.color = Color.red;

		yield return new WaitForSeconds(0.2f);

		sprite.color = Color.white;
	}
}
