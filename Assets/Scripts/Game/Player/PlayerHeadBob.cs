using UnityEngine;

namespace Game.Player.Controls
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

        [Header("References (optional)")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform cameraPivot;

        private Vector3 _initialLocalPos;
        private float _bobTimer;
        private Vector3 _smoothVelocity;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }
            if (cameraPivot == null)
            {
                cameraPivot = Camera.main?.transform;
            }

            if (cameraPivot == null || controller == null)
            {
                enabled = false;
                return;
            }

            _initialLocalPos = cameraPivot.localPosition;
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            Vector3 horizVel = controller.velocity;
            horizVel.y = 0f;
            float speed = horizVel.magnitude;

            Vector3 targetLocal = _initialLocalPos;

            if (controller.isGrounded && speed > moveThreshold)
            {
                _bobTimer += dt * bobSpeed * (1f + speed * 0.1f);
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

        public void ResetToInitial()
        {
            if (cameraPivot != null)
                cameraPivot.localPosition = _initialLocalPos;
        }
    }
}
