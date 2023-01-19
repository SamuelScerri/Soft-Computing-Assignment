using UnityEngine;

public class AudioScaler : MonoBehaviour
{
	private AudioSource Source;

	private void Start()
	{
		Source = GetComponent<AudioSource>();
	}

	private void Update()
	{
		Source.pitch = Time.timeScale;
	}
}
