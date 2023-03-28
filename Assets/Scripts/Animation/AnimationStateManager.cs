using System;
using UnityEngine;

namespace Animation
{
    public class AnimationStateManager : MonoBehaviour
    {
        private const string State = "State";
        
        [SerializeField] private Animator animator;

        public event Action<PlayerAnimationState> AnimationPerformed;
        public event Action<PlayerAnimationState> AnimationCanceled;
    
        private PlayerAnimationState _currentState = PlayerAnimationState.Idle;
        private bool _inTriggerMode;

        /// <summary>
        /// Overrides any currently running animation. In case some trigger is running it will check if the priority of the animation is higher only than trigger animation will be canceled.
        /// Typically used for looping animations 
        /// </summary>
        /// <param name="state"></param>
        /// <returns>If the animation is started successfully</returns>
        public bool ApplyAnimationState(PlayerAnimationState state)
        {
            switch (_inTriggerMode)
            {
                case true when state <= _currentState:
                    return false;
                case true:
                    AnimationCanceled?.Invoke(_currentState);
                    _inTriggerMode = false;
                    break;
            }

            _currentState = state;
            animator.SetInteger(State,  (int) state);
            return true;
        }
    
        /// <summary>
        /// Almost the same as <see cref="ApplyAnimationState"/>. Typically used for one time animations
        /// </summary>
        /// <param name="state"></param>
        public bool TriggerAnimationState(PlayerAnimationState state)
        {
            if(state <= _currentState)
                return false;
        
            if(_inTriggerMode)
                AnimationCanceled?.Invoke(_currentState);
        
            _inTriggerMode = true;
            _currentState = state;
            animator.SetInteger(State,  (int) state);
            return true;
        }

        public void OnAnimationPerformed()
        {
            AnimationPerformed?.Invoke(_currentState);
        }

        public void OnAnimationFinished()
        {
            _inTriggerMode = false;
            ApplyAnimationState(PlayerAnimationState.Idle);
        }
    }
}