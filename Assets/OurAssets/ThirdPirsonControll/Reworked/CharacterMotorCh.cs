using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class CharacterMotorCh : MonoBehaviour
{

    [System.Serializable]
    public class CharacterMotorMovement
    {
        enum MovementTransferOnJump
        {
            None, // The jump is not affected by velocity of floor at all.
            InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
            PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
            PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
        }

        public float maxForwardSpeed = 10;
        public float maxSidewaysSpeed = 10;
        public float maxBackwardsSpeed = 10;
        public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
        public float maxGroundAcceleration = 30;
        public float maxAirAcceleration = 20;
        public float gravity = 10;
        public float maxFallSpeed = 20;

        private CollisionFlags collisionFlags;
        private Vector3 velocity;
        private Vector3 frameVelocity = Vector3.zero;
        private Vector3 hitPoint = Vector3.zero;
        private Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
    }

    public class CharacterMotorJumping
    {
        // Can the character jump?
        public bool enabled = true;

        // How high do we jump when pressing jump and letting go immediately
        float baseHeight = 1.0f;

        // We add extraHeight units (meters) on top when holding the button down longer while jumping
        float extraHeight = 4.1f;

        // How much does the character jump out perpendicular to the surface on walkable surfaces?
        // 0 means a fully vertical jump and 1 means fully perpendicular.
        float perpAmount = 0.0f;

        // How much does the character jump out perpendicular to the surface on too steep surfaces?
        // 0 means a fully vertical jump and 1 means fully perpendicular.
        float steepPerpAmount = 0.5f;

        // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
        // Very handy for organization!

        // Are we jumping? (Initiated with jump button and not grounded yet)
        // To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.

        private bool jumping = false;

        private bool holdingJumpButton = false;

        // the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
        private float lastStartTime = 0.0f;

        private float lastButtonDownTime = -100;

        private Vector3 jumpDir = Vector3.up;
    }

    public class CharacterMotorMovingPlatform
    {
        public enum MovementTransferOnJump
        {
            None, // The jump is not affected by velocity of floor at all.
            InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
            PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
            PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
        }
        public bool enabled = true;
        public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;
        private Transform hitPlatform;
        private Transform activePlatform;
        private Vector3 activeLocalPoint;
        private Vector3 activeGlobalPoint;
        private Quaternion activeLocalRotation;
        private Quaternion activeGlobalRotation;
        private Matrix4x4 lastMatrix;
        private Vector3 platformVelocity;
        private bool newPlatform;
    }

    public class CharacterMotorSliding
    {
        public bool enabled = true;

        public float slidingSpeed = 15;

        // How much can the player control the sliding direction?
        // If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
        public float sidewaysControl = 1.0f;

        // How much can the player influence the sliding speed?
        // If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
        public float speedControl = 0.4f;
    }


    public bool CanControl = true;
    public bool UseFixedUpdate = true;

    private Vector3 inputmoveDirection;
    private bool inputJump;
    private CharacterMotorMovement movement = new CharacterMotorMovement();
    private CharacterMotorJumping jumping = new CharacterMotorJumping();
    private CharacterMotorSliding sliding = new CharacterMotorSliding();
    private CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();

    private bool grounded = true;
    private Vector3 groundNormal = Vector3.zero;
    private Vector3 lastGroundNormal = Vector3.zero;
    private Transform tr : Transform;

private var controller : CharacterController;

    // We will contain all the jumping related variables in one helper class for clarity.
}