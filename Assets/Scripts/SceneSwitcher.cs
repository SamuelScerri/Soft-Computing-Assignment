using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	public void QuitGame()
	{
		Application.Quit();
	}

	public void StartGame()
	{
		SceneManager.LoadScene(1);
	}

	public void EasyDifficulty()
	{
		GameManager.DifficultyLevel = 1;
		SceneManager.LoadScene(2);
	}

	public void MediumDifficulty()
	{
		GameManager.DifficultyLevel = 2;
		SceneManager.LoadScene(2);
	}

	public void HardDifficulty()
	{
		GameManager.DifficultyLevel = 3;
		SceneManager.LoadScene(2);
	}
}
