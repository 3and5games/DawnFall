using UnityEngine;
using System.Collections;

namespace Invector
{
    public abstract class ThirdPersonAnimator : ThirdPersonMotor
    {
        #region Variables
        // generate a random idle animation
        private float randomIdleCount, randomIdle;
	    // used to lerp the head track
	    private Vector3 lookPosition;
        // match cursorObject to help animation to reach their cursorObject
        [HideInInspector] public Transform matchTarget;
	    // head track control, if you want to turn off at some point, make it 0
	    [HideInInspector] public float lookAtWeight;
	    // access the animator states (layers)
	    [HideInInspector] public AnimatorStateInfo stateInfo, combatStateInfo;
        [HideInInspector] public float oldSpeed;
        public float speedTime
        {
            get
            {
                var _speed = animator.GetFloat("Speed");
                var acceleration = (_speed - oldSpeed) / Time.fixedDeltaTime;
                oldSpeed = _speed;
                return Mathf.Round(acceleration);
            }
        }
        
        #endregion

        //**********************************************************************************//
        // ANIMATOR			      															//
        // update animations at the animator controller (Mecanim)							//
        //**********************************************************************************//
        public void UpdateAnimator()
        {
            if (ragdolled)        
                DisableActions();
            else
            {
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                RandomIdle();
                QuickTurn180Animation();
                RollForwardAnimation();
                LandHighAnimation();
                JumpOverAnimation();
                ClimbUpAnimation();
                StepUpAnimation();
                JumpAnimation();
                LadderAnimation();                
                QuickStopAnimation();

                ExtraMoveSpeed();
                LocomotionAnimation();

                animator.SetBool("Aiming", strafing);
                animator.SetBool("Crouch", crouch);
                animator.SetBool("OnGround", onGround);
                animator.SetFloat("GroundDistance", groundDistance);
                animator.SetFloat("VerticalVelocity", verticalVelocity);
            }           
        }

        //**********************************************************************************//
        // DISABLE ACTIONS	      															//
        // turn false every action bool if ragdoll is enabled 								//
        //**********************************************************************************//    
        void DisableActions()
        {
		    quickTurn180 = false;
		    quickStop = false;
		    canSprint = false;
		    landHigh = false;
		    jumpOver = false;
		    rolling = false;
		    stepUp = false;
		    crouch = false;
		    strafing = false;
            jump = false;
        }

	    //**********************************************************************************//
	    // RANDOM IDLE		      															//
	    // assign the animations into the layer IdleVariations on the Animator				//
	    //**********************************************************************************//    
        void RandomIdle()
        {
            if(randomIdleTime > 0)
            {
                if (input.sqrMagnitude == 0 && !strafing && !crouch && capsuleCollider.enabled && onGround)
                {
                    randomIdleCount += Time.fixedDeltaTime;
                    if (randomIdleCount > 6)
                    {
                        randomIdleCount = 0;
                        animator.SetTrigger("IdleRandomTrigger");
                        animator.SetInteger("IdleRandom", Random.Range(1, 4));
                    }
                }
                else
                {
                    randomIdleCount = 0;
                    animator.SetInteger("IdleRandom", 0);
                }
            }        
        }

	    //**********************************************************************************//
	    // CONTROL LOCOMOTION      															//
	    //**********************************************************************************//
        void LocomotionAnimation()
        {
            if (freeLocomotionConditions)
                // free directional movement
                animator.SetFloat("Direction", lockPlayer ? 0f : direction);
            else
                // strafe movement
                animator.SetFloat("Direction", lockPlayer ? 0f : direction, 0.15f, Time.fixedDeltaTime);
            
            animator.SetFloat("Speed", !stopMove || lockPlayer ? speed : 0f, 0.2f, Time.fixedDeltaTime);
        }

