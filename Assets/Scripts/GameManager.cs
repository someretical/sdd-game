using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public MapManager mapScript;
	private int level = 0;
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		mapScript = GetComponent<MapManager>();

		InitGame();
	}
	void InitGame()
	{
		mapScript.SetupMap(level);
	}
}
