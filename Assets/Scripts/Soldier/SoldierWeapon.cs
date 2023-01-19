using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierWeapon : MonoBehaviour
{
	[SerializeField]
	private WeaponData _weaponInformation;
	private Animator _animator;
	private AudioSource _source;
	private NavMeshAgent _agent;

	[SerializeField]
	private float _attackDistance;

	private float desiredRotation, smoothRotation;

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
		_source = GetComponents<AudioSource>()[2];
		_source.clip = _weaponInformation.Sound;

		StartCoroutine(Attack(GameObject.FindWithTag("Player").transform));
	}

	private void Update()
	{
		if (_agent.velocity.normalized.magnitude < .1f && GetComponent<Soldier>().GetSoldierMode() == SoldierMode.Attack)
			RotateUpdate(GameObject.FindWithTag("Player").transform);
	}

	private void RotateUpdate(Transform player)
	{
		desiredRotation = Quaternion.LookRotation(player.position - transform.position).eulerAngles.y;
		transform.eulerAngles = new Vector2(transform.eulerAngles.x,
			Mathf.SmoothDamp(transform.eulerAngles.y, desiredRotation, ref smoothRotation, .4f));
	}

	private IEnumerator Attack(Transform player)
	{
		while (true)
		{
			if (Vector3.Distance(transform.position, player.position) <= _attackDistance && GetComponent<Soldier>().GetSoldierMode() == SoldierMode.Attack)
			{
				if (GetComponent<Soldier>().DetectObject(player.position, 45))
				{
					_animator.SetTrigger("Fire");
					_source.Play();
				}

				yield return new WaitForSeconds(_weaponInformation.Delay);
			}

			yield return null;
		}
	}
}
