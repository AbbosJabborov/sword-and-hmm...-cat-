using UnityEngine;

namespace Game.Player
{
    public class PlayerHeadBob : MonoBehaviour
    {
        [Header("Bob Settings")]
        [SerializeField] private float bobSpeed = 8f;
        [SerializeField] private float bobAmount = 0.05f;
        [SerializeField] private float bobSideAmount = 0.02f;
        [SerializeField] private float midpoint = 0.0f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private float moveThreshold = 0.1f;

        [Header("References")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Transform cameraPivot;

        private Vector3 _initialLocalPos;
        private float _bobTimer;
        private Vector3 _smoothVelocity;
        private Vector2 _lastMoveInput;

        private void Awake()
        {
            if (playerMovement == null)
            {
                playerMovement = GetComponent<PlayerMovement>();
            }
            if (cameraPivot == null)
            {
                cameraPivot = Camera.main?.transform;
            }

            if (cameraPivot == null || playerMovement == null)
            {
                enabled = false;
                return;
            }

            _initialLocalPos = cameraPivot.localPosition;
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            float speed = _lastMoveInput.magnitude;

            Vector3 targetLocal = _initialLocalPos;

            if (playerMovement.IsGrounded && speed > moveThreshold)
            {
                float speedMultiplier = playerMovement.IsRunning ? 1.5f : 1f;
                _bobTimer += dt * bobSpeed * speedMultiplier;
                float y = Mathf.Sin(_bobTimer) * bobAmount + midpoint;
                float x = Mathf.Cos(_bobTimer * 2f) * bobSideAmount;
                targetLocal = _initialLocalPos + new Vector3(x, y, 0f);
            }
            else
            {
                _bobTimer = 0f;
            }

            cameraPivot.localPosition = 
                Vector3.SmoothDamp(cameraPivot.localPosition, targetLocal, ref _smoothVelocity, smoothTime);
        }

        public void UpdateMovement(Vector2 moveInput, bool isRunning)
        {
            _lastMoveInput = moveInput;
        }

        public void ResetToInitial()
        {
            if (cameraPivot != null)
                cameraPivot.localPosition = _initialLocalPos;
        }
    }
}
