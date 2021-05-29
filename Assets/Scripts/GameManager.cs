using System.Collections;
using System.Collections.Generic;
using DungeonGeneratorNamespace;
using UnityEngine;
public class GameManager : MonoBehaviour
{
	public GameObject levelManager;
	public TextAsset defaultRoomsJSON;
	public TextAsset entranceRoomsJSON;
	public TextAsset exitRoomsJSON;
	public TextAsset intersectionRoomsJSON;
	public TextAsset secretRoomsJSON;
	public TextAsset treasureRoomsJSON;
	public TextAsset shopRoomsJSON;
	public RoomManager roomManager = new RoomManager();
	public int levelCounter = 0;
	public float levelSizeIncreasePercentage = 0.2f;
	public float maxRoomIncreasePercentage = 0.4f;
	public float itemPriceIncreasePercentage = 0.4f;
	public float enemyHealthIncreasePercentage = 0.2f;
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

		++levelCounter;
		if (blanks < 2)
			blanks = 2;

		currentLevelManager = Instantiate(levelManager, Vector3.zero, Quaternion.identity, transform);
	}
}
