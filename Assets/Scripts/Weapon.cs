using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[System.Serializable]
public class WeaponInformation
{
	public Vector3 WeaponDestination;
	public Mesh WeaponModel;
	public AudioClip WeaponSound;

	public bool Automatic;
	public float Delay, Spread;
	public byte Bullets;

	public WeaponInformation(Vector3 WeaponDestination, Mesh WeaponModel, AudioClip WeaponSound, bool Automatic, float Delay, float Spread, byte Bullets)
	{
		this.WeaponDestination = WeaponDestination;
		this.WeaponModel = WeaponModel;
		this.WeaponSound = WeaponSound;
		this.Automatic = Automatic;
		this.Delay = Delay;
		this.Spread = Spread;
		this.Bullets = Bullets;
	}
}
 
public class Weapon : MonoBehaviour
{
	[SerializeField]
	private List<WeaponInformation> Weapons;

	[SerializeField]
	private byte WeaponSwayStrength;

	[SerializeField]
	private GameObject BulletHole, Bullet;

	[SerializeField]
	private bool OffsetWeapon;

	private Vector3 WeaponSway, WeaponSwayVelocity, WeaponOffsetVelocity;

	private Player Character;
	private GameObject WeaponTransform;
	private Animator Animate;
	private AudioSource Sound;

	[SerializeField]
	private byte TotalAmmo;

	private byte CurrentWeapon;
	private bool CanFire = true;

	private void Start()
	{
		WeaponTransform = Camera.main.transform.GetChild(0).GetChild(0).gameObject;

		Character = GetComponent<Player>();
		Animate = GetComponent<Animator>();
		Sound = WeaponTransform.GetComponent<AudioSource>();

		GameManager.SetUIText(TotalAmmo.ToString());

		SwitchWeapon(0);
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && CanFire && TotalAmmo > 0)
		{
			StartCoroutine(Muzzle());
			StartCoroutine(Fire());
		}
			
	}

	//The Late Update Is Responsible For Showing The Weapon, This Includes Weapon Swaying & Moving Animations
	private void LateUpdate()
	{
		WeaponSway = Vector3.SmoothDamp(WeaponSway, new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), Input.GetAxis("Mouse X")) * WeaponSwayStrength, ref WeaponSwayVelocity, .1f, Mathf.Infinity, Time.unscaledDeltaTime);
		WeaponTransform.transform.eulerAngles = Camera.main.transform.eulerAngles + WeaponSway;

		Animate.SetBool("IsMoving", Character.IsGrounded() ? (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) : false);
		Animate.SetFloat("Speed", Character.GetVelocity().magnitude / 3);
	}

	private void SwitchWeapon(byte Index)
	{
		WeaponTransform.transform.GetChild(0).GetComponent<MeshFilter>().mesh = Weapons[Index].WeaponModel;
		Sound.clip = Weapons[Index].WeaponSound;

		CurrentWeapon = 0;
	}

	private IEnumerator Fire()
	{
		CanFire = false;

		Animate.SetTrigger("Fire");

		Vector3 ForwardVector = Camera.main.transform.TransformDirection(Vector3.forward);

		for (byte I = 0; I < Mathf.Clamp(Weapons[CurrentWeapon].Bullets, 0, TotalAmmo); I++)
		{
			Vector3 BulletSpread = new Vector3(Random.Range(-Weapons[CurrentWeapon].Spread, Weapons[CurrentWeapon].Spread),
				Random.Range(-Weapons[CurrentWeapon].Spread, Weapons[CurrentWeapon].Spread),
				Random.Range(-Weapons[CurrentWeapon].Spread, Weapons[CurrentWeapon].Spread));
			Instantiate(Bullet, Camera.main.transform.position, Camera.main.transform.rotation * Quaternion.Euler(BulletSpread));

			TotalAmmo--;

			GameManager.SetUIText(TotalAmmo.ToString());
		}


		Sound.Play();

		yield return new WaitForSeconds(Weapons[CurrentWeapon].Delay);

		CanFire = true;
	}

	private IEnumerator Muzzle()
	{
		transform.GetChild(1).gameObject.SetActive(true);
		yield return new WaitForSeconds(.04f);
		transform.GetChild(1).gameObject.SetActive(false);
	}

	public void AddAmmo(byte Amount)
	{
		TotalAmmo += Amount;
	}
}
