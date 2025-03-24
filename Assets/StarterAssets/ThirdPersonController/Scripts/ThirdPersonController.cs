using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        // Make singleton
        public static ThirdPersonController Instance;

        [Header("Player Stats")]
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public float RotationSpeed = 1.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Upgradable Player Stats")]
        [Tooltip("Level of the player")]
        public int PlayerLevel = 1;
        [Tooltip("Current player XP")]
        public int CurrentPlayerXP = 0;
        [Tooltip("XP Required for next level")]
        public int NextLevelRequiredXP = 100;

        [Tooltip("Maximum health of the player")]
        public float MaxHealth = 100f;
        private float _health = 100f;

        [Tooltip("Health regeneration rate per second")]
        public float HealthRegenRate = 0f;
        private float _regenTimer = 0f;

        [Tooltip("Base move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed multiplier of the character")]
        public float SprintMultiplier = 2f;

        [Tooltip("Damage dealt by the player")]
        public float Damage = 10f;

        [Tooltip("Attack speed of the player (attacks per second)")]
        public float AttackSpeed = 1.0f;

        [Tooltip("Maximum number of jumps allowed")]
        public int MaxJumps = 2;
        private bool _doubleJumpAvailable = false;

        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;



        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built-in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degrees to override the camera. Useful for fine-tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axes")]
        public bool LockCameraPosition = false;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        // Cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // Player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _baseMoveSpeed;
        private float _currentJumps; // num of jumps taken currently
        private Weapon weapon;
        public List<int> items;

        public int gold;
        // Timeout delta time
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDHealth;



