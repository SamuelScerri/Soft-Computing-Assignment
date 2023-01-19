using UnityEngine;

public class AudioScaler : MonoBehaviour
{
	private AudioSource[] _sources;

	private void Start()
	{
		_sources = GetComponents<AudioSource>();
	}

	private void Update()
	{
		foreach (AudioSource source in _sources)
			source.pitch = Time.timeScale;
	}
}
