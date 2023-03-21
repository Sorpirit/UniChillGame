using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Charac : MonoBehaviour
{
    public struct FrameInput
    {
        public float X;
        public bool JumpDown;
        public bool JumpUp;
    }
    
    [Header("WALKING")] 
    [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    [Header("COLLISION")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
    
    [FormerlySerializedAs("_jumpHeight")] [Header("JUMPING")] [SerializeField] private float _jumpVelocity = 30;
    [SerializeField] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _coyoteTimeThreshold = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpEndEarlyGravityMultiplier = 0.45f;
    
    private Rigidbody2D _rb;
    private Collider2D _collider;
    
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool CanUseCoyote => !_collisionGround && _coyoteUsable && _ofGroundTime + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _collisionGround && _lastJumpPressed + _jumpBuffer > Time.time;
    
    private Vector3 Velocity 
    { 
        get => _rb.velocity;
        set => _rb.velocity = value; 
    }

    private FrameInput Input { get; set; }
    
    private float _currentHorizontalSpeed, _currentVerticalSpeed;
    
    private bool _collisionGround;
    private float _ofGroundTime;
    
    private PlayerInputActions _inputActions;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        _inputActions.Player.Jump.performed += GatherJumpInput;
        _inputActions.Player.Jump.canceled += GatherJumpInput;
        _inputActions.Player.HorizontalMovement.performed += GatherHorizontalInput;
        _inputActions.Player.HorizontalMovement.canceled += GatherHorizontalInput;
    }

    private void Update()
    {
        RunGroundCheck();
        _currentVerticalSpeed = Velocity.y;

        //Fall speed clamp
        if (_currentVerticalSpeed < _fallClamp) 
            _currentVerticalSpeed = _fallClamp;
        
        //Apex calc
        if (!_collisionGround)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
        }
        else
        {
            _apexPoint = 0;
        }
        
        CalculateHorizontalSpeed();
        CalculateJump();

        Velocity = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed);
    }


    #region Gather Input

    private void GatherJumpInput(InputAction.CallbackContext context)
    {
        Input = new FrameInput
        {
            JumpDown = context.performed,
            JumpUp = context.canceled,
            X = _inputActions.Player.HorizontalMovement.ReadValue<float>()
        };
        if (Input.JumpDown)
        {
            _lastJumpPressed = Time.time;
        }
    }
    
    private void GatherHorizontalInput(InputAction.CallbackContext context)
    {
        Input = new FrameInput
        {
            JumpDown = Input.JumpDown,
            JumpUp = Input.JumpUp,
            X = context.ReadValue<float>()
        };
    }

    #endregion

    #region Collisions
    
    private void RunGroundCheck()
    {
        Vector2 minBounds = _collider.bounds.min;
        Vector2 maxBounds = _collider.bounds.max;
        var groundedCheck = EvaluateRayPositions(new Vector2(minBounds.x + _rayBuffer, minBounds.y), new Vector2(maxBounds.x - _rayBuffer, minBounds.y))
            .Any(point => Physics2D.Raycast(point, Vector2.down, _detectionRayLength, _groundLayer));;
        
        switch (_collisionGround)
        {
            case true when !groundedCheck:
                _ofGroundTime = Time.time; // Only trigger when first leaving
                break;
            case false when groundedCheck:
                _coyoteUsable = true; // Only trigger when first touching
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
    
    private void CalculateHorizontalSpeed()
    {
        if (Input.X != 0)
        {
            // Set horizontal move speed
            _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

            // Apply bonus at the apex of a jump
            var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }
    }

    #endregion
    
    #region Jump
    
    private void CalculateJump()
    {
        if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
        {
            _currentVerticalSpeed = _jumpVelocity;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _ofGroundTime = float.MinValue;
        }

        // End the jump early if button released
        if (!_collisionGround && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
        {
            _currentVerticalSpeed *= _jumpEndEarlyGravityMultiplier;
            _endedJumpEarly = true;
        }
    }

    #endregion
}