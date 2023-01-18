using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FootstepNoise
{
	public int GroundLayer;
	public List<AudioClip> Sounds;

	public FootstepNoise(int GroundLayer, List<AudioClip> Sounds)
	{
		this.GroundLayer = GroundLayer;
		this.Sounds = Sounds;
	}
}

public class Footstep : MonoBehaviour
{
	private AudioSource Sound;

	[SerializeField]
	private List<FootstepNoise> Footsteps;
	private FootstepNoise CurrentFootstepNoise;

	private Coroutine FootstepCoroutine;
	private CharacterController CC;

	private Player PlayerInformation;

	private float StepTime;

	private void Start()
	{
		PlayerInformation = GetComponent<Player>();
		CC = GetComponent<CharacterController>();
		Sound = GetComponent<AudioSource>();

		CurrentFootstepNoise = Footsteps[0];
	}

	private void Update()
	{
		if (CC.isGrounded && StepTime <= 0) Step();
		else if (!CC.isGrounded) StepTime = -1;
		
		if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
			StepTime = Mathf.MoveTowards(StepTime, 0, PlayerInformation.GetCurrentSpeed().magnitude * Time.unscaledDeltaTime);
	}

	private void Step()
	{
		Sound.clip = CurrentFootstepNoise.Sounds[Random.Range(0, CurrentFootstepNoise.Sounds.Count)];

		Sound.Play();
		StepTime = 1;
	}

	private void OnControllerColliderHit(ControllerColliderHit Information)
	{
		foreach(FootstepNoise F in Footsteps)
			if (F.GroundLayer == Information.gameObject.layer)
				CurrentFootstepNoise = F;
	}
}
