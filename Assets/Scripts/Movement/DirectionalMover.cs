using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Movement
{
    public class DirectionalMover : MonoBehaviour
    {
        [Header("COLLISION")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; 
        
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityMultiplier = 0.45f;
        
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private new Collider2D collider;

        public Vector2 Velocity => rigidbody.velocity;
        
        private bool _collisionGround;
        private float _ofGroundTime;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint;
        private float _lastJumpPressed;

        private bool CanUseCoyote => !_collisionGround && _coyoteUsable && _ofGroundTime + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _collisionGround && _lastJumpPressed + _jumpBuffer > Time.time;
        
        #region Collisions
    
        public void RunGroundCheck()
        {
            Vector2 minBounds = (Vector2) collider.bounds.min;// + collider.offset;
            Vector2 maxBounds = (Vector2) collider.bounds.max;// + collider.offset;
            var groundedCheck = EvaluateRayPositions(new Vector2(minBounds.x + _rayBuffer, minBounds.y), new Vector2(maxBounds.x - _rayBuffer, minBounds.y))
                .Any(point => Physics2D.Raycast(point, Vector2.down, _detectionRayLength, _groundLayer));;
        
            switch (_collisionGround)
            {
                case true when !groundedCheck:
                    _ofGroundTime = Time.time;
                    break;
                case false when groundedCheck:
                    _coyoteUsable = true;
                    break;
            }

            _collisionGround = groundedCheck;
        }
    
        private IEnumerable<Vector2> EvaluateRayPositions(Vector2 start, Vector2 end)
        {
            for (var i = 0; i < _detectorCount; i++)
            {
                var t = (float) i / (_detectorCount - 1);
                yield return Vector2.Lerp(start, end, t);
            }
        }
    
        #endregion
    
        #region Walk
    
        public void CalculateHorizontalSpeed(MovementInput input, MovementData data)
        {
            float currentHorizontalSpeed = rigidbody.velocity.x;
            if (input.X != 0)
            {
                currentHorizontalSpeed += input.X * data.Acceleration * Time.deltaTime;

                currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -data.MoveClamp, data.MoveClamp);
            }
            else
            {
                currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, data.DeAcceleration * Time.deltaTime);
            }

            var rigidbodyVelocity = rigidbody.velocity;
            rigidbodyVelocity.x = currentHorizontalSpeed;
            rigidbody.velocity = rigidbodyVelocity;
        }

        #endregion
    
        #region Jump
    
        public void CalculateJump(MovementInput input, MovementData data)
        {
            float currentVerticalSpeed = rigidbody.velocity.y;
            
            if (currentVerticalSpeed < data.FallClamp) 
                currentVerticalSpeed = data.FallClamp;

            if (input.JumpDown)
                _lastJumpPressed = Time.time;
            
            if (input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                currentVerticalSpeed = data.JumpVelocity;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _ofGroundTime = float.MinValue;
            }

            // End the jump early if button released
            if (!_collisionGround && input.JumpUp && !_endedJumpEarly && rigidbody.velocity.y > 0)
            {
                currentVerticalSpeed *= _jumpEndEarlyGravityMultiplier;
                _endedJumpEarly = true;
            }
            
            var rigidbodyVelocity = rigidbody.velocity;
            rigidbodyVelocity.y = currentVerticalSpeed;
            rigidbody.velocity = rigidbodyVelocity;
        }

        #endregion
    }
}