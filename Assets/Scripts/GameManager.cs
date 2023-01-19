using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Singleton;
	public static byte DifficultyLevel = 2;

	public bool Paused;

	private void Start()
	{
		if (Singleton != null)
			Destroy(this.gameObject);
		else
			Singleton = this;

		Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			GameManager.TogglePause();
	}

	private static void TogglePause()
	{
		Singleton.Paused = Singleton.Paused ? false : true;
		Time.timeScale = Singleton.Paused ? 0 : 1;

		Cursor.lockState = Singleton.Paused ? CursorLockMode.None : CursorLockMode.Locked;

		Singleton.transform.GetChild(4).gameObject.SetActive(Singleton.Paused ? true : false); 
	}

	public static void SetBulletTimeUI(float time)
	{
		Singleton.transform.GetChild(2).GetComponent<Text>().text = "Bullet Time: " + time.ToString();
	}

	public static void ShowDeathScreen()
	{
		Singleton.transform.GetChild(3).gameObject.SetActive(true);
	}

	public static void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
