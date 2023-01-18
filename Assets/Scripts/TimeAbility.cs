using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeAbility : MonoBehaviour
{
	[SerializeField]
	private byte Length;

	[SerializeField]
	private float Scale;

	private float CurrentLength;
	private bool Enabled;

	private void Start()
	{
		CurrentLength = Length;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt))
		{
			if (Enabled)
				DisableAbility();
			else
				EnableAbility();
		}

		if (Enabled)
			CurrentLength -= Time.unscaledDeltaTime;
		else CurrentLength += Time.unscaledDeltaTime;
		
		if (CurrentLength <= 0 || Input.GetMouseButton(0))
			DisableAbility();
		
		CurrentLength = Mathf.Clamp(CurrentLength, 0, Length);
	}

	private void EnableAbility()
	{
		Enabled = true;
		Time.timeScale = Scale;
	}

	private void DisableAbility()
	{
		Enabled = false;
		Time.timeScale = 1;
	}
}
