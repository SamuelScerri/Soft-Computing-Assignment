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
}