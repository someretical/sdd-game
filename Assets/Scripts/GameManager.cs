using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneratorNamespace
{
	public class GameManager : MonoBehaviour
	{
		public GameObject levelManager;
		public TextAsset defaultRoomsJSON;
		public TextAsset entranceRoomsJSON;
		public TextAsset exitRoomsJSON;
		public TextAsset intersectionRoomsJSON;
		public TextAsset secretRoomsJSON;
		public TextAsset treasureRoomsJSON;
		public RoomManager roomManager = new RoomManager();
		private GameObject currentLevelManager;
		void Start()
		{
			roomManager.Init(this);

			CreateLevel();
		}
		public void CreateLevel()
		{
			Destroy(currentLevelManager);

			currentLevelManager = Instantiate(levelManager);
			currentLevelManager.transform.SetParent(transform);
		}
	}
}
