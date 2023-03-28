using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sources
{
    public class GlobalPlayerInput
    {
        public static GlobalPlayerInput Instance => _instance ??= new GlobalPlayerInput();

        public static PlayerInputActions InputInstance => Instance.Input;

        [CanBeNull] private static GlobalPlayerInput _instance;
        
        public PlayerInputActions Input { get; }

        [CanBeNull] public Camera GlobalCamera { get; set; }
        public Vector2 MouseScreenPosition => Input.Player.MousePosition.ReadValue<Vector2>();
        public Vector2 MouseWorldPosition
        {
            get
            {
                Assert.IsNotNull(GlobalCamera, "Camera is not inited");
                var screenPosition2D = MouseScreenPosition;
                var screenPosition3D = new Vector3(screenPosition2D.x, screenPosition2D.y, GlobalCamera.nearClipPlane);
                return GlobalCamera.ScreenToWorldPoint(screenPosition3D);
            }
        }

        private GlobalPlayerInput()
        {
            Assert.IsNull(_instance);
            _instance = this;
            
            Input = new PlayerInputActions();
            Input.Enable();

        }
    }
}