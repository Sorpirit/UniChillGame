using System;
using UnityEngine;

public class AnimationStateManager : MonoBehaviour
{
    private const string State = "State";
        
    [SerializeField] private Animator animator;

    public event Action<PlayerAnimationState> AnimationPerformed;
    public event Action<PlayerAnimationState> AnimationCanceled;
    
    private PlayerAnimationState _currentState = PlayerAnimationState.Idle;
    private bool _inTriggerMode;

    public void ApplyAnimationState(PlayerAnimationState state)
    {
        switch (_inTriggerMode)
        {
            case true when state <= _currentState:
                return;
            case true:
                AnimationCanceled?.Invoke(_currentState);
                _inTriggerMode = false;
                break;
        }

        _currentState = state;
        animator.SetInteger(State,  (int) state);
    }
    
    public void TriggerAnimationState(PlayerAnimationState state)
    {
        if(state <= _currentState)
            return;
        
        if(_inTriggerMode)
            AnimationCanceled?.Invoke(_currentState);
        
        _inTriggerMode = true;
        _currentState = state;
        animator.SetInteger(State,  (int) state);
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