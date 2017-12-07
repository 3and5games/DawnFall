using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Invector
{ 
    [System.Serializable]
    public abstract class Character : MonoBehaviour, ICharacter
    {
        #region Character Variables
        // acess camera info
        [HideInInspector]   public TPCamera tpCamera;
        // acess hud controller 
    
        // get the animator component of character
        [HideInInspector]   public Animator animator;   
        // physics material
        [HideInInspector]   public PhysicMaterial frictionPhysics, defaultPhysics;
        // get capsule collider information
        [HideInInspector]   public CapsuleCollider capsuleCollider;
        // storage capsule collider extra information
        [HideInInspector]   public float colliderRadius, colliderHeight;
        // storage the center of the capsule collider info
        [HideInInspector]   public Vector3 colliderCenter;
        // access the rigidbody component
        [HideInInspector]   public Rigidbody _rigidbody;
        // generate input for the controller
        [HideInInspector]   public Vector2 input;
        // lock all the character locomotion 
        [HideInInspector]   public bool lockPlayer;
        // general variables to the locomotion
        [HideInInspector]   public float speed, direction, verticalVelocity;
        // create a offSet base on the character hips 
        [HideInInspector]   public float offSetPivot;

        [HideInInspector]   public bool ragdolled;

        public Transform cameraTransform
        {
            get
            {
                Transform cT = transform;
                if (Camera.main != null)
                    cT = Camera.main.transform;
                if (tpCamera)
                    cT = tpCamera.transform;
                if (cT == transform)
                {
                    Debug.LogWarning("Invector : Missing TPCamera or MainCamera");
                    this.enabled = false;
                }                   

                return cT;
            }
        }

        [Header("--- Health & Stamina ---")]
        public bool isDead;


        #endregion

        //**********************************************************************************//
        // INITIAL SETUP 																	//
        // prepare the initial information for capsule collider, physics materials, etc...  //
        //**********************************************************************************//
        public void InitialSetup()
        {
            animator = GetComponent<Animator>();

            tpCamera = TPCamera.instance;
          
            // create a offset pivot to the character, to align camera position when transition to ragdoll
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            offSetPivot = Vector3.Distance(transform.position, hips.position);

            if (tpCamera != null)
            {
                tpCamera.offSetPlayerPivot = offSetPivot;
                tpCamera.target = transform;
            }

          
            // prevents the collider from slipping on ramps
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = 0.6f;
            frictionPhysics.dynamicFriction = 0.6f;

            // default physics 
            defaultPhysics = new PhysicMaterial();
            defaultPhysics.name = "defaultPhysics";
            defaultPhysics.staticFriction = 0f;
            defaultPhysics.dynamicFriction = 0f;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider 
            capsuleCollider = GetComponent<CapsuleCollider>();

            // save your collider preferences 
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;       

            cameraTransform.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
        }   


        public void ResetRagdoll()
        {
            tpCamera.offSetPlayerPivot = offSetPivot;
            tpCamera.SetTarget(this.transform);            
            lockPlayer = false;
            verticalVelocity = 0f;
            ragdolled = false;
        }

        public void RagdollGettingUp()
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            capsuleCollider.enabled = true;
        }

        public void EnableRagdoll()
        {
            tpCamera.offSetPlayerPivot = 0f;
            tpCamera.SetTarget(animator.GetBoneTransform(HumanBodyBones.Hips));
            ragdolled = true;
            capsuleCollider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            lockPlayer = true;
        }
    }
}