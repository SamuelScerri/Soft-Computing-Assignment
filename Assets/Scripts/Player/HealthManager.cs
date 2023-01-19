using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
	public void Kill()
	{
		Debug.Log("Killing");

		foreach (Component component in GetComponents<MonoBehaviour>())
			if (component != this)
				Destroy(component);
	}
}
