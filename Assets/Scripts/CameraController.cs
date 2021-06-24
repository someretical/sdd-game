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
	public Text version;
	[Space]
	[Header("Health bar")]
	public TextMeshProUGUI hpText;
	public Image hpImage;
	public Slider hpSlider;
	public Gradient hpGradient;
	private GameManager gameManager;
	[Space]
	[Header("Armour")]
	public TextMeshProUGUI armour;
	[Space]
	[Header("Blanks")]
	public TextMeshProUGUI blanks;
	[Space]
	[Header("Keys")]
	public TextMeshProUGUI keys;
	[Space]
	[Header("Coins")]
	public TextMeshProUGUI coins;
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
		UpdateHP();
		UpdateArmour();
		UpdateBlanks();
		UpdateKeys();
		UpdateCoins();
	}
	public void UpdateHP()
	{
		hpText.text = $"{gameManager.hp}/{gameManager.maxHp}";
		hpSlider.maxValue = gameManager.maxHp;
		hpSlider.value = gameManager.hp;
		hpImage.color = hpGradient.Evaluate(hpSlider.normalizedValue);
	}
	public void UpdateArmour()
	{
		armour.text = gameManager.armour.ToString();
	}
	public void UpdateBlanks()
	{
		blanks.text = gameManager.blanks.ToString();
	}
	public void UpdateKeys()
	{
		keys.text = gameManager.keys.ToString();
	}
	public void UpdateCoins()
	{
		coins.text = gameManager.coins.ToString();
	}
}
