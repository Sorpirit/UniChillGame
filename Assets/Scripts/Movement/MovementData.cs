using UnityEngine;
using UnityEngine.Serialization;

namespace Movement
{
    [CreateAssetMenu]
    public class MovementData : ScriptableObject
    {
        [Header("WALKING")]
        [SerializeField] private float acceleration = 90;
        [SerializeField] private float moveClamp = 13;
        [SerializeField] private float deAcceleration = 60f;
        
        [Header("GRAVITY")] [SerializeField] private float fallClamp = -40f;
        [Header("JUMPING")] [SerializeField] private float jumpVelocity = 30;

        public float Acceleration => acceleration;
        public float MoveClamp => moveClamp;
        public float DeAcceleration => deAcceleration;
        public float FallClamp => fallClamp;
        public float JumpVelocity => jumpVelocity;
    }
}