using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField]
	private byte Speed;

	private void Start()
	{
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * Speed, ForceMode.VelocityChange);
	}

	private void OnCollisionEnter(Collision Information)
	{
		if (Information.gameObject.tag == "Soldier")
			Information.gameObject.GetComponent<SoldierOld>().Damage(1);
		else if (Information.gameObject.tag == "Player")
			Information.gameObject.GetComponent<Player>().Kill();

		Debug.Log("Hit");

		Destroy(this.gameObject);
	}
}
