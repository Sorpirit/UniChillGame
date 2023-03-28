using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterFightingController : MonoBehaviour
{
    [Serializable]
    public struct AttackInput
    {
        public float LastTimeAttackATriggered;
        public float LastTimeAttackBTriggered;
    }

    [SerializeField]private AttackInfo attackA;
    [SerializeField]private AttackInfo attackB;
    
    [SerializeField] private float inputBuffer;
    [SerializeField] private AnimationStateManager animator;
    
    public AttackInput _input;
    private float _attackRechargeTimer;

    private bool InputIsBuffered => Mathf.Max(_input.LastTimeAttackATriggered, _input.LastTimeAttackBTriggered) > 0; 
    
    private void Start()
    {
        GlobalPlayerInput.InputInstance.Player.BasicAttack.performed += GatherInputAttackA;
        GlobalPlayerInput.InputInstance.Player.StrongAttack.performed += GatherInputAttackB;
    }

    private void OnDestroy()
    {
        GlobalPlayerInput.InputInstance.Player.BasicAttack.performed -= GatherInputAttackA;
        GlobalPlayerInput.InputInstance.Player.StrongAttack.performed -= GatherInputAttackB;
    }

    private void Update()
    {
        if (_attackRechargeTimer > 0)
        {
            _attackRechargeTimer -= Time.deltaTime;
            return;
        }
        
        if(!InputIsBuffered)
            return;

        var targetAttack = GetTargetAttack();
        
        if(!targetAttack.HasValue)
            return;

        Attack(targetAttack.Value);
        RestInput(targetAttack.Value.Equals(attackA), targetAttack.Value.Equals(attackB));
    }

    private void RestInput(bool resetAttackA = false, bool resetAttackB = false)
    {
        if(resetAttackA)
            _input.LastTimeAttackATriggered = -1;
        
        if(resetAttackB)
            _input.LastTimeAttackBTriggered = -1;
    }

    private void GatherInputAttackA(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        
        _input.LastTimeAttackATriggered = Time.time;
    }
    
    private void GatherInputAttackB(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        
        _input.LastTimeAttackBTriggered = Time.time;
    }

    private void Attack(AttackInfo targetAttack)
    {
        _attackRechargeTimer = targetAttack.rechargeTime;
        animator.TriggerAnimationState(targetAttack.animationState);
    }

    private AttackInfo? GetTargetAttack()
    {
        AttackInfo targetAttack;
        var attackTime = Mathf.Min(_input.LastTimeAttackATriggered, _input.LastTimeAttackBTriggered);
        
        if(attackTime + inputBuffer < Time.time)
        {
            attackTime = Mathf.Max(_input.LastTimeAttackATriggered, _input.LastTimeAttackBTriggered);
            
            if (attackTime + inputBuffer < Time.time)
            {
                RestInput(true, true);
                return null;
            }
        }

        if (_input.LastTimeAttackATriggered > 0 && Math.Abs(attackTime - _input.LastTimeAttackATriggered) < 0.001)
        {
            targetAttack = attackA;
        }
        else
        {
            targetAttack = attackB;
        }

        return targetAttack;
    }
}