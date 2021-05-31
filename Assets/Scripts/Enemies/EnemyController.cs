using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public int weight;
	public int coinReward;
	public int health;
	public NavMeshAgent agent;
	public NavMeshObstacle obstacle;
	private PlayerController player;
	private Transform sprite;
	void Start()
	{
		sprite = transform.GetChild(0);
		agent = transform.GetChild(1).gameObject.GetComponent<NavMeshAgent>();
		obstacle = transform.GetChild(1).gameObject.GetComponent<NavMeshObstacle>();
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();

		var position = new Vector3(transform.position.x, transform.position.y, 0f);
		transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		sprite.position = position;
		agent.transform.position = position;
	}
	void Update()
	{
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

		var adjustedPosition = new Vector3(agent.transform.position.x, agent.transform.position.y, 0f);
		sprite.position = Vector3.Lerp(sprite.position, adjustedPosition, Time.deltaTime * 5);
	}
}
