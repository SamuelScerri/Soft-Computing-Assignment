using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField]
	private Vector3 WeaponDestination;

	[SerializeField]
	private byte WeaponSwayStrength;

	[SerializeField]
	private bool OffsetWeapon;

	private Transform WeaponTransform;
	private Transform CameraTransform;

	private Player Character;
	
	private Vector3 WeaponSway;
	private Vector3 WeaponOffset;

	private Vector3 WeaponSwayVelocity;
	private Vector3 WeaponOffsetVelocity;

	private Animator Animate;
	

	private void Start()
	{
		WeaponTransform = Camera.main.transform.GetChild(0).GetChild(0);
		CameraTransform = Camera.main.transform;

		Character = GetComponent<Player>();
		Animate = GetComponent<Animator>();
	}

	private void LateUpdate()
	{
		if (OffsetWeapon)
		{
			WeaponOffset = Vector3.SmoothDamp(WeaponOffset, -Character.GetVelocity() / 512, ref WeaponOffsetVelocity, .1f);
			WeaponTransform.localPosition = WeaponDestination + WeaponOffset;
		}
			
		WeaponSway = Vector3.SmoothDamp(WeaponSway, new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), Input.GetAxis("Mouse X")) * WeaponSwayStrength, ref WeaponSwayVelocity, .1f);
		WeaponTransform.eulerAngles = CameraTransform.eulerAngles + WeaponSway;

		if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
			Animate.SetBool("IsMoving", true);
		else Animate.SetBool("IsMoving", false);

		Animate.speed = Character.GetVelocity().magnitude / 8;
	}
}
