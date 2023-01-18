using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private static GameManager Singleton;

	public static GameObject Player;
	public static GameObject UIInstance;

	[SerializeField]
	private GameObject PrefabUI;

	private void Start()
	{
		if (Singleton)
			Destroy(this.gameObject);
		else
			Initialize();
	}

	private void Initialize()
	{
		Singleton = this;
		UIInstance = Instantiate(PrefabUI) as GameObject;
	}

	public static void SetUIText(string NewText)
	{
		UIInstance.transform.GetChild(0).GetComponent<Text>().text = "Ammo: " + NewText;
	}
}
