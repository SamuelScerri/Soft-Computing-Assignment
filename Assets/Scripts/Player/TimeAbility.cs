using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeAbility : MonoBehaviour
{
	[SerializeField]
	private float _length;

	[SerializeField]
	private float _scale;

	private float _currentLength;
	private bool _enabled;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt))
		{
			if (_enabled)
				DisableAbility();
			else
				EnableAbility();
		}

		if (_enabled)
			_length -= Time.unscaledDeltaTime;
		
		if (_length <= 0)
			DisableAbility();

		GameManager.SetBulletTimeUI(Mathf.Clamp(_length, 0, Mathf.Infinity));
	}

	private void EnableAbility()
	{
		_enabled = true;
		Time.timeScale = _scale;
	}

	private void DisableAbility()
	{
		_enabled = false;
		Time.timeScale = 1;
	}

	public bool IsEnabled()
	{
		return _enabled;
	}
}
