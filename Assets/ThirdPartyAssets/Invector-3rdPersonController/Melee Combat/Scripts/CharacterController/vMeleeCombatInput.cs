using UnityEngine;
using System.Collections;
#if MOBILE_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif
using Invector.EventSystems;
using System;

namespace Invector.CharacterController
{
    // here you can modify the Melee Combat inputs
    // if you want to modify the Basic Locomotion inputs, go to the vThirdPersonInput
    [vClassHeader("MELEE INPUT MANAGER", iconName = "inputIcon")]
    public class vMeleeCombatInput : vThirdPersonInput, vIMeleeFighter
    {
        [System.Serializable]
        public class OnUpdateEvent : UnityEngine.Events.UnityEvent<vMeleeCombatInput> { }

        #region Variables                

        protected vMeleeManager meleeManager;
        protected bool isAttacking;
        protected bool isBlocking;       

        [Header("MeleeCombat Inputs")]
        public GenericInput weakAttackInput = new GenericInput("Mouse0", "RB", "RB");
        public GenericInput strongAttackInput = new GenericInput("Alpha1", false, "RT", true, "RT", false);
        public GenericInput blockInput = new GenericInput("Mouse1", "LB", "LB");

        [HideInInspector]
        public new OnUpdateEvent onUpdateInput = new OnUpdateEvent();
        [HideInInspector]
        public bool lockInputByItemManager;

        #endregion

        protected override void Start()
        {
            base.Start();
        }

        public virtual bool lockInventory
        {
            get
            {
                return isAttacking || cc.isDead;
            }
        }

        protected override void LateUpdate()
        {
            UpdateAttackBehaviour();
            base.LateUpdate();
            onUpdateInput.Invoke(this);
        }

        protected override void InputHandle()
        {
            if (cc == null) return;

            if (MeleeAttackConditions && !lockInputByItemManager)
            {
                MeleeWeakAttackInput();
                MeleeStrongAttackInput();
                BlockingInput();
            }
            else
            {
                isBlocking = false;
            }

            if (!isAttacking)
            {
                base.InputHandle();
                UpdateMeleeAnimations();
            }
        }

        #region MeleeCombat Input Methods
        
        /// <summary>
        /// WEAK ATK INPUT
        /// </summary>
        protected virtual void MeleeWeakAttackInput()
        {
            if (cc.animator == null) return;

            if (weakAttackInput.GetButtonDown() && MeleeAttackStaminaConditions())
            {
                cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
                cc.animator.SetTrigger("WeakAttack");
            }
        }

        /// <summary>
        /// STRONG ATK INPUT
        /// </summary>
        protected virtual void MeleeStrongAttackInput()
        {
            if (cc.animator == null) return;

            if (strongAttackInput.GetButtonDown() && MeleeAttackStaminaConditions())
            {
                cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
                cc.animator.SetTrigger("StrongAttack");
            }
        }

        /// <summary>
        /// BLOCK INPUT
        /// </summary>
        protected virtual void BlockingInput()
        {
            if (cc.animator == null) return;

            isBlocking = blockInput.GetButton() && cc.currentStamina > 0;
        }

        #endregion

        #region Conditions

        protected virtual bool MeleeAttackStaminaConditions()
        {
            var result = cc.currentStamina - meleeManager.GetAttackStaminaCost();
            return result >= 0;
        }

        protected virtual bool MeleeAttackConditions
        {
            get
            {
                if (meleeManager == null) meleeManager = GetComponent<vMeleeManager>();
                return meleeManager != null && !cc.customAction && !cc.lockMovement && !cc.isCrouching;
            }
        }

        #endregion

        #region Update Animations

        protected virtual void UpdateMeleeAnimations()
        {
            if (cc.animator == null || meleeManager == null) return;
            cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
            cc.animator.SetInteger("DefenseID", meleeManager.GetDefenseID());
            cc.animator.SetBool("IsBlocking", isBlocking);
            cc.animator.SetFloat("MoveSet_ID", meleeManager.GetMoveSetID(), .2f, Time.deltaTime);            
        }

        protected virtual void UpdateAttackBehaviour()
        {
            if (cc.IsAnimatorTag("Attack")) return;                     
            // lock the speed to stop the character from moving while attacking
            cc.lockSpeed = cc.IsAnimatorTag("Attack") || isAttacking;
            // force root motion animation while attacking
            cc.forceRootMotion = cc.IsAnimatorTag("Attack") || isAttacking;
        }

        #endregion

        #region Melee Methods

        public void OnEnableAttack()
        {
            cc.currentStaminaRecoveryDelay = meleeManager.GetAttackStaminaRecoveryDelay();
            cc.currentStamina -= meleeManager.GetAttackStaminaCost();
            cc.lockRotation = true;
            isAttacking = true;
        }

        public void OnDisableAttack()
        {
            cc.lockRotation = false;
            isAttacking = false;
        }

        public void ResetAttackTriggers()
        {
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        public void BreakAttack(int breakAtkID)
        {
            ResetAttackTriggers();
            OnRecoil(breakAtkID);
        }

        public void OnRecoil(int recoilID)
        {
            cc.animator.SetInteger("RecoilID", recoilID);
            cc.animator.SetTrigger("TriggerRecoil");
            cc.animator.SetTrigger("ResetState");
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        public void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {
            // character is blocking
            if (!damage.ignoreDefense && isBlocking && meleeManager != null && meleeManager.CanBlockAttack(attacker.Character().transform.position))
            {
                var damageReduction = meleeManager.GetDefenseRate();
                if (damageReduction > 0)
                    damage.ReduceDamage(damageReduction);
                if (attacker != null && meleeManager != null && meleeManager.CanBreakAttack())
                    attacker.OnRecoil(meleeManager.GetDefenseRecoilID());
                meleeManager.OnDefense();
                cc.currentStaminaRecoveryDelay = damage.staminaRecoveryDelay;
                cc.currentStamina -= damage.staminaBlockCost;
            }
            // apply damage
            cc.TakeDamage(damage, !isBlocking);
        }

        public vCharacter Character()
        {
            return cc;
        }

        #endregion

    }
}

