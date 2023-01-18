using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Modes
{
	Patrolling,
	Staying,
	Searching,
	Attacking
};

public class Soldier : MonoBehaviour
{
	[SerializeField]
	public List<Transform> Waypoint;

	[SerializeField]
	private AudioClip Patrolling, Staying, Searching, Attacking, Dying;

	[SerializeField]
	private byte PatrollingSpeed, AttackingSpeed;

	[SerializeField]
	private byte Health;

	[SerializeField]
	private Modes CurrentMode;

	[SerializeField]
	private byte AttackDistance;

	[SerializeField]
	private GameObject BulletPrefab;

	[SerializeField]
	private float TimeBetweenFire;

	[SerializeField]
	private byte BulletsEveryShot;

	[SerializeField]
	private float BulletSpread;

	private byte CurrentWaypointIndex;

	private NavMeshAgent Agent;
	private Animator Animate;
	private AudioSource Source;

	private AudioSource VoiceSource;

	private Coroutine FireCoroutine;
	private Coroutine DeathCoroutine;

	//This Is The Actual Speed Of The Animation
	private const float WalkAnimationSpeed = 0.976f;
	private const float RunAnimationSpeed = 2.921f;
	private const float StrafeAnimationSpeed = 3.189f;

	private float SmoothWeight;
	private float DeathTransition;

	private void Start()
	{
		Agent = GetComponent<NavMeshAgent>();
		Animate = GetComponent<Animator>();

		AudioSource[] Sources = GetComponents<AudioSource>();
		Source = Sources[0];
		VoiceSource = Sources[1];

		Agent.SetDestination(Waypoint[0].position);

		Animate.SetTrigger(CurrentMode == Modes.Patrolling ? "Walk" : "Run");
	}

	private void Update()
	{
		if (DeathCoroutine == null)
			SoldierUpdate();
		else
		{
			Animate.SetLayerWeight(3, Mathf.SmoothDamp(Animate.GetLayerWeight(3), 1, ref DeathTransition, .2f));
		}
	}

	private void SoldierUpdate()
	{
		if (CurrentMode == Modes.Attacking)
		{
			Agent.SetDestination(GameManager.Player.transform.position);
			Agent.stoppingDistance = AttackDistance;

			if (Vector3.Distance(transform.position, GameManager.Player.transform.position) < AttackDistance + 1)
			{
				if (FireCoroutine == null)
				{
					StartCoroutine(Muzzle());
					FireCoroutine = StartCoroutine(Fire());
				}
					

				Vector3 LookPosition = GameManager.Player.transform.position - transform.position;
				LookPosition.y = 0;

				Quaternion Rotation = Quaternion.LookRotation(LookPosition);
				transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, Time.deltaTime * 4);
			}
				
		}
			
		else if (Agent.pathStatus == NavMeshPathStatus.PathComplete && Agent.remainingDistance == 0 && CurrentMode != Modes.Staying)
			StartCoroutine(GuardPosition(4));

		Animate.SetFloat("Walk Speed", Agent.velocity.magnitude / WalkAnimationSpeed);
		Animate.SetFloat("Run Speed", Agent.velocity.magnitude / RunAnimationSpeed);
		Animate.SetLayerWeight(1, Mathf.SmoothDamp(Animate.GetLayerWeight(1), CurrentMode == Modes.Patrolling ? Animate.GetFloat("Walk Speed") : Animate.GetFloat("Run Speed"), ref SmoothWeight, .2f));

		Agent.speed = CurrentMode == Modes.Patrolling ? PatrollingSpeed : AttackingSpeed;
		Agent.acceleration = Agent.speed;


	}

	public void InvestigateDistraction(Vector3 Position)
	{
		Agent.SetDestination(Position);
		CurrentMode = Modes.Searching;
		VoiceSource.clip = Searching;
		VoiceSource.Play();
	}

	private IEnumerator GuardPosition(float Time)
	{
		CurrentMode = Modes.Staying;
		VoiceSource.clip = Staying;
		VoiceSource.Play();

		Animate.SetTrigger("Stop");

		yield return new WaitForSeconds(Time);

		CurrentWaypointIndex++;

		if (CurrentWaypointIndex >= Waypoint.Count)
			CurrentWaypointIndex = 0;

		VoiceSource.clip = Patrolling;
		VoiceSource.Play();
		Agent.SetDestination(Waypoint[CurrentWaypointIndex].position);

		CurrentMode = Modes.Patrolling;
		Animate.SetTrigger("Walk");
	}

	public void Damage(byte Amount)
	{
		if (Health == 0)
		{
			if (DeathCoroutine == null)
				DeathCoroutine = StartCoroutine(CleanObject());
		}

			
		else
		{
			Health -= Amount;
			Animate.SetTrigger("Hit");
		}
	}

	private IEnumerator Fire()
	{
		Animate.SetTrigger("Fire");
		Source.Play();

		for (byte I = 0; I < BulletsEveryShot; I++)
		{
			Vector3 Spread = new Vector3(Random.Range(-BulletSpread, BulletSpread), Random.Range(-BulletSpread, BulletSpread), Random.Range(-BulletSpread, BulletSpread));
			Instantiate(BulletPrefab, transform.position + Vector3.up * 1.8f, transform.rotation * Quaternion.Euler(Spread));
		}
		

		yield return new WaitForSeconds(TimeBetweenFire);

		FireCoroutine = null;
	}

	private IEnumerator Muzzle()
	{
		transform.GetChild(0).gameObject.SetActive(true);
		yield return new WaitForSeconds(.04f);
		transform.GetChild(0).gameObject.SetActive(false);
	}

	//This Will Basically Remove Any Component That Wouldn't Be Needed Anymore, Saving Memory
	private IEnumerator CleanObject()
	{
		Animate.SetTrigger("Death");
		VoiceSource.clip = Dying;
		VoiceSource.Play();

		Destroy(Agent);
		Destroy(Source);
		Destroy(GetComponent<AudioScaler>());
		Destroy(GetComponent<Collider>());

		yield return new WaitForSeconds(1);

		Destroy(this);
	}
}
