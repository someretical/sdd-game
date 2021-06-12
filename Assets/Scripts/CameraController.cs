using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
	public float radius;
	[Header("Helper references")]
	public Transform player;
	public Camera cam;
	public TextMeshProUGUI playerData;
	public Text version;
	private GameManager gameManager;
	void Start()
	{
		gameManager = transform.parent.parent.GetComponent<GameManager>();
		version.text = $"Version {Application.version}";
	}
	void LateUpdate()
	{
		// This piece of fancy code offets the camera location
		// to 10% of the distance and direction from the player to the cursor
		// This makes the game feel more dynamic I think
		var mouseOffset = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - cam.transform.position.z)) - player.position;
		var mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -transform.position.z));

		transform.position = new Vector3(
			(mousePos.x - player.position.x) * 0.1f + player.position.x,
			(mousePos.y - player.position.y) * 0.1f + player.position.y,
			transform.position.z
		);

		// Should have max radius that the camera should stay WITHIN
		var dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y));
		if (dist > radius)
		{
			var norm = mouseOffset.normalized;
			transform.position = new Vector3(norm.x * radius + player.position.x, norm.y * radius + player.position.y, transform.position.z);
		}
	}
	public void UpdateHUD()
	{
		// Indentation broken here because of how multi line strings work in c#
		// If I wanted to fix this, I would have to create a helper function to:
		// 1) Detect the number of tabs of indentation on the first newline
		// 2) Strip that same number of tabs from all following lines
		// as to not disturb any 'intended' indentation
		// But this project has a budget of exactly zero dollars

		playerData.text = $@"Health: {gameManager.hp}/{gameManager.maxHp}
Armour: {gameManager.armour}
Blanks: {gameManager.blanks}
Keys: {gameManager.keys}
Coins: {gameManager.coins}
";
	}
}
