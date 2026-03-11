using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 10f; // Smooth rotation toward movement
        
        public bool IsGrounded => isGrounded;
        public bool IsRunning => isRunning;
        public Vector3 CurrentVelocity => velocity;

        private CharacterController controller;
        private Vector2 moveInput;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isRunning;

        private void Awake() => controller = GetComponent<CharacterController>();

        public void SetMoveInput(Vector2 input) => moveInput = input;
        public void SetRunning(bool running) => isRunning = running;

        private void FixedUpdate()
        {
            isGrounded = controller.isGrounded;
            
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply movement in world space (not character-relative)
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDirection * (speed * Time.deltaTime));

            // Handle gravity
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}