using System;
using Animation;
using Core.Player;
using Fighting;
using JetBrains.Annotations;
using Movement;
using UnityEngine;

namespace Core
{
    public class GlobalSceneManager : MonoBehaviour
    {
        public static GlobalSceneManager Instance { get; private set; }
        public static PlayerInputActions InputInstance => Instance.Input;

        [SerializeField] private MovementData _movementData;
        [SerializeField] private AttacksData _attacksData;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private DirectionalMover player;
        [SerializeField] private AnimationStateManager animationStateManager;
        [SerializeField] private Transform spriteFlipper;
        
        public PlayerInputActions Input { get; private set; }
        [CanBeNull] public Camera GlobalCamera { get; set; }

        private PlayerBrain _playerBrain;

        private bool isPaused = false;
        
        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;

            GlobalCamera = camera;
            Input = new PlayerInputActions();
            Input.Enable();

            PlayerMoveInputReader moveInputReader = new PlayerMoveInputReader(Input);
            PlayerFightInputReader fightInputReader = new PlayerFightInputReader(Input, _attacksData);
            PlayerAnimationController playerAnimation = new PlayerAnimationController(animationStateManager, spriteFlipper);
            BasicAttacker attacker = new BasicAttacker();
            _playerBrain = new PlayerBrain(_movementData, _attacksData, moveInputReader, fightInputReader, player, attacker, playerAnimation);
        }

        private void FixedUpdate()
        {
            if(isPaused)
                return;
            
            _playerBrain.FixedUpdate();   
        }
    }
}