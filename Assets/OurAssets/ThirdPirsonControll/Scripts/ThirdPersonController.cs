

using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
	public bool Blocked;
	[SerializeField]
	private float SpeedMultiplyer = 1;
	private bool allowShooting = false;
	private float animationLerpAmount = 3;
	private float speed, sideSpeed, rotationSpeed;
	private Animator animator;
	private bool crouching = false;

	void Start ()
	{
		animator = GetComponentInChildren<Animator> ();
		Blocked = false;
	}

	void Update ()
	{
		if (Blocked == false) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Controle ();
		}
		Move ();
		Animate ();
	}

	private void Animate ()
	{
		animator.SetFloat ("Speed", Mathf.Lerp (animator.GetFloat ("Speed"), speed, Time.deltaTime * animationLerpAmount));
		animator.SetFloat ("SideSpeed", Mathf.Lerp (animator.GetFloat ("SideSpeed"), sideSpeed, Time.deltaTime * animationLerpAmount));
		animator.SetFloat ("RotationSpeed", Mathf.Lerp (animator.GetFloat ("RotationSpeed"), rotationSpeed, Time.deltaTime * animationLerpAmount/4));
		animator.SetBool ("Crouching", crouching);
		animator.SetBool ("Aiming", allowShooting);
	}

	private void Controle ()
	{
		allowShooting = Input.GetMouseButton (1);
		speed = Input.GetAxis ("Vertical");
		sideSpeed = Input.GetAxis ("Horizontal");
		if (Input.GetKey (KeyCode.LeftShift) && speed == 1 && !allowShooting && !crouching) {
			speed = 3;
		}

		crouching = Input.GetKey (KeyCode.LeftControl);
	}


	private void Move ()
	{
		var mouseHorizontal = Input.GetAxis ("Mouse X");
		rotationSpeed = mouseHorizontal;
		transform.Rotate (Vector3.up*mouseHorizontal);
		transform.Translate (sideSpeed * Time.deltaTime * SpeedMultiplyer, 0, Time.deltaTime * speed * SpeedMultiplyer);
	}

	private void OnDisable()
	{
		animator.SetFloat ("Speed", 0);
		animator.SetFloat ("SideSpeed", 0);
		animator.SetFloat ("RotationSpeed", 0);
		animator.SetBool ("Crouching", false);
	}
}
