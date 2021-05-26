using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManager : MonoBehaviour
{
	public bool ready = false;
	public GameObject player;
	public GameObject dungeonManager;
	void Awake()
	{
		var _player = Instantiate(player);
		_player.transform.parent = transform;

		var _dungeonManager = Instantiate(dungeonManager);
		_dungeonManager.transform.parent = transform;
	}
	void Update()
	{
		// Update will only run when all awakes and starts have been called
		if (!ready)
			ready = true;
	}
}
