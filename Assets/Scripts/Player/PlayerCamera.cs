using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float _cameraSensitivity, _cameraMaxSpeed;
	private Vector2 _cameraRotation, _cameraDampedRotation;
	private float _cameraPosition, _cameraDampedPosition;

	private CharacterController _characterController;

	private void Start()
	{
		_characterController = GetComponent<CharacterController>();
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0)
		{
			transform.eulerAngles = Vector2.up *
				Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraRotation.y, ref _cameraDampedRotation.y, .1f);

			//This Is Responsible For Smoothly Rotating The Camera
			Camera.main.transform.rotation = Quaternion.Euler(
				Mathf.SmoothDampAngle(Camera.main.transform.eulerAngles.x, _cameraRotation.x, ref _cameraDampedRotation.x, .1f),
			Camera.main.transform.eulerAngles.y, 0);

			UpdateCamera();
		}
	}

	private void UpdateCamera()
	{
		//A Seperate Vector Is Used To Ensure That The Player Won't Be Able To Flip The Camera Upside Down
		if (Mathf.Abs(_cameraDampedRotation.y) < _cameraMaxSpeed)
			_cameraRotation += new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * _cameraSensitivity;
		_cameraRotation = new Vector2(Mathf.Clamp(_cameraRotation.x, -90, 90), _cameraRotation.y);

		_cameraPosition = Mathf.SmoothDamp(_cameraPosition, _characterController.height, ref _cameraDampedPosition, .1f);
		Camera.main.transform.localPosition = Vector3.up * _cameraPosition;
	}
}
