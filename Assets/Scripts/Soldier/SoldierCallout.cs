using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierCallout : MonoBehaviour
{
	[SerializeField]
	private AudioClip _patrolClip, _attackClip, _supportClip, _clearClip, _deathClip, _acknowledgeClip;
	private AudioSource _source;

	private void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	public void Patrol()
	{
		PlayClip(_patrolClip);
	}

	public void Attack()
	{
		PlayClip(_attackClip);
	}

	public void Clear()
	{
		PlayClip(_clearClip);
	}

	public void Acknowledge()
	{
		PlayClip(_acknowledgeClip);
	}


	public void Support()
	{
		PlayClip(_supportClip);
	}

	public void Death()
	{
		PlayClip(_deathClip);
	}

	private void PlayClip(AudioClip clip)
	{
		_source.clip = clip;
		_source.Play();
	}
}
