using Core;
using UnityEngine;

namespace Animation
{
    public class ParallaxBackgroundController : MonoBehaviour
    {
        [SerializeField] private Transform[] parallaxBg;
        [SerializeField] private Transform[] staticBg;
    
        [SerializeField] private Vector2 parallaxEffectOffset = new (0.1f, 0.1f);
        [SerializeField] private Vector2 parallaxEffectDeclaration = new (0.8f, 0.96f);

        [SerializeField] private float characterOffset;
        [SerializeField] private float nearClipPlane;
        [SerializeField] private float farClipPlane = 50;

        [SerializeField] private float yFactor = 180f / 320f;
    
        private Vector3 _lastCameraPosition;
    
        private void Start()
        {
            _lastCameraPosition = GlobalSceneManager.Instance.GlobalCamera.transform.position;
        }

        private void FixedUpdate()
        {
            Vector3 cameraPosition = GlobalSceneManager.Instance.GlobalCamera.transform.position;
            Vector3 delta = cameraPosition - _lastCameraPosition;
            Vector2 multiplayer = Vector2.one - parallaxEffectOffset;
            foreach (var bgTransform in parallaxBg)
            {
                Vector3 bgPos = bgTransform.position;
                float distFromSubject = bgPos.z - characterOffset;
                float clippingPlane = cameraPosition.z + (distFromSubject > 0 ? farClipPlane : nearClipPlane);
                float parallaxFactor = Mathf.Abs(distFromSubject) / clippingPlane; 
                bgTransform.position = new Vector3(bgPos.x + delta.x * parallaxFactor, bgPos.y + delta.y * parallaxFactor * yFactor, bgTransform.position.z);
                multiplayer *= parallaxEffectDeclaration;
            }
        
            foreach (var bgTransform in staticBg)
            {
                bgTransform.position = cameraPosition;
            }

            _lastCameraPosition = cameraPosition;
        }
    }
}
