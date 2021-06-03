using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
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
	}
	public void CheckHealth()
	{
		if (hp == 0)
		{
			// End game or SOMETHING???
			Debug.Log("The player is supposed to be dead right now lmao");
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
