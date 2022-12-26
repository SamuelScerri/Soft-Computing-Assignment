/*
	I was originally going to use a pre-made First-Person Controller, but since I managed to make a lot of progress, I decided to create my own movement system which allowed me to have more control over the gameplay.
	The reason I used Character Controller instead of Rigidbody is to have more fine-tuned control over the movement, even though it is less realistic. In the end I think it feels nice and doesn't run poorly.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	private byte Sensitivity, WalkSpeed, RunSpeed, FallSpeed, JumpStrength;

	[SerializeField]
	private bool StairSnap, CameraInterpolation;

	[SerializeField]
	private float Acceleration;

	private CharacterController CC;

	private Vector3 CurrentSpeed;
	private Vector3 CurrentVelocity;
	private Vector3 PreviousPosition;
	private float CameraPosition;
	private float CameraVelocity;

	private float RotationLimit;
	private float Gravity;

	private bool LastGrounded;

	private void Start()
	{
		CC = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;

		CameraPosition = Camera.main.transform.position.y;
		CC.Move(Vector2.down * 512);
	}

	private void Update()
	{
		//This Ensures That More Velocity Will Only Be In A Certain Direction, This Means That If You Look Behind You Whilst Moving, You Won't Continue Moving Forward But Rather Backwards As You Will Still Be Slowing Down
		Vector3 Direction = transform.TransformDirection(Vector3.forward) * Input.GetAxisRaw("Vertical") + transform.TransformDirection(Vector3.right) * Input.GetAxisRaw("Horizontal");
		
		//Movement Is Normalized To Ensure That Diagonal Movement Will Stay The Same
		Direction.Normalize();

		//Smooth Damp Will Ensure That The Acceleration Will Be Constant Across Different Framerates
		if (Input.GetKey(KeyCode.LeftShift) && LastGrounded)
			CurrentSpeed = Vector3.SmoothDamp(CurrentSpeed, Direction * RunSpeed, ref CurrentVelocity, Acceleration);
		else CurrentSpeed = Vector3.SmoothDamp(CurrentSpeed, Direction * WalkSpeed, ref CurrentVelocity, Acceleration);



		CC.Move(CurrentSpeed * Time.deltaTime);
		GravityCheck();

		//This Will Be Used For The Next Gravity Check, More Information Is Given In The Function
		PreviousPosition = transform.position;


	}

	private void GravityCheck()
	{
		if (LastGrounded)
		{
			//This Is To Ensure That The Player Will Go Down The Stairs Properly, Without Bouncing Awkwardly Down; Giving The Illusion Of Your Character Actually Going Down The Stairs
			//The While Loop Is Used To Ensure That Low Framerate Won't Affect The Stair Movement, This Can Be Tested With Very High Movement Speeds, And Will Hopefully Always Work

			//Note, It Is Recommended That Camera Interpolation Is Checked To Avoid The Jittered Movement When You Go Down The Stairs
			if (StairSnap)
				while (PreviousPosition != transform.position)
				{
					PreviousPosition = Vector3.MoveTowards(PreviousPosition, transform.position, .1f);
					if (Physics.Raycast(PreviousPosition, Vector2.down, CC.stepOffset * 2 + CC.skinWidth))
						CC.Move(Vector2.down * 512);
				}
			
			if (Input.GetKeyDown(KeyCode.Space))
				Gravity = -JumpStrength;
			else Gravity = 0;
		}

		Gravity = Mathf.MoveTowards(Gravity, FallSpeed * 2, FallSpeed * Time.deltaTime);
		CC.Move(Vector2.down * Gravity * Time.deltaTime);

		LastGrounded = CC.isGrounded;
	}

	private void LateUpdate()
	{
		transform.Rotate(Vector2.up * Input.GetAxis("Mouse X") * Sensitivity);

		RotationLimit += Input.GetAxis("Mouse Y") * Sensitivity;
		RotationLimit = Mathf.Clamp(RotationLimit, -90, 90);

		Camera.main.transform.localEulerAngles = Vector2.right * RotationLimit;

		//Smooth Camera Movement
		if (CameraInterpolation)
		{
			CameraPosition = Mathf.SmoothDamp(CameraPosition, transform.position.y + 2, ref CameraVelocity, .04f);
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, CameraPosition, Camera.main.transform.position.z);
		}
	}
}
