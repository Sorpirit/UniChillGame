using Fighting;
using Movement;
using UnityEngine;

namespace Animation
{
    public class PlayerAnimationController
    {

        private readonly AnimationStateManager _animationStateManager;
        private readonly Transform _animationFlipper;
        private bool _canPlayAttackAnimation = false;

        public PlayerAnimationController(AnimationStateManager animationStateManager, Transform animationFlipper)
        {
            _animationStateManager = animationStateManager;
            _animationFlipper = animationFlipper;
        }

        public void PreUpdate(BasicAttacker attacker)
        {
            _canPlayAttackAnimation = attacker.IsAbleToAttack;
        }
        
        public void UpdateAnimationSystem(MovementInput input, AttackInfo? attackInfo, Vector2 velocity)
        {
            PlayerAnimationState newState = PlayerAnimationState.Idle;
            bool isTurning = false;
            
            if (_canPlayAttackAnimation && attackInfo.HasValue)
            {
                _animationStateManager.TriggerAnimationState(attackInfo.Value.animationState);
                return;
            }

            if (velocity.y != 0)
            {
                newState = velocity.y > 0 ? PlayerAnimationState.Jump : PlayerAnimationState.Fall;
            }
            else if (velocity.x != 0)
            {
                isTurning = (velocity.x * input.X) < 0;
                newState = isTurning ? PlayerAnimationState.QuickTurn : PlayerAnimationState.Run;
            }
        
            if (!isTurning && input.X != 0)
            {
                var animationFlipperLocalScale = _animationFlipper.localScale;
                animationFlipperLocalScale.x = Mathf.Sign(velocity.x);
                _animationFlipper.localScale = animationFlipperLocalScale;
            }
            
            _animationStateManager.ApplyAnimationState(newState);
        }
    }
}