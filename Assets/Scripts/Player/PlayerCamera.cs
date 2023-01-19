using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float _cameraSensitivity, _cameraMaxSpeed;
	private Vector3 _cameraRotation, _cameraDampedRotation;
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
			if (Input.GetKey("q"))
				_cameraRotation = new Vector3(_cameraRotation.x, _cameraRotation.y, 15);
			else if (Input.GetKey("e"))
				_cameraRotation = new Vector3(_cameraRotation.x, _cameraRotation.y, -15);

			transform.eulerAngles = Vector2.up *
				Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraRotation.y, ref _cameraDampedRotation.y, .1f);

			//This Is Responsible For Smoothly Rotating The Camera
			Camera.main.transform.localRotation = Quaternion.Euler(
				Mathf.SmoothDampAngle(Camera.main.transform.eulerAngles.x, _cameraRotation.x, ref _cameraDampedRotation.x, .1f),
				0,
				Mathf.SmoothDampAngle(Camera.main.transform.eulerAngles.z, _cameraRotation.z, ref _cameraDampedRotation.z, .1f));

			UpdateCamera();
		}
	}

	private void UpdateCamera()
	{
		//A Seperate Vector Is Used To Ensure That The Player Won't Be Able To Flip The Camera Upside Down
		if (Mathf.Abs(_cameraDampedRotation.y) < _cameraMaxSpeed)
			_cameraRotation += new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * _cameraSensitivity;
		_cameraRotation = new Vector2(Mathf.Clamp(_cameraRotation.x, -89, 89), _cameraRotation.y);

		_cameraPosition = Mathf.SmoothDamp(_cameraPosition, _characterController.height, ref _cameraDampedPosition, .1f);
		//Camera.main.transform.localPosition = Camera.main.transform.localRotation * Vector3.up * _cameraPosition;

		Camera.main.transform.localPosition =  (Camera.main.transform.localRotation * new Vector3(0, _cameraPosition, 0));
		Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, _cameraPosition, 0);
	}
}
