using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SoldierMode
{
	Patrol,
	Search,
	Attack,
};

public class Soldier : MonoBehaviour
{
	//This Is Used To Sync The Animation With The AI's Speed
	private const float SlowAnimationSpeed = 0.976f;
	private const float FastAnimationSpeed = 2.921f;

	[SerializeField]
	private Transform[] _waypoints;
	private SoldierMode _soldierMode;

	[SerializeField]
	private float _patrolSpeed, _attackSpeed;
	
	//Built-In Components
	private NavMeshAgent _agent;
	private Animator _animator;
	private float _agentSpeed;

	//This Is Used For Smoothing The Layer Weight
	private float _animationDampReference;

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
		_agentSpeed = _agent.speed;

		StartCoroutine(Patrol(_waypoints[0]));
		ChangeSpeed(_patrolSpeed);
	}

	private void Update()
	{
		_animator.SetFloat("Slow Speed", _agent.velocity.magnitude / SlowAnimationSpeed);
		_animator.SetFloat("Fast Speed", _agent.velocity.magnitude / FastAnimationSpeed);

		_animator.SetLayerWeight(1,
			Mathf.SmoothDamp(_animator.GetLayerWeight(1),
				_soldierMode == SoldierMode.Patrol ? _animator.GetFloat("Slow Speed") : _animator.GetFloat("Fast Speed"), ref _animationDampReference, .2f));

		GetComponent<Footstep>().UpdateFootstep(_agent.velocity.magnitude);

		switch (_soldierMode)
		{
			//When The Soldier Finds The Player, Start Attacking
			case SoldierMode.Patrol:
				_animator.SetTrigger("Walk");

				if (DetectObject(GameObject.FindWithTag("Player").transform.position, 45))
				{
					//Call The Soldiers To The Player's Location
					RequestSupport(GameObject.FindWithTag("Player").transform);
					SwitchMode(Attack(GameObject.FindWithTag("Player").transform));
				}

				break;

			//Similar To Patrol Mode, But Won't Request Support
			case SoldierMode.Search:
				_animator.SetTrigger("Run");

				if (DetectObject(GameObject.FindWithTag("Player").transform.position, 45))
					SwitchMode(Attack(GameObject.FindWithTag("Player").transform));
				break;

			//When The Soldier Doesn't See The Player Anymore, Go To Its Last Known Position
			case SoldierMode.Attack:
				_animator.SetTrigger("Run");

				if (!DetectObject(GameObject.FindWithTag("Player").transform.position, 0))
					SwitchMode(Search(GameObject.FindWithTag("Player").transform));
				break;
		}
	}


	//This Is Responsible For The Guard Patrolling The Area, It Goes To Its Dedicated Waypoint & Stays There For 1 Second
	private IEnumerator Patrol(Transform initialWaypoint)
	{
		_soldierMode = SoldierMode.Patrol;
		_agent.SetDestination(initialWaypoint.position);

		_agent.stoppingDistance = 0;
		ChangeSpeed(_patrolSpeed);
		
		while (true)
		{
			//Here We Iterate Over Every Waypoint, The Soldier Will Then Stay There For 1 Second And Then Proceed To The Next Waypoint
			foreach (Transform waypoint in _waypoints)
			{
				if (!HasArrived(_agent.destination))
				{
					yield return new WaitUntil(() => HasArrived(_agent.destination));
					yield return new WaitForSeconds(1);
					GetComponent<SoldierCallout>().Patrol();
				}

				_agent.SetDestination(waypoint.position);				
			}			

			//We Yield Here To Avoid Crashing Unity
			yield return new WaitForSeconds(.1f);
		}
	}

	//The Enemy's Main Goal Here Is To Run Towards The Player
	private IEnumerator Attack(Transform waypoint)
	{
		_soldierMode = SoldierMode.Attack;
		ChangeSpeed(_attackSpeed);

		_agent.stoppingDistance = 4;

		while (true)
		{
			_agent.SetDestination(waypoint.position);
			yield return new WaitForSeconds(.1f);
		}
	}

	//The Enemy Will Go To The Called Out Position & Guard The Place For 1 Second
	public IEnumerator Search(Transform position)
	{
		_soldierMode = SoldierMode.Search;

		//yield return new WaitForSeconds(2);
		//GetComponent<SoldierCallout>().Acknowledge();
		ChangeSpeed(_attackSpeed);
		_agent.stoppingDistance = 2;

		_agent.SetDestination(position.position);

		while (true)
		{
			yield return new WaitUntil(() => HasArrived(_agent.destination));
			yield return new WaitForSeconds(2);

			GetComponent<SoldierCallout>().Clear();

			yield return new WaitForSeconds(1);
			SwitchMode(Patrol(_waypoints[0]));
		}
	}

	public IEnumerator Acknowledge(Transform position)
	{
		_soldierMode = SoldierMode.Search;
		_agent.ResetPath();

		yield return new WaitForSeconds(2);
		GetComponent<SoldierCallout>().Acknowledge();

		SwitchMode(Search(position));
	}

	//This Is A Helper Function Used To Make Things More Readable
	private bool HasArrived(Vector3 waypoint)
	{
		return _agent.pathStatus == NavMeshPathStatus.PathComplete &&
			Vector3.Distance(transform.position, waypoint) < _agent.stoppingDistance * 2 + .1f;
	}

	//The Direction Is Calculated Between The Enemy And The Player, Then The Angle Is Calculated Between The Forward Direction & The Direction
	//If The Angle Is Less Than 30 Then The Player is In Range
	private bool DetectObject(Vector3 position, float angle)
	{
		Debug.DrawLine(transform.position + Vector3.up * 2, position + Vector3.up * 2);

		if (!Physics.Linecast(transform.position + Vector3.up * 2, position + Vector3.up * 2))
		{
			Vector3 direction = ((position + Vector3.up * 2) - (transform.position + Vector3.up * 2)).normalized;

			if (angle == 0)
				return true;
			else return Vector3.Angle(transform.forward, direction) < angle;
		}

		else return false;
	}

	//The Soldier Will Call Out Their Allies And Make Them Go To The Player's Last Position
	private void RequestSupport(Transform position)
	{
		_animator.SetTrigger("Wave");

		GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
		GetComponent<SoldierCallout>().Support();

		foreach (GameObject soldier in soldiers)
		{
			Soldier behaviour = soldier.GetComponent<Soldier>();
			behaviour.SwitchMode(behaviour.Acknowledge(position));
		}
	}

	//This Will Stop The Coroutine To Ensure That No Previous Modes Will Conflict With The Current Mode
	private void SwitchMode(IEnumerator mode)
	{
		StopAllCoroutines();
		StartCoroutine(mode);
	}

	private void ChangeSpeed(float speed)
	{
		_agent.speed = speed;
		_agent.acceleration = speed;
	}
}