        /// <summary>
        /// EXTRA MOVE SPEED - apply extra speed for the the free directional movement or the strafe movement
        /// </summary>
        void ExtraMoveSpeed()
        {
            combatStateInfo = animator.GetCurrentAnimatorStateInfo(3);
            inAttack = !combatStateInfo.IsName("Null");

            if (!inAttack)
            {
                if (stateInfo.IsName("Grounded.Strafing Movement") || stateInfo.IsName("Grounded.Strafing Crouch"))
                {
                    var newSpeed_Y = (extraStrafeSpeed * speed);
                    var newSpeed_X = (extraStrafeSpeed * direction);
                    newSpeed_Y = Mathf.Clamp(newSpeed_Y, -extraStrafeSpeed, extraStrafeSpeed);
                    newSpeed_X = Mathf.Clamp(newSpeed_X, -extraStrafeSpeed, extraStrafeSpeed);
                    transform.position += transform.forward * (newSpeed_Y * Time.fixedDeltaTime);
                    transform.position += transform.right * (newSpeed_X * Time.fixedDeltaTime);
                }
                else if (stateInfo.IsName("Grounded.Free Movement") || stateInfo.IsName("Grounded.Free Crouch"))
                {
                    var newSpeed = (extraMoveSpeed * speed);
                    transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
                }
            }
            else
            {
                speed = 0f;                
            }            
        }

        //**********************************************************************************//
        // QUICK TURN 180	      															//
        //**********************************************************************************//
        void QuickTurn180Animation()
        {
            animator.SetBool("QuickTurn180", quickTurn180);

            // complete the 180 with matchTarget and disable quickTurn180 after completed
            if (stateInfo.IsName("Action.QuickTurn180"))
            {                
                if (!animator.IsInTransition(0) && !ragdolled)
                    MatchTarget(Vector3.one, freeRotation, AvatarTarget.Root,
                                 new MatchTargetWeightMask(Vector3.zero, 1f),
                                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.9f);

                if (stateInfo.normalizedTime > 0.9f)
                {
                    quickTurn180 = false;
                    direction = 0;
                }                    
            }
        }
               

        //**********************************************************************************//
        // QUICK STOP		      															//
        //**********************************************************************************//
        void QuickStopAnimation()
        {
            animator.SetBool("QuickStop", quickStop);             

            bool quickStopConditions = !actions && onGround;
           
                // make a quickStop when release the key while running
                if (speedTime <= -3f && quickStopConditions)
                    quickStop = true;
        
                            

            // disable quickStop
            if (quickStop && input.sqrMagnitude > 0.9f || quickTurn180)
                quickStop = false;
            else if (stateInfo.IsName("Action.QuickStop"))
            {
                if (stateInfo.normalizedTime > 0.9f || input.sqrMagnitude >= 0.1f || stopMove)
                    quickStop = false;
            }
        }

        //**********************************************************************************//
        // LADDER			      															//
        //**********************************************************************************//
        void LadderAnimation()
        {
            // resume the states of the ladder in one bool 
            usingLadder = 
                stateInfo.IsName("Ladder.EnterLadderBottom") ||
                stateInfo.IsName("Ladder.ExitLadderBottom") ||
                stateInfo.IsName("Ladder.ExitLadderTop") ||
                stateInfo.IsName("Ladder.EnterLadderTop") ||
                stateInfo.IsName("Ladder.ClimbLadder");

            // just to prevent any wierd blend between this animations
            if(usingLadder)
            {
                jump = false;
                quickTurn180 = false;
            }

            // make sure to lock the player when entering or exiting a ladder
            var lockOnLadder = 
                stateInfo.IsName("Ladder.EnterLadderBottom") ||
                stateInfo.IsName("Ladder.ExitLadderBottom") ||
                stateInfo.IsName("Ladder.ExitLadderTop") ||
                stateInfo.IsName("Ladder.EnterLadderTop");

            lockPlayer = lockOnLadder;

            LadderBottom();
            LadderTop();
        }

