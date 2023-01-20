using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierWeapon : MonoBehaviour
{
	[SerializeField]
	private WeaponData _weaponInformation;

	[SerializeField]
	private GameObject _bulletPrefab;

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

		transform.GetChild(2).GetChild(0).GetComponent<MeshFilter>().mesh = _weaponInformation.Model;
		transform.GetChild(2).GetChild(0).Rotate(Vector3.down * _weaponInformation.Rotation);

		StartCoroutine(Attack(GameObject.FindWithTag("Player").transform));
		transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
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
				if (GetComponent<Soldier>().DetectObject(player.position, 15))
				{
					for (byte i = 0; i < _weaponInformation.BulletsAmount; i++)
					{
						float spreadX = Random.Range(-_weaponInformation.Spread, _weaponInformation.Spread);
						float spreadY = Random.Range(-_weaponInformation.Spread, _weaponInformation.Spread);
						float spreadZ = Random.Range(-_weaponInformation.Spread, _weaponInformation.Spread);

						GameObject bullet = Instantiate(_bulletPrefab, transform.position + Vector3.up * 2, transform.rotation) as GameObject;
						bullet.transform.Rotate(new Vector3(spreadX, spreadY, spreadZ));
					}

					transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
					_animator.SetTrigger("Fire");
					_source.Play();

					yield return new WaitForSeconds(.1f);
					//transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
					transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
				}

				yield return new WaitForSeconds(_weaponInformation.Delay - .01f);
				transform.GetChild(0).gameObject.SetActive(false);
			}

			transform.GetChild(0).gameObject.SetActive(false);

			yield return null;
		}
	}
}
