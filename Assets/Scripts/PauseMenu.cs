using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenuUI;
	public LevelManager levelManager;
	public bool paused = false;
	void Update()
	{
		if (levelManager.ready && Input.GetButtonDown("Pause"))
			if (paused)
				Resume();
			else
				Pause();
	}
	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		paused = false;
	}
	public void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		paused = true;
	}
	public void NewGame()
	{
		// Basically reload current scene
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		Time.timeScale = 1f;
	}
	public void QuitGame()
	{
		Application.Quit();
	}
}
