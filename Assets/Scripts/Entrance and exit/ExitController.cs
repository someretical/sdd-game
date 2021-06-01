using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
	private bool playerTouching = false;
	private bool timeoutRunning = false;
	private GameManager gameManager;
	private LevelManager levelManager;
	void Start()
	{
		gameManager = transform.parent.parent.parent.parent.gameObject.GetComponent<GameManager>();
		levelManager = transform.parent.parent.parent.gameObject.GetComponent<LevelManager>();
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
		{
			levelManager.transitioning = true;
			levelManager.ready = false;
			Cursor.visible = false;

			var start = new Color(levelManager.blackOut.color.r, levelManager.blackOut.color.g, levelManager.blackOut.color.b, 0f);
			var end = new Color(levelManager.blackOut.color.r, levelManager.blackOut.color.g, levelManager.blackOut.color.b, 1f);

			for (var t = 0f; t < 1f; t += Time.deltaTime)
			{
				var normalizedTime = t / 1f;

				levelManager.blackOut.color = Color.Lerp(start, end, normalizedTime);

				yield return null;
			}

			levelManager.blackOut.color = end;

			yield return new WaitForSeconds(1f);

			gameManager.CreateNewLevel();
		}
		else
			timeoutRunning = false;
	}
}
