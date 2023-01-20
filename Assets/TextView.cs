using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextView : MonoBehaviour
{
	private void Update()
	{
		if (Physics.Linecast(Camera.main.transform.position, transform.position))
			gameObject.SetActive(false);
		else gameObject.SetActive(true);
	}
}
