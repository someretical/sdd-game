using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
	private bool playerTouching = false;
	private bool timeoutRunning = false;
	private GameManager gameManager;
	void Start()
	{
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.gameObject.CompareTag("Player"))
			return;

		playerTouching = true;

		if (!timeoutRunning)
		{
			timeoutRunning = true;

			StartCoroutine(DelayedActivation());
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (!other.gameObject.CompareTag("Player"))
			return;

		playerTouching = false;
	}
	IEnumerator DelayedActivation()
	{
		yield return new WaitForSeconds(3f);

		if (playerTouching)
			gameManager.CreateNewLevel();
		else
			timeoutRunning = false;
	}
}
