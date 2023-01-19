using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FootstepNoise
{
	public int GroundLayer;
	public List<AudioClip> _sources;

	public FootstepNoise(int groundLayer, List<AudioClip> _sources)
	{
		this.GroundLayer = groundLayer;
		this._sources = _sources;
	}
}

public class Footstep : MonoBehaviour
{
	private AudioSource _source;

	[SerializeField]
	private List<FootstepNoise> _footstepNoises;
	private FootstepNoise _currentFootstepNoise;

	private float _stepTime;

	private void Start()
	{
		_source = GetComponents<AudioSource>()[1];
		_currentFootstepNoise = _footstepNoises[0];
	}

	public void UpdateFootstep(float speed)
	{
		_stepTime -= speed * Time.deltaTime;
		if (_stepTime <= 0) Step();
	}

	private void Step()
	{
		_source.clip = _currentFootstepNoise._sources[Random.Range(0, _currentFootstepNoise._sources.Count)];

		_source.Play();
		_stepTime = 1;
	}
}
