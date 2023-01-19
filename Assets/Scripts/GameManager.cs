using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Singleton;
	public static bool Paused;

	[SerializeField]
	private GameObject PrefabUI;

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
		Paused = Paused ? false : true;
		Time.timeScale = Paused ? 0 : 1;

		Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;
	}
}
