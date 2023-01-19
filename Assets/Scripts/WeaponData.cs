using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Weapon/Create New Weapon", order = 1)]
public class WeaponData : ScriptableObject
{
	public Mesh Model;
	public AudioClip Sound;

	public float Rotation;
	public bool Automatic;
	public byte BulletsAmount;
	public float Delay, Spread;
}