using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	private byte _walkSpeed, _runSpeed, _crouchSpeed, _fallSpeed, _jumpStrength, _standHeight, _crouchHeight;

	[SerializeField]
	private bool _stairSnap;

	[SerializeField]
	private float _acceleration;

	private CharacterController _characterController;

	private Vector3 _currentSpeed, _currentVelocity, _previousPosition;
	private float _gravity;

	private bool _lastGrounded, _isCrouched;

	private void Start()
	{
		//We Get The Character Controller And Move To The Nearest Ground
		_characterController = GetComponent<CharacterController>();
		_characterController.Move(Vector2.down * 9999);
	}

	private void Update()
	{
		//This Ensures That More Velocity Will Only Be In A Certain Direction, This Means That If You Look Behind You Whilst Moving, You Won't Continue Moving Forward But Rather Backwards As You Will Still Be Slowing Down
		Vector3 direction = (transform.forward * Input.GetAxisRaw("Vertical") +
			transform.right* Input.GetAxisRaw("Horizontal")).normalized;

		//Smooth Damp Will Ensure That The Acceleration Will Be Constant Across Different Framerates
		if (_isCrouched)
			_currentSpeed = Vector3.SmoothDamp(_currentSpeed, direction * _crouchSpeed, ref _currentVelocity, _acceleration, Mathf.Infinity, Time.deltaTime);
		else if (Input.GetKey(KeyCode.LeftShift) && _lastGrounded)
			_currentSpeed = Vector3.SmoothDamp(_currentSpeed, direction * _runSpeed, ref _currentVelocity, _acceleration, Mathf.Infinity, Time.deltaTime);
		else _currentSpeed = Vector3.SmoothDamp(_currentSpeed, direction * _walkSpeed, ref _currentVelocity, _acceleration, Mathf.Infinity, Time.deltaTime);

		_characterController.Move(_currentSpeed * Time.deltaTime);
		GetComponent<Footstep>().UpdateFootstep(_characterController.velocity.magnitude);

		GravityUpdate();
		CrouchUpdate();


		_previousPosition = transform.position;
	}

	private void GravityUpdate()
	{
		_gravity = Mathf.MoveTowards(_gravity, _fallSpeed * 2, _fallSpeed * Time.deltaTime);
		_characterController.Move(Vector2.down * _gravity * Time.deltaTime);

		_gravity = _characterController.isGrounded || _characterController.collisionFlags == CollisionFlags.Above ? 0 : _gravity;

		if (_lastGrounded && !_characterController.isGrounded)
			//This Is To Ensure That The Player Will Go Down The Stairs Properly, Without Bouncing Awkwardly Down; Giving The Illusion Of Your Character Actually Going Down The Stairs
			//The For Loop Is Used To Ensure That Low Framerate Won't Affect The Stair Movement, This Can Be Tested With Very High Movement Speeds, And Will Hopefully Always Work

			//Note, It Is Recommended That Camera Interpolation Is Checked To Avoid The Jittered Movement When You Go Down The Stairs
			if (_stairSnap)
			{
				float inverseDistance = 1 / Vector3.Distance(_previousPosition, transform.position);

				for (float i = 0; i < 1; i += inverseDistance)
				{
					Vector3 lerpedPosition = Vector3.Lerp(_previousPosition, transform.position, i);

					if (Physics.Raycast(lerpedPosition, Vector2.down, _characterController.stepOffset * 2 + _characterController.skinWidth))
						_characterController.Move(Vector2.down * 9999);
				}
			}

		if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space) &&
			!Physics.CapsuleCast(transform.position + Vector3.up * _characterController.stepOffset, transform.position + Vector3.up * _characterController.stepOffset, _characterController.radius, Vector3.up, _standHeight - _characterController.stepOffset + _characterController.skinWidth))
		{
			_lastGrounded = false;
			_gravity = -_jumpStrength;
		}

		else _lastGrounded = _characterController.isGrounded;
	}

	private void CrouchUpdate()
	{
		if (_lastGrounded)
		{
			if (Input.GetKey(KeyCode.LeftControl) ||
				Physics.CapsuleCast(transform.position + Vector3.up * _characterController.stepOffset, transform.position + Vector3.up * _characterController.stepOffset, _characterController.radius, Vector3.up, _standHeight - _characterController.stepOffset + _characterController.skinWidth))
			{
				_characterController.height = _crouchHeight;
				_isCrouched = true;
			}

			else
			{
				_characterController.height = _standHeight;
				_isCrouched = false;
			}
		}

		_characterController.center = Vector2.up * _characterController.height / 2;
	}
}