        void LadderBottom()
        {
            animator.SetBool("EnterLadderBottom", enterLadderBottom);
            animator.SetBool("ExitLadderBottom", exitLadderBottom);

            // enter ladder from bottom
            if (stateInfo.IsName("Ladder.EnterLadderBottom"))
            {
                capsuleCollider.isTrigger = true;
                _rigidbody.useGravity = false;

                // we are using matchtarget to find the correct X & Z to start climb the ladder
                // this information is provided by the cursorObject on the object, that use the script TriggerAction 
                // in this state we are sync the position based on the AvatarTarget.Root, but you can use leftHand, left Foot, etc.
                if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                               AvatarTarget.Root, new MatchTargetWeightMask
                                (new Vector3(1, 1, 1), 1), 0.25f, 0.9f);

                if (stateInfo.normalizedTime >= 0.75f)                
                   enterLadderBottom = false;                
            }

            // exit ladder bottom
            if (stateInfo.IsName("Ladder.ExitLadderBottom"))
            {
                capsuleCollider.isTrigger = false;
                _rigidbody.useGravity = true;

                if (stateInfo.normalizedTime >= 0.4f)
                {
                    exitLadderBottom = false;
                    usingLadder = false;
                }
            }
        }

        void LadderTop()
        {
            animator.SetBool("EnterLadderTop", enterLadderTop);
            animator.SetBool("ExitLadderTop", exitLadderTop);

            // enter ladder from top            
            if (stateInfo.IsName("Ladder.EnterLadderTop"))
            {
                capsuleCollider.isTrigger = true;
                _rigidbody.useGravity = false;

                // we are using matchtarget to find the correct X & Z to start climb the ladder
                // this information is provided by the cursorObject on the object, that use the script TriggerAction 
                // in this state we are sync the position based on the AvatarTarget.Root, but you can use leftHand, left Foot, etc.
                if (stateInfo.normalizedTime < 0.25f && !animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                                AvatarTarget.Root, new MatchTargetWeightMask
                                (new Vector3(1, 0, 0.1f), 1), 0f, 0.25f);
                else if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                                AvatarTarget.Root, new MatchTargetWeightMask
                                (new Vector3(1, 1, 1), 1), 0.25f, 0.7f);

                if (stateInfo.normalizedTime >= 0.7f)
                    enterLadderTop = false;
            }