#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            Instance = this; // instantate singleton
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _animator = GetComponentInParent<Animator>(); // animator is attached to parent, not child
            _hasAnimator = _animator != null;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            gold = 0;
            MaxJumps = 2; // Make sure to set MaxJumps to 2 at start of game

            // Get weapon from child
            weapon = GetComponentInChildren<Weapon>();
            if (weapon == null)
            {
                Debug.LogWarning("Weapon script not found on child object!");
            }

            // Ensure UI gets the player's max health at the start
            UIController.Instance.SetMaxHealth((int)_health);

            // initialize item list to be empty
            items = new List<int>();
        }

        private void Update()
        {
            //Debug.Log("Move Input: " + _input.move); // Check if input is updating

            if (!_playerInput.enabled)
            {
                Debug.LogError("PlayerInput is DISABLED! Enabling it now...");
                _playerInput.enabled = true;
            }

            JumpAndGravity();
            GroundedCheck();
            Move();


            _animator.SetFloat(_animIDHealth, _health);

            // Interacting with chests
            if (_input.interact)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 15f))
                {
                    //print("Interacted with " + hit.transform.name + " from " + hit.distance + "m.");

                    // Always find the Chest component on the parent
                    Chest targetChest = hit.transform.GetComponentInParent<Chest>();
                    if (targetChest) targetChest.Interact();

                    // find the microwave door if there is any
                    MicrowaveDoor microwaveDoor = hit.transform.GetComponent<MicrowaveDoor>();
                    if (microwaveDoor) microwaveDoor.Toggle();

                    // kicking the lemon
                    if (hit.collider.CompareTag("Collidables"))
                    {
                        Rigidbody lemonRb = hit.collider.GetComponent<Rigidbody>();
                        if (lemonRb != null)
                        {
                            Vector3 kickDirection = ray.direction.normalized;
                            float kickForce = 50f;

                            lemonRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
                            Debug.Log("Player kicked lemon with force " + kickForce);
                        }
                    }
                }

                _input.interact = false; // Reset interaction
            }

            // Health Regen
            if (HealthRegenRate > 0 && _health < MaxHealth)
            {
                _regenTimer += Time.deltaTime;

                if (_regenTimer >= 1f)
                {
                    float regenAmount = HealthRegenRate * Mathf.Floor(_regenTimer); // Regen per full second(s)
                    _health += regenAmount;
                    _health = Mathf.Min(_health, MaxHealth); // Clamp to MaxHealth
                    _regenTimer -= Mathf.Floor(_regenTimer); // Retain fractional overflow

                    UIController.Instance.SetHealth((int)_health);
                }
            }


        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            _health = Mathf.Max(0, _health);
            //Debug.Log("Player took damage! Current Health: " + _health);

            UIController.Instance.SetHealth((int)_health);

            if (_health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            //Debug.Log("Player is dead");

            // Store final stats for Game Over screen
            PlayerPrefs.SetInt("FinalMoney", gold);
            PlayerPrefs.SetFloat("FinalTime", UIController.Instance.GetElapsedTime());
            PlayerPrefs.Save(); // Ensure data is saved

            // Load Game Over scene
            UIController.Instance.GameOver();
        }


        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDHealth = Animator.StringToHash("Health");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
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
            // Determine the target speed by multiplying base MoveSpeed with SprintMultiplier when sprinting
            float targetSpeed = _input.sprint ? MoveSpeed * SprintMultiplier : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalize input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
            else
            {
                print("No animator!?");
            }
        }

        private void JumpAndGravity()
        {
            // Check if the player is grounded.
            if (Grounded)
            {
                // Reset the double jump flag when on the ground.
                _doubleJumpAvailable = true;
                _currentJumps = 1;

                // Prevent downward buildup on the ground.
                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = -2f;
                }

                // If the player presses space on the ground, perform the first jump.
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PerformFirstJump();
                }
            }
            else
            {
                // While airborne, if the player presses space and a double jump is available,
                // perform the double jump.
                if (Input.GetKeyDown(KeyCode.Space) && _doubleJumpAvailable)
                {
                    PerformDoubleJump();
                }
            }

            // Apply gravity over time.
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void PerformFirstJump()
        {
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _doubleJumpAvailable = true;
            _currentJumps++;
            if (_hasAnimator)
            {
                _animator.Play("PlayerJump"); // Restart the animation immediately
                _animator.SetBool("Jump", true);
                Invoke("ResetJumpBool", 0.1f); // Small delay to prevent immediate exit
            }

            //Debug.Log("First jump executed.");
        }

        private void PerformDoubleJump()
        {
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _currentJumps++;
            if (_currentJumps >= MaxJumps)
            {
                _doubleJumpAvailable = false;
            }


            if (_hasAnimator)
            {
                _animator.Play("PlayerJump"); // Restart the animation immediately
                _animator.SetBool("Jump", true);
                Invoke("ResetJumpBool", 0.1f);
            }

            //Debug.Log("Double jump executed.");
        }

        private void ResetJumpBool()
        {
            _animator.SetBool("Jump", false);
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
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void AddMoney(int amount)
        {
            gold += amount;
            //Debug.Log("Player received money! Total Gold: " + gold);

            // Update UI
            UIController.Instance.AddMoney(amount);
        }

        public void AddItem(int itemID)
        {
            items.Add(itemID);
            //Debug.Log("Added " + itemID + " to player items");
        }

        // Gets the weapon, used for updating UI
        public Weapon GetWeapon()
        {
            return weapon;
        }

        public void UpgradePlayer(string statName, float value)
        {
            switch (statName.ToLower())
            {
                case "maxhealth":
                    MaxHealth += value;  // Increase MaxHealth by the given value
                    _health = MaxHealth; // Restore health to new max
                    UIController.Instance.SetMaxHealth((int)MaxHealth); // Update UI health bar
                    break;

                case "movespeed":
                    MoveSpeed += value;
                    break;

                case "sprintspeed":
                    SprintMultiplier += value;
                    break;

                case "damage":
                    weapon.damage += value;
                    Debug.Log("Weapon damage upgraded to " + weapon.damage);
                    break;

                case "attackspeed":

                    weapon.attackSpeed += value;
                    Debug.Log("Weapon attack speed upgraded to " + weapon.attackSpeed);
                    break;

                case "maxjumps":
                    // Increase MaxJumps by value (cast to int if necessary)
                    MaxJumps += (int)value;
                    Debug.Log("MaxJumps increased to " + MaxJumps);
                    break;

                case "healthregen":
                    HealthRegenRate += value;
                    break;

                case "jumpheight":
                    // Increase JumpHeight by value
                    JumpHeight += value;
                    break;

                default:
                    Debug.LogWarning($"UpgradePlayer: Invalid stat name '{statName}'");
                    return;
            }

            // Notify UI to update stats display
            UIController.Instance.UpdateStatsDisplay();
        }


    }
}