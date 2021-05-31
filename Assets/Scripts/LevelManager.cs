using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	public bool ready = false;
	public bool transitioning = false;
	public GameObject player;
	public GameObject mainCamera;
	public GameObject dungeonManager;
	public GameObject navMesh;
	public Image blackOut;
	private GameObject cam;
	private TextMeshProUGUI levelText;
	private string label;
	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;

		Instantiate(player, Vector3.zero, Quaternion.identity, transform);
		cam = Instantiate(mainCamera, Vector3.zero, Quaternion.identity, transform);
		Instantiate(dungeonManager, Vector3.zero, Quaternion.identity, transform);
		Instantiate(navMesh, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), transform);

		blackOut = cam.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
		levelText = cam.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
	}
	void Start()
	{
		levelText.text = $"Floor {transform.parent.gameObject.GetComponent<GameManager>().levelCounter}";
	}
	IEnumerator RemoveTransitionComponents()
	{
		transitioning = true;

		yield return new WaitForSeconds(1f);

		var start = new Color(blackOut.color.r, blackOut.color.g, blackOut.color.b, 1f);
		var end = new Color(blackOut.color.r, blackOut.color.g, blackOut.color.b, 0f);

		for (var t = 0f; t < 1f; t += Time.deltaTime)
		{
			var normalizedTime = t / 1f;

			blackOut.color = Color.Lerp(start, end, normalizedTime);

			yield return null;
		}

		blackOut.color = end;

		ready = true;
		Cursor.lockState = CursorLockMode.None;

		yield return new WaitForSeconds(0.5f);

		start = new Color32(255, 255, 255, 0);
		end = new Color32(255, 255, 255, 255);

		for (var t = 0f; t < 0.3f; t += Time.deltaTime)
		{
			var normalizedTime = t / 0.3f;

			levelText.color = Color.Lerp(start, end, normalizedTime);

			yield return null;
		}

		levelText.color = end;

		yield return new WaitForSeconds(2f);

		start = new Color32(255, 255, 255, 255);
		end = new Color32(255, 255, 255, 0);

		for (var t = 0f; t < 0.3f; t += Time.deltaTime)
		{
			var normalizedTime = t / 0.3f;

			levelText.color = Color.Lerp(start, end, normalizedTime);

			yield return null;
		}

		levelText.color = end;
		transitioning = false;
	}
	void Update()
	{
		if (!ready && !transitioning)
			StartCoroutine(RemoveTransitionComponents());
	}
}
