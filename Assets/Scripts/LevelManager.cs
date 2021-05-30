using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManager : MonoBehaviour
{
	public bool ready = false;
	public GameObject player;
	public GameObject mainCamera;
	public GameObject dungeonManager;
	public GameObject navMesh;
	void Awake()
	{
		Instantiate(player, Vector3.zero, Quaternion.identity, transform);
		Instantiate(mainCamera, Vector3.zero, Quaternion.identity, transform);
		Instantiate(dungeonManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(navMesh, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), transform);
	}
	void Update()
	{
		// Update will only run when all awakes and starts have been called
		if (!ready)
			ready = true;
	}
}
