using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
	private GameManager gameManager;
	void Start()
	{
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
			gameManager.CreateNewLevel();
	}
}
