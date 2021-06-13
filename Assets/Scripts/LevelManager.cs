using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	[HideInInspector]
	public bool ready = false;
	[HideInInspector]
	public bool transitioning = false;
	[Header("Helper references")]
	public Image blackOut;
	public TextMeshProUGUI levelText;
	public CameraController cam;
	public TextMeshProUGUI stats;
	public GameObject deathScreen;
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		levelText.text = $"Floor {transform.parent.GetComponent<GameManager>().levelCounter}";
	}
	IEnumerator RemoveTransitionComponents()
	{
		// Remove blackout
		transitioning = true;
		yield return new WaitForSeconds(1f);

		StartCoroutine(DisplayFloorText());
		//RGB colours are set based on the image "blackOut"
		var start = new Color(blackOut.color.r, blackOut.color.g, blackOut.color.b, 1f);
		var end = new Color(blackOut.color.r, blackOut.color.g, blackOut.color.b, 0f);

		for (var t = 0f; t < 1f; t += Time.deltaTime)
		{
			//The colour of blackOut smoothly transitions out to fade using lerp.
			blackOut.color = Color.Lerp(start, end, t);

			yield return null;
		}

		blackOut.color = end;
		blackOut.gameObject.SetActive(false);
	}
	IEnumerator DisplayFloorText()
	{
		// Fade in floor # text
		var start = new Color32(255, 255, 255, 0);
		var end = new Color32(255, 255, 255, 255);

		for (var t = 0f; t < 0.4f; t += Time.deltaTime)
		{
			// levelText.color = Color.Lerp(start, end, t / 0.4); - unoptimised
			// Compiler probably optimises to t * 2.5 anyway
			// but doesn't hurt to optimise it yourself if you easily can

			levelText.color = Color.Lerp(start, end, t * 2.5f);

			yield return null;
		}

		levelText.color = end;

		yield return new WaitForSeconds(2f);

		// Fade out floor # text
		start = new Color32(255, 255, 255, 255);
		end = new Color32(255, 255, 255, 0);

		for (var t = 0f; t < 0.4f; t += Time.deltaTime)
		{
			levelText.color = Color.Lerp(start, end, t * 2.5f);

			yield return null;
		}

		levelText.color = end;
		levelText.gameObject.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		ready = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	void Update()
	{
		//Constantly checks every frame if it is NOT ready and NOT transitioning, then it will run the coroutine.
		if (!ready && !transitioning)
		{
			cam.UpdateHUD();

			StartCoroutine(RemoveTransitionComponents());
		}
	}
}
