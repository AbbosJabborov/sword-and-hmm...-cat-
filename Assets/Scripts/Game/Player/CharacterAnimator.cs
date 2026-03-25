using UnityEngine;

namespace Game.Player
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;

        private bool _wasGrounded = true;
        private float _currentSpeedParam = 0f;

        private void Start()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            
            if (!movement)
                movement = GetComponent<PlayerMovement>();

            if (!animator)
                Debug.LogError("[ANIMATOR] No Animator component found!");
            if (!movement)
                Debug.LogError("[ANIMATOR] No PlayerMovement component found!");
        }

        private void Update()
        {
            if (!animator || !movement) return;
            
            float targetSpeed = 0f;
            
            _wasGrounded = movement.IsGrounded;
            
            if (movement.CurrentSpeed > 0.1f)
            {
                // Calculate speed based on walk/run speed
                float maxSpeed = movement.IsRunning ? runSpeed : walkSpeed;
                targetSpeed = Mathf.Clamp01(movement.CurrentSpeed / maxSpeed);
            }

            // Smooth transition to target speed
            _currentSpeedParam = Mathf.Lerp(_currentSpeedParam, targetSpeed, Time.deltaTime * 5f);
            animator.SetFloat("speed", _currentSpeedParam);

            Debug.Log($"[ANIMATOR] Speed: {_currentSpeedParam:F2}, IsRunning: {movement.IsRunning}, IsGrounded: {movement.IsGrounded}");

            // Handle jumping
            if (!movement.IsGrounded && _wasGrounded)
            {
                // Just left ground
                animator.SetBool("isJumping", true);
                Debug.Log("[ANIMATOR] Started jumping");
            }

            if (movement.IsGrounded && !_wasGrounded)
            {
                // Just landed
                animator.SetBool("isJumping", false);
                Debug.Log("[ANIMATOR] Landed");
            }


        }
    }
}