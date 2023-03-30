using System;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMoveInputReader : IMovementInputProvider
    {
        public MovementInput Input { get; private set; }

        public PlayerMoveInputReader(PlayerInputActions actions)
        {
            actions.Player.Jump.performed += GatherJumpInput;
            actions.Player.Jump.canceled += GatherJumpInput;
            actions.Player.HorizontalMovement.performed += GatherHorizontalInput;
            actions.Player.HorizontalMovement.canceled += GatherHorizontalInput;
        }

        private void GatherJumpInput(InputAction.CallbackContext context)
        {
            Input = new MovementInput
            {
                JumpDown = context.performed,
                JumpUp = context.canceled,
                X = Input.X
            };
        }
    
        private void GatherHorizontalInput(InputAction.CallbackContext context)
        {
            Input = new MovementInput
            {
                JumpDown = Input.JumpDown,
                JumpUp = Input.JumpUp,
                X = context.ReadValue<float>()
            };
        }
        
        public void ResetOneTimeActions()
        {
            Input = new MovementInput
            {
                JumpDown = false,
                JumpUp = false,
                X = Input.X
            };
        }
    }
}