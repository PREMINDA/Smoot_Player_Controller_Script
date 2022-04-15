using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


namespace SmoothPlayer
{
    public class NewCharacterController : MonoBehaviour
    {
        [SerializeField] private Bounds playerBounds;
        [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        public bool JumpingThisFrame;
        private PlayerFrameInput _input;
        
        public bool landingThisFrame;
        public Vector3 Velocity { get; private set; }
        
        //Calculate Rays
        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        
        //Collision Check
        [Header("Collision")]
        [SerializeField] private int detectorCount = 3;
        [SerializeField] private float detectionRayLength = 0.1f;
        private bool _colUp, _colRight, _colDown, _colLeft;
        private float _timeLeftGrounded;
        private bool _coyoteUsable;
        
        //Calculate Walk
        [Header("Walk")]
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;
        [SerializeField] private float _acceleration = 90f;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;
        private float _moveClamp = 13;
        
        //Jump
        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        private bool _endedJumpEarly = true;
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;
        private float _apexPoint;

        //Gravity
        [FormerlySerializedAs("_releasJumEarly")]
        [Header("Gravity")]
        [SerializeField] private bool _releasJumpEarly = true;
        [SerializeField] private float _fallClamp = -40f;
        private float _fallSpeed;
        
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            CalculateRayRanged();
           
            Debug.DrawRay(_raysUp.Start,_raysUp.Dir,Color.green);
            Debug.DrawRay(_raysUp.End,_raysUp.Dir,Color.green);
            
            Debug.DrawRay(_raysDown.Start,_raysDown.Dir,Color.blue);
            Debug.DrawRay(_raysDown.End,_raysDown.Dir,Color.blue);
            
            Debug.DrawRay(_raysLeft.Start,_raysLeft.Dir,Color.magenta);
            Debug.DrawRay(_raysLeft.End,_raysLeft.Dir,Color.magenta);
            
            Debug.DrawRay(_raysRight.Start,_raysRight.Dir,Color.yellow);
            Debug.DrawRay(_raysRight.End,_raysRight.Dir,Color.yellow);

        }

        private void RegisterInput()
        {
            _input = new PlayerFrameInput()
            {
                X = Input.GetAxis("Horizontal"),
                JumpDown = Input.GetKeyDown(KeyCode.Space),
                JumpUp = Input.GetKeyUp(KeyCode.Space)
            };
            if (_input.JumpDown) {
                _lastJumpPressed = Time.time;
            }
        }

        private void CollisionCheck()
        {
            CalculateRayRanged();

            landingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time;
            else if (!_colDown && groundedCheck)
            {
                _coyoteUsable = true; // Only trigger when first touching
                landingThisFrame = true;
            }

            _colDown = groundedCheck;

            _colUp = RunDetection(_raysUp);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);
        }
        
        private bool RunDetection(RayRange range) {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, groundLayer));
        }
        
        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
            for (var i = 0; i < detectorCount; i++) {
                var t = (float)i / (detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void CalculateRayRanged()
        {
            var bound = new Bounds(transform.position, playerBounds.size);

            _raysDown = new RayRange(bound.min.x + rayBuffer, bound.min.y, bound.max.x - rayBuffer, bound.min.y, Vector2.down);
            _raysUp = new RayRange(bound.min.x + rayBuffer, bound.max.y, bound.max.x - rayBuffer, bound.max.y, Vector2.up);
            _raysLeft = new RayRange(bound.min.x, bound.min.y + rayBuffer, bound.min.x, bound.max.y - rayBuffer, Vector2.left);
            _raysRight = new RayRange(bound.max.x, bound.min.y + rayBuffer, bound.max.x, bound.max.y - rayBuffer, Vector2.right);

        }

        private void CalculateWalk()
        {
            if (_input.X != 0)
            {
                //Set horizontal speed
                _currentHorizontalSpeed += _input.X * _acceleration * Time.deltaTime;

                //set limit to speed
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);
                
                //bonus at the apex jump
                var apexBonus = Mathf.Sign(_input.X) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                //if No input
                _currentHorizontalSpeed =
                    Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }
            if(_currentHorizontalSpeed>0 && _colRight || _currentHorizontalSpeed<0 &&_colLeft)
            {
                //Stopping move through wall
                _currentHorizontalSpeed = 0;
            }
        }

        private void CalculateGravity()
        {
            if (_colDown)
            {
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                var fallSpeed = _releasJumpEarly && _currentVerticalSpeed > 0
                    ? _fallSpeed * _jumpEndEarlyGravityModifier
                    : _fallSpeed;

                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }
        
        private void CalculateJumpApex() {
            if (!_colDown) {
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else {
                _apexPoint = 0;
            }
        }
        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + playerBounds.center, playerBounds.size);
        }
    }
}
