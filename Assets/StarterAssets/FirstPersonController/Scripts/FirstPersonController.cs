using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Camera height while crouched")]
        public float crouchHeight = 0.6f;
        [Tooltip("Speed while crouched")]
        public float crouchSpeed = 2.0f;
        [Tooltip("Speed you can move while sliding")]
        public float slideSpeed = 0.3f;
        [Tooltip("Drag on player more while sliding")]
        public float slideFriction = 0.75f;
        private bool sliding;
        private bool crouchKeyPressed;

        [Space(10)]
        [Tooltip("Amount to slow time by on slide 0-1")]
        public float slowDownFactor = 0.1f;
        [Tooltip("Time it takes for slow effect to wear off")]
        public float slowDownLength = 2f;
        [Tooltip("Amount to alter from starting fov, +wider, -more zoomed")]
        public float fovSlideAmount = 20f;
        public AnimationCurve fovLerpCurve;
        public AnimationCurve timeLerpCurve;
        public float deathTimer = 1.0f;
        public float fovDeathAmount = -10f;
        public AnimationCurve deathLerpCurve;
        private float targetFOV;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;
        CinemachineVirtualCamera cinemachineVirtualCamera;
        private float storedFOV;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private Vector3 prevPosition;
        private Vector3 _horizontalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool dead = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        //pause menu
        private PauseMenu pauseMenu;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.001f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            pauseMenu = FindObjectOfType<PauseMenu>();
            cinemachineVirtualCamera = transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            storedFOV = cinemachineVirtualCamera.m_Lens.FieldOfView;
            targetFOV = storedFOV;
        }

        private void FixedUpdate()
        {
            if (PauseMenu.isPaused) return;
            JumpAndGravity();
            GroundedCheck();
            SlidingCheck();
            Crouch();
            Move();
            TimeScale();
        }

        private void LateUpdate()
        {
            if (PauseMenu.isPaused) return;
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void SlidingCheck()
        {
            if (_input.crouch)
            {
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                if (currentHorizontalSpeed > crouchSpeed + 0.2f)
                {
                    sliding = true;
                }
                else
                {
                    sliding = false;
                }
            }
        }

        private void TimeScale()
        {
            Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);

            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        private IEnumerator animateFOV(float amountToAdjust)
        {
            targetFOV = storedFOV + amountToAdjust;
            float timer = 0.01f;
            while(timer < slowDownLength)
            {
                float lerpPos = fovLerpCurve.Evaluate(timer / slowDownLength);
                cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(targetFOV, storedFOV, lerpPos);

                float timeLerpPos = timeLerpCurve.Evaluate(timer / slowDownLength);
                Time.timeScale = timeLerpPos;
                AudioManager.AdjustPitch(timeLerpPos);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        public IEnumerator animateDeath()
        {
            dead = true;

            targetFOV = storedFOV + (fovDeathAmount * -_input.move.y);
            float timer = 0.01f;
            while(timer < deathTimer)
            {
                float timeLerpPos = deathLerpCurve.Evaluate(timer / deathTimer);
                Time.timeScale = timeLerpPos;
                AudioManager.AdjustPitch(timeLerpPos);
                cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(targetFOV, storedFOV, timeLerpPos);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            pauseMenu.GameLose();
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            //calculate horizontal velocity
            _horizontalVelocity = (transform.position - prevPosition);

            if (dead)
            {
                Vector3 currentHorizontalVelocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
                Vector3 newSpeed = Vector3.Lerp(inputDirection.normalized * (slideSpeed * Time.deltaTime) + currentHorizontalVelocity, Vector3.zero, Time.deltaTime * slideFriction);
                _controller.Move(newSpeed * Time.deltaTime + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
                return;
            }

            if (_input.crouch)
            {
                Vector3 currentHorizontalVelocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
                if (sliding)
                {
                    if(!crouchKeyPressed)
                    {
                        //SlowTimeDown(slowDownFactor);
                        StartCoroutine(animateFOV(fovSlideAmount));
                    }
                    Vector3 newSpeed = Vector3.Lerp(inputDirection.normalized * (slideSpeed * Time.deltaTime) + currentHorizontalVelocity, Vector3.zero, Time.deltaTime * slideFriction);
                    Vector3 leftRightInputDirection = transform.right * _input.move.x;
                    _controller.Move(leftRightInputDirection.normalized * (slideSpeed * Time.deltaTime) + (newSpeed * Time.deltaTime) + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
                }
                else
                {
                    _controller.Move(inputDirection.normalized * (crouchSpeed * Time.deltaTime) + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));
                }
                crouchKeyPressed = true;
            }
            else
            {
                // move the player
                _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                crouchKeyPressed = false;
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Crouch()
        {
            if (_input.crouch)
            {
                transform.localScale = new Vector3(1, crouchHeight, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
                sliding = false;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
