using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using TMPro;
using UnityEngine;
public class GameManager : MonoBehaviour
{
	[Header("Level manager")]
	public GameObject levelManager;
	[Space]
	[Header("JSON files")]
	public TextAsset defaultRoomsJSON;
	public TextAsset entranceRoomsJSON;
	public TextAsset exitRoomsJSON;
	public TextAsset intersectionRoomsJSON;
	public TextAsset secretRoomsJSON;
	public TextAsset treasureRoomsJSON;
	public TextAsset shopRoomsJSON;
	[HideInInspector]
	public RoomManager roomManager = new RoomManager();
	[HideInInspector]
	public int levelCounter = 0;
	[Space]
	[Header("Scaling")]
	public float levelSizeIncreasePercentage = 0.2f;
	public float maxRoomIncreasePercentage = 0.4f;
	public float itemPriceIncreasePercentage = 0.4f;
	public float enemyHealthIncreasePercentage = 0.2f;
	[Space]
	[Header("Player info")]
	public int maxHp = 6;
	public int hp = 6;
	public int mana = 100;
	public int armour = 0;
	public int coins = 0;
	public int blanks = 2;
	public int keys = 2;
	public int floorsCleared = 0;
	public int enemyKills = 0;
	public int totalCoins = 0;
	public int totalSpentCoins = 0;
	public int totalSecretRoomsRevealed = 0;
	public float timeAlive = 0f;
	[HideInInspector]
	public bool counting = false;
	[Space]
	[Header("Helper references")]
	private GameObject currentLevelManager;
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		roomManager.Init(this);

		CreateNewLevel();
	}
	void Update()
	{
		CheckHealth();
		IncrementTime();
	}
	void IncrementTime()
	{
		if (counting)
			timeAlive += Time.deltaTime;
	}
	public void CheckHealth()
	{
		if (hp == 0)
		{
			counting = false;

			var time = TimeSpan.FromSeconds(timeAlive);
			currentLevelManager.GetComponent<LevelManager>().stats.text = $@"Floors cleared: {floorsCleared}
Enemy kills: {enemyKills}
Coins collected: {totalCoins}
Coins spent: {totalSpentCoins}
Secret rooms revealed: {totalSecretRoomsRevealed}
Time alive: {string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds)}";

			currentLevelManager.GetComponent<LevelManager>().deathScreen.SetActive(true);
		}
	}
	public void CreateNewLevel()
	{
		Destroy(currentLevelManager);

		// Player starts every new floor with at least 2 blanks
		++levelCounter;
		if (blanks < 2)
			blanks = 2;

		currentLevelManager = Instantiate(levelManager, Vector3.zero, Quaternion.identity, transform);
	}
}
