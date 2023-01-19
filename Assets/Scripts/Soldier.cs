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

	private Coroutine _currentCoroutine;
	private float _agentSpeed;

	//This Is Used For Smoothing The Layer Weight
	private float _animationDampReference;

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
		_agentSpeed = _agent.speed;

		_currentCoroutine = StartCoroutine(Patrol(_waypoints[0]));
	}

	private void Update()
	{
		_animator.SetFloat("Slow Speed", _agent.velocity.magnitude / SlowAnimationSpeed);
		_animator.SetFloat("Fast Speed", _agent.velocity.magnitude / FastAnimationSpeed);

		_animator.SetLayerWeight(1,
			Mathf.SmoothDamp(_animator.GetLayerWeight(1), _animator.GetFloat("Slow Speed"), ref _animationDampReference, .2f));

		switch (_soldierMode)
		{
			//When The Soldier Finds The Player, Start Attacking
			case SoldierMode.Patrol:
				if (DetectObject(GameObject.FindWithTag("Player").transform.position))
					SwitchMode(Attack(GameObject.FindWithTag("Player").transform));
				break;

			//When The Soldier Doesn't See The Player Anymore, Go To Its Last Known Position
			case SoldierMode.Attack:
				if (!DetectObject(GameObject.FindWithTag("Player").transform.position))
					SwitchMode(Patrol(GameObject.FindWithTag("Player").transform));
				break;
		}
	}


	//This Is Responsible For The Guard Patrolling The Area, It Goes To Its Dedicated Waypoint & Stays There For 1 Second
	private IEnumerator Patrol(Transform initialWaypoint)
	{
		_soldierMode = SoldierMode.Patrol;
		_agent.SetDestination(initialWaypoint.position);

		ChangeSpeed(_patrolSpeed);

		while (true)
		{
			//Here We Iterate Over Every Waypoint, The Soldier Will Then Stay There For 1 Second And Then Proceed To The Next Waypoint
			foreach (Transform waypoint in _waypoints)
			{
				yield return new WaitUntil(() => HasArrived(_agent.destination));
				yield return new WaitForSeconds(1);

				_agent.SetDestination(waypoint.position);
			}			

			//We Yield Here To Avoid Crashing Unity
			yield return null;
		}
	}

	//The Enemy's Main Goal Here Is To Run Towards The Player
	private IEnumerator Attack(Transform waypoint)
	{
		_soldierMode = SoldierMode.Attack;
		ChangeSpeed(_attackSpeed);

		while (true)
		{
			_agent.SetDestination(waypoint.position);
			yield return null;
		}

		yield return null;
	}

	//This Is A Helper Function Used To Make Things More Readable
	private bool HasArrived(Vector3 waypoint)
	{
		return _agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.remainingDistance == 0 &&
			Vector3.Distance(transform.position, waypoint) < .1f;
	}

	//The Direction Is Calculated Between The Enemy And The Player, Then The Angle Is Calculated Between The Forward Direction & The Direction
	//If The Angle Is Less Than 30 Then The Player is In Range
	private bool DetectObject(Vector3 position)
	{
		if (!Physics.Linecast(transform.position, position))
		{
			Vector3 direction = (position - transform.position).normalized;
			return Vector3.Angle(transform.forward, direction) < 30;
		}

		else return false;
	}

	//This Will Stop The Coroutine To Ensure That No Previous Modes Will Conflict With The Current Mode
	private void SwitchMode(IEnumerator mode)
	{
		StopCoroutine(_currentCoroutine);
		_currentCoroutine = StartCoroutine(mode);
	}

	private void ChangeSpeed(float speed)
	{
		_agent.speed = _patrolSpeed;
		_agent.acceleration = _agent.speed / 2;
	}
}
