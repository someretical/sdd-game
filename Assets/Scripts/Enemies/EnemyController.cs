using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public int weight;
	public int coinReward;
	public int health;
	public Camera cam;
	public NavMeshAgent agent;
	private PlayerController player;
	private Transform NOTNAVMESHGAMEOBJECT;
	void Start()
	{
		cam = transform.parent.parent.parent.GetChild(1).gameObject.GetComponent<Camera>();
		player = transform.parent.parent.parent.GetChild(0).gameObject.GetComponent<PlayerController>();

		// This fix literally took me half a day to find
		// Was actually driving me insane
		// All the pathfinding was working but the damn thing just wasn't displaying
		// Thankfully, I was saved by https://forum.unity.com/threads/top-down-sprite-with-the-navmeshagent-issues.219965/ 
		NOTNAVMESHGAMEOBJECT = transform.GetChild(0);
		NOTNAVMESHGAMEOBJECT.transform.position = new Vector3(NOTNAVMESHGAMEOBJECT.transform.position.x, NOTNAVMESHGAMEOBJECT.transform.position.y, 1f);
		NOTNAVMESHGAMEOBJECT.rotation = Quaternion.Euler(0f, 0f, 0f);

		agent = gameObject.GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}
	void Update()
	{
		agent.SetDestination(player.gameObject.transform.position);
	}
}
