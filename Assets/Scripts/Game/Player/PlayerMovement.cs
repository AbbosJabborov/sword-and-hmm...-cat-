using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.5f;
        
        public bool IsGrounded => _isGrounded;
        public bool IsRunning => _isRunning;

        private CharacterController _controller;
        private Vector2 _moveInput;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isRunning;

        private void Awake() => _controller = GetComponent<CharacterController>();

        public void SetMoveInput(Vector2 input) => _moveInput = input;
        public void SetRunning(bool running) => _isRunning = running;

        public void Jump()
        {
            if (_isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void FixedUpdate()
        {
            _isGrounded = _controller.isGrounded;

            float speed = _isRunning ? runSpeed : walkSpeed;
            Vector3 move = (transform.right * _moveInput.x + transform.forward * _moveInput.y).normalized;
            _controller.Move(move * (speed * Time.deltaTime));

            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}