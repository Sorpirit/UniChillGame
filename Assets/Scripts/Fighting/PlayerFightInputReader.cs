using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fighting
{
    public class PlayerFightInputReader : IFightingInputProvider
    {
        private const int AttackActions = 2;
        private const float TOLERANCE = 0.001f;
        
        private float[] _lastTriggeredBuffer = new float[AttackActions];
        private float _attackBuffer;

        public int ActiveAttackIndex
        {
            get
            {
                int minI = -1;
                for (int i = 0; i < _lastTriggeredBuffer.Length; i++)
                {
                    if(Math.Abs(_lastTriggeredBuffer[i] - (-1)) < TOLERANCE)
                        continue;

                    if (_lastTriggeredBuffer[i] + _attackBuffer < Time.deltaTime)
                    {
                        _lastTriggeredBuffer[i] = -1;
                        continue;
                    }

                    if (minI == -1 || _lastTriggeredBuffer[i] < _lastTriggeredBuffer[minI])
                        minI = i;
                }

                return minI;
            }
        }
        
        public PlayerFightInputReader(PlayerInputActions actions, AttacksData data)
        {
            actions.Player.BasicAttack.performed += GatherInputAttackA;
            actions.Player.StrongAttack.performed += GatherInputAttackB;

            _attackBuffer = data.AttackBuffer;
        }
        
        public void ResetAttackIndex(int index)
        {
            if(index < 0)
                return;
            
            _lastTriggeredBuffer[index] = -1;
        }
        
        private void GatherInputAttackA(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;
        
            _lastTriggeredBuffer[0] = Time.time;
        }

        private void GatherInputAttackB(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _lastTriggeredBuffer[1] = Time.time;
        }
    }
}