            // exit ladder top
            if (stateInfo.IsName("Ladder.ExitLadderTop"))
            {
                if (stateInfo.normalizedTime >= 0.85f)
                {
                    capsuleCollider.isTrigger = false;
                    _rigidbody.useGravity = true;
                    exitLadderTop = false;
                    usingLadder = false;
                }
            }
        }

        //**********************************************************************************//
        // ROLLING			      															//
        //**********************************************************************************//
        void RollForwardAnimation()
        {
            animator.SetBool("RollForward", rolling);

            // rollFwd
            if (stateInfo.IsName("Action.RollForward"))
            {
                lockPlayer = true;
                _rigidbody.useGravity = false;

			    // prevent the character to rolling up 
                if (verticalVelocity >= 1)
                    _rigidbody.velocity = Vector3.ProjectOnPlane(_rigidbody.velocity, groundHit.normal);

                // reset the rigidbody a little ealier to the character fall while on air
                if (stateInfo.normalizedTime > 0.3f)
                    _rigidbody.useGravity = true;

                if (!crouch && stateInfo.normalizedTime > 0.85f)
                {
                    lockPlayer = false;
                    rolling = false;
                }
			    else if (crouch && stateInfo.normalizedTime > 0.75f)
			    {
				    lockPlayer = false;
				    rolling = false;
			    }
            }
        }

	    // control the direction of rolling when strafing
	    public void Rolling()
	    {
		    bool conditions = (strafing || speed >= 0.25f) && !stopMove && onGround && !actions && !landHigh;
		
		    if (conditions)
		    {			
			    if (strafing)
			    {
				    // check the right direction for rolling if you are aiming
				    freeRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
				    var newAngle = freeRotation.eulerAngles - transform.eulerAngles;
				    direction = newAngle.NormalizeAngle().y;		
				    transform.Rotate(0, direction, 0, Space.Self);
			    }
			    rolling = true;
			    quickTurn180 = false;
		    }
	    }

        //**********************************************************************************//
        // JUMP ANIMATION	      															//
        //**********************************************************************************//
        void JumpAnimation()
        {
            animator.SetBool("Jump", jump);            
            var newSpeed = (jumpForward * speed);

            isJumping = stateInfo.IsName("Action.Jump") || stateInfo.IsName("Action.JumpMove") || stateInfo.IsName("Airborne.FallingFromJump");
            animator.SetBool("IsJumping", isJumping);

            if (stateInfo.IsName("Action.Jump"))
            {
                // apply extra height to the jump
                if (stateInfo.normalizedTime < 0.85f)
                {
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
                    transform.position += transform.up * (jumpForce * Time.fixedDeltaTime);
                }                    
                // end jump animation
                if (stateInfo.normalizedTime >= 0.85f)                          
                    jump = false;
                // apply extra speed forward
                if (stateInfo.normalizedTime >= 0.65f && jumpAirControl)                    
                    transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
                else if (stateInfo.normalizedTime >= 0.65f && !jumpAirControl)
                    transform.position += transform.forward * Time.fixedDeltaTime;
            }

            if (stateInfo.IsName("Action.JumpMove"))
            {
                // apply extra height to the jump
                if (stateInfo.normalizedTime < 0.85f)
                {
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
                    transform.position += transform.up * (jumpForce * Time.fixedDeltaTime);
                }
                // end jump animation
                if (stateInfo.normalizedTime >= 0.55f)
                    jump = false;
                // apply extra speed forward
                if (jumpAirControl)                    
                    transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
                else                    
                    transform.position += transform.forward * Time.fixedDeltaTime;
            }

            // apply extra speed forward when falling
            if (stateInfo.IsName("Airborne.FallingFromJump") && jumpAirControl)                
                transform.position += transform.forward * (newSpeed * Time.fixedDeltaTime);
            else if (stateInfo.IsName("Airborne.FallingFromJump") && !jumpAirControl)                
                transform.position += transform.forward * Time.fixedDeltaTime;
        }

        //**********************************************************************************//
        // HARD LANDING		      															//
        //**********************************************************************************//
        void LandHighAnimation()
        {
            animator.SetBool("LandHigh", landHigh);

		    // if the character fall from a great height, landhigh animation
		    if (!onGround && verticalVelocity <= landHighVel && groundDistance <= 0.5f)
                landHigh = true;

            if (landHigh && stateInfo.IsName("Airborne.LandHigh"))
            {
                quickStop = false;
                if (stateInfo.normalizedTime >= 0.1f && stateInfo.normalizedTime <= 0.2f)
			    {
				    // vibrate the controller 
			    }
			
			    if (stateInfo.normalizedTime > 0.9f)
                {
                    landHigh = false;                
                }
            }
        }

        //**********************************************************************************//
        // STEP UP			      															//
        //**********************************************************************************//
        void StepUpAnimation()
        {
            animator.SetBool("StepUp", stepUp);

            if (stateInfo.IsName("Action.StepUp"))
            {
                if (stateInfo.normalizedTime > 0.1f && stateInfo.normalizedTime < 0.3f)
                {
                    gameObject.GetComponent<Collider>().isTrigger = true;
                    _rigidbody.useGravity = false;
                }

			    // we are using matchtarget to find the correct height of the object                
                if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                                AvatarTarget.LeftHand, new MatchTargetWeightMask
                                (new Vector3(0, 1, 1), 0), 0f, 0.5f);

                if (stateInfo.normalizedTime > 0.9f)
                {
                    gameObject.GetComponent<Collider>().isTrigger = false;
                    _rigidbody.useGravity = true;
                    stepUp = false;
                }
            }
        }

        //**********************************************************************************//
        // JUMP OVER		      															//
        //**********************************************************************************//
        void JumpOverAnimation()
        {
            animator.SetBool("JumpOver", jumpOver);

		    if (stateInfo.IsName("Action.JumpOver"))
            {
                quickTurn180 = false;
			    if (stateInfo.normalizedTime > 0.1f && stateInfo.normalizedTime < 0.3f)
				    _rigidbody.useGravity = false;

                // we are using matchtarget to find the correct height of the object
                if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                                AvatarTarget.LeftHand, new MatchTargetWeightMask
                                (new Vector3(0, 1, 1), 0), 0.1f*(1-stateInfo.normalizedTime), 0.3f*(1 - stateInfo.normalizedTime));

                if (stateInfo.normalizedTime >= 0.7f)
			    {
				    _rigidbody.useGravity = true;
				    jumpOver = false;
			    }                
            }
        }

        //**********************************************************************************//
        // CLIMB			      															//
        //**********************************************************************************//
        void ClimbUpAnimation()
        {
            animator.SetBool("ClimbUp", climbUp);

            if (stateInfo.IsName("Action.ClimbUp"))
            {
                if (stateInfo.normalizedTime > 0.1f && stateInfo.normalizedTime < 0.3f)
                {
				    _rigidbody.useGravity = false;
                    gameObject.GetComponent<Collider>().isTrigger = true;               
                }

			    // we are using matchtarget to find the correct height of the object
                if (!animator.IsInTransition(0))
                    MatchTarget(matchTarget.position, matchTarget.rotation,
                               AvatarTarget.LeftHand, new MatchTargetWeightMask
                               (new Vector3(0, 1, 1), 0), 0f, 0.2f);

                if (stateInfo.normalizedTime >= 0.85f)
                {
                    gameObject.GetComponent<Collider>().isTrigger = false;
				    _rigidbody.useGravity = true;
                    climbUp = false;
                }
            }
        }    
  
       //***********************************************************************************//
       // HEAD TRACK		      											    			//
       // head follow where you point at and curve the spine while aiming   				//
       //***********************************************************************************//
       Vector3 lookPoint(float distance)
       {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            return ray.GetPoint(distance);
       }

       void OnAnimatorIK()
       {
            // get the random idle layer to blend animations
            stateInfo = animator.GetCurrentAnimatorStateInfo(1);

            var rot = Quaternion.LookRotation(lookPoint(1000f) - transform.position, Vector3.up);
            var newAngle = rot.eulerAngles - transform.eulerAngles;
            var ang = newAngle.NormalizeAngle().y;
            bool headTrackConditions = !usingLadder && !rolling && !landHigh;

            if (!strafing)
            {
                if (headTracking && headTrackConditions)
                {
                    if (ang <= 70 && ang >= 0 && !lockPlayer && stateInfo.IsName("Null") || ang > -70 && ang <= 0 && !lockPlayer && stateInfo.IsName("Null"))
                        lookAtWeight += 1f * Time.fixedDeltaTime;
                    else
                        lookAtWeight -= 1f * Time.fixedDeltaTime;

                    lookAtWeight = Mathf.Clamp(lookAtWeight, 0f, 1f);
                    animator.SetLookAtWeight(lookAtWeight, 0.3f, 1f);
                }
            }
            else
                if(!actions) animator.SetLookAtWeight(0.5f, spineCurvature);

            lookPosition = Vector3.Lerp(lookPosition, lookPoint(1000f), 10f * Time.fixedDeltaTime);
            animator.SetLookAtPosition(lookPosition);
        }

        //**********************************************************************************//
        // MATCH TARGET																		//
        // call this method to help animations find the correct cursorObject						//
        // don't forget to add the curve MatchStart and MatchEnd on the animation clip		//
        //**********************************************************************************//
        void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, 
	                     MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
	    {
		    if (animator.isMatchingTarget)
			    return;
		
		    float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
		    if (normalizeTime > normalisedEndTime)
			    return;

		    if(!ragdolled)
		    animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
	    }   
	
    }
}