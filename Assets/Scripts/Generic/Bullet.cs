using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField]
	private float _bulletSpeed;

	private TrailRenderer _trailRenderer;
	private AudioSource _source;
	private MeshRenderer _meshRenderer;

	private void Start()
	{
		_trailRenderer = GetComponent<TrailRenderer>();
		_source = GetComponent<AudioSource>();
		_meshRenderer = GetComponent<MeshRenderer>();

		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * _bulletSpeed, ForceMode.VelocityChange);
	}

	private void FixedUpdate()
	{
		//GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * (Time.timeScale != 1 ? _bulletSpeed : _bulletSpeed * 2) * Time.deltaTime);
		_trailRenderer.enabled = Time.timeScale != 1 ? true : false;
		_meshRenderer.enabled = Time.timeScale != 1 ? true : false;
		_source.enabled = Time.timeScale != 1 ? true : false;
	}

	private void OnCollisionEnter(Collision collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			GameManager.ShowDeathScreen();
			collider.gameObject.GetComponent<HealthManager>().Kill();
			Time.timeScale = 1;
			Destroy(this.gameObject);
		}

		if (collider.gameObject.tag == "Map")
			Destroy(this.gameObject);
	}
}
