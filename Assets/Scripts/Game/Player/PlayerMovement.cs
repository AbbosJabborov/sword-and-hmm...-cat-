using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 10f;
        
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float groundDrag = 0.1f;

        public bool IsGrounded { get; private set; }
        public bool IsRunning => _isRunning;
        public Vector3 CurrentVelocity { get; private set; }
        public float CurrentSpeed => CurrentVelocity.magnitude;

        private CharacterController _controller;
        private Vector2 _moveInput;
        private Vector3 _velocity;
        private bool _isRunning;

        private void Awake() => _controller = GetComponent<CharacterController>();

        public void SetMoveInput(Vector2 input) => _moveInput = input;
        public void SetRunning(bool running) => _isRunning = running;

        private void FixedUpdate()
        {
            // Ground detection
            IsGrounded = _controller.isGrounded;
            
            Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply movement in world space
            float speed = _isRunning ? runSpeed : walkSpeed;
            _controller.Move(moveDirection * (speed * Time.deltaTime));

            // Handle gravity and velocity
            if (IsGrounded && _velocity.y < 0)
                _velocity.y = -2f; // Small negative to keep grounded

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);

            CurrentVelocity = moveDirection * speed;

            Debug.Log($"[MOVEMENT] Speed: {CurrentSpeed:F2}, IsGrounded: {IsGrounded}, Velocity.y: {_velocity.y:F2}");
        }

        public void Jump()
        {
            if (!IsGrounded)
            {
                Debug.Log("[JUMP] Not grounded, cannot jump");
                return;
            }

            _velocity.y = jumpForce;
            IsGrounded = false;
            Debug.Log($"[JUMP] Jumped! Velocity.y = {_velocity.y}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
        }
    }
}