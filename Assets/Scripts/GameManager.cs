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
		private readonly RoomManager roomManager = new RoomManager();
		private GameObject currentLevelManager;
		void Start()
		{
			roomManager.Init(this);

			currentLevelManager = Instantiate(levelManager, new Vector3Int(0, 0, 0), Quaternion.identity, transform);

			var dungeonManager = currentLevelManager.transform.Find("Dungeon").gameObject.GetComponent<DungeonManager>();
			dungeonManager.Init(roomManager);
		}
	}
}
