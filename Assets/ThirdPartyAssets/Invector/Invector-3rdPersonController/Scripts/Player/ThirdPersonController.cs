using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Invector.CharacterController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]

    public class ThirdPersonController : ThirdPersonAnimator
    {
        private static ThirdPersonController _instance;
        public static ThirdPersonController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ThirdPersonController>();
                    //Tell unity not to destroy this object when loading a new scene
                    //DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }        
	   
        void Awake()
        {
            StartCoroutine("UpdateRaycast");	// limit raycasts calls for better performance            
        }

        void Start()
        {
            InitialSetup();						// setup the basic information, created on Character.cs	
        }

        void FixedUpdate()
        {
		    UpdateMotor();					// call ThirdPersonMotor methods
		    UpdateAnimator();				// update animations on the Animator and their methods              
            ControlCameraState();			// change CameraStates
        }

        void LateUpdate()
        {
            HandleInput();					// handle input from controller, keyboard&mouse or touch
            RotateWithCamera();				// rotate the character with the camera    					
        }  


        //**********************************************************************************//
        // INPUT  		      																//
        // here you can config everything that recieve input								//   
        //**********************************************************************************//
        void HandleInput()
        {
           
            CameraInput();
            if (!lockPlayer)
            {
               
                    
                AttackInput();
            }
            else
            {
                input = Vector2.zero;
                speed = 0f;
			    canSprint = false;
            }
        }

	    //**********************************************************************************//
	    // UPDATE RAYCASTS																	//
	    // handles a separate update for better performance									//
	    //**********************************************************************************//
	    public IEnumerator UpdateRaycast()
	    {
		    while (true)
		    {
			    yield return new WaitForEndOfFrame();
			
			    CheckAutoCrouch();
			    CheckForwardAction();
			    StopMove();
                SlopeLimit();
            }
	    }

        //**********************************************************************************//
        // CAMERA STATE    		      														//
        // you can change de CameraState here, the bool means if you want lerp of not		//
        // make sure to use the same CameraState String that you named on TPCameraListData  //
        //**********************************************************************************//
        void ControlCameraState()
        {
            if (tpCamera == null)
                return;

            if (changeCameraState && !strafing)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
            else if (crouch)            
                tpCamera.ChangeState("Crouch", true);
            else if (strafing)
                tpCamera.ChangeState("Aim", true);
            else
                tpCamera.ChangeState("Default", true);
        }

	    //**********************************************************************************//
	    // MOVE CONTROLLER 		      														//
	    // gets input from keyboard, gamepad or mobile touch								//
	    //**********************************************************************************//
		void HorizontalInput(float v)
        {         
			input = new Vector2(v, input.y);
        }

		void VerticalInput(float v)
		{
			input = new Vector2(input.x, v);
		}

	    //**********************************************************************************//
	    // CAMERA INPUT	 		      														//
	    //**********************************************************************************//
        void CameraInput()
        {
            if (tpCamera == null)
                return;

                tpCamera.RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                tpCamera.Zoom(Input.GetAxis("Mouse ScrollWheel"));
                  
        }

        //**********************************************************************************//
        // RUNNING INPUT		      														//
        //**********************************************************************************//
		void RunningInput(bool running)
        {
            
			if (running && input.sqrMagnitude > 0.1f)
                {
                    if (onGround && !strafing && !crouch)
                        canSprint = true;
                }
			else if (!running || input.sqrMagnitude < 0.1f || strafing)
                    canSprint = false;
           

        }

	    //**********************************************************************************//
	    // CROUCH INPUT		      															//
	    //**********************************************************************************//
		void CrouchInput(bool crouchInput)
        {
            if (autoCrouch)
                crouch = true;
            
				crouch = crouchInput && onGround && !actions;			
			Debug.Log (crouch);
        }

        //**********************************************************************************//
        // JUMP INPUT   		      														//
        //**********************************************************************************//
        void JumpInput()
        {
            bool jumpConditions = !crouch && onGround && !actions;

           
                if (jumpConditions)
                    jump = true;
          
        }

        //**********************************************************************************//
        // AIMING INPUT		      															//
        //**********************************************************************************//
		void AimInput(bool aiming)
        {
            if (!locomotionType.Equals(LocomotionType.OnlyFree))
            {
               
				strafing = aiming && !actions;                                 
         
            }           
        }

        // prototype 
        void AttackInput()
        {
            if (Input.GetMouseButtonDown(0) && !actions)
                animator.SetTrigger("MeleeAttack");
        }

	   
		void OnEnable()
		{
			//TPSInput.Instance.onRoll += Rolling;
			TPSInput.Instance.onHorizontalChanged += HorizontalInput;
			TPSInput.Instance.onVerticalChanged += VerticalInput;
			//TPSInput.Instance.onRunningChanged += RunningInput;
			TPSInput.Instance.onCrouchChanged += CrouchInput;
			TPSInput.Instance.onAimingChanged += AimInput;
			TPSInput.Instance.onJump += JumpInput;
		}

        //**********************************************************************************//
        // ACTIONS 		      																//
        // WHITE raycast to check if there is anything interactable ahead					//
        //**********************************************************************************//
        void CheckForwardAction()
        {
            var hitObject = CheckActionObject();
            if (hitObject != null)
            {
                try
                {
                    if (hitObject.CompareTag("ClimbUp"))
                        DoAction(hitObject, ref climbUp);
                    else if (hitObject.CompareTag("StepUp"))
                        DoAction(hitObject, ref stepUp);
                    else if (hitObject.CompareTag("JumpOver"))
                        DoAction(hitObject, ref jumpOver);
                    else if (hitObject.CompareTag("AutoCrouch"))
                        autoCrouch = true;                    
                    else if (hitObject.CompareTag("EnterLadderBottom") && !jump)
                        DoAction(hitObject, ref enterLadderBottom);
                    else if (hitObject.CompareTag("EnterLadderTop") && !jump)
                        DoAction(hitObject, ref enterLadderTop);
                }
                catch (UnityException e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
            
        }

        void DoAction(GameObject hitObject, ref bool action)
        {
            var triggerAction = hitObject.transform.GetComponent<TriggerAction>();
            if (!triggerAction)
            {
                Debug.LogWarning("Missing TriggerAction Component on " + hitObject.transform.name + "Object");
                return;
            }
            
                if (Input.GetButton("A") && !actions || triggerAction.autoAction && !actions)
                {
                    // turn the action bool true and call the animation
                    action = true;
                    // disable the text and sprite 
                    
                    // find the cursorObject height to match with the character animation
                    matchTarget = triggerAction.target;
                    // align the character rotation with the object rotation
                    var rot = hitObject.transform.rotation;
                    transform.rotation = rot;
                }
            
        }        

        //**********************************************************************************//
        // ON TRIGGER STAY                                                                  //
        //**********************************************************************************//
        void OnTriggerStay(Collider other)
        {
			/*
            try
            {
                // if you are using the ladder and reach the exit from the bottom
                if (other.CompareTag("ExitLadderBottom") && usingLadder)
                {
                    
                        if (Input.GetButtonDown("B") || speed <= -0.05f && !enterLadderBottom)
                            exitLadderBottom = true;
                 
                }
                // if you are using the ladder and reach the exit from the top
                if (other.CompareTag("ExitLadderTop") && usingLadder && !enterLadderTop)
                {
                    if (speed >= 0.05f)
                        exitLadderTop = true;
                }
            }
            catch (UnityException e)
            {
                Debug.LogWarning(e.Message);
            }
*/
        }  
    }    
}