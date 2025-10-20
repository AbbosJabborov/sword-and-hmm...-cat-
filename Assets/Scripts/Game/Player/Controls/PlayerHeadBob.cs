using UnityEngine;

namespace Core.Game.Player.Controls
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

        private Vector3 initialLocalPos;
        private float bobTimer;
        private Vector3 smoothVelocity;

        private void Awake()
        {
            // Try assigned refs first, then fall back to common lookups
            if (controller == null)
            {
                controller = GetComponent<CharacterController>()
                             ?? GetComponentInChildren<CharacterController>()
                             ?? GetComponentInParent<CharacterController>();
            }

            if (cameraPivot == null)
            {
                // Prefer an explicitly named pivot child
                var pivot = transform.Find("CameraPivot");
                if (pivot != null) cameraPivot = pivot;

                // If not found, prefer the main camera's parent (common setup)
                if (cameraPivot == null && Camera.main != null)
                    cameraPivot = Camera.main.transform.parent ?? Camera.main.transform;

                // Last resort: any Camera inside this object
                if (cameraPivot == null)
                {
                    var cam = GetComponentInChildren<Camera>();
                    if (cam != null) cameraPivot = cam.transform.parent ?? cam.transform;
                }
            }

            if (cameraPivot == null || controller == null)
            {
                Debug.LogWarning("PlayerHeadBob: Missing CharacterController or Camera pivot. Disabling.");
                enabled = false;
                return;
            }

            initialLocalPos = cameraPivot.localPosition;
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            Vector3 horizVel = controller.velocity;
            horizVel.y = 0f;
            float speed = horizVel.magnitude;

            Vector3 targetLocal = initialLocalPos;

            if (controller.isGrounded && speed > moveThreshold)
            {
                bobTimer += dt * bobSpeed * (1f + speed * 0.1f);
                float y = Mathf.Sin(bobTimer) * bobAmount + midpoint;
                float x = Mathf.Cos(bobTimer * 2f) * bobSideAmount;
                targetLocal = initialLocalPos + new Vector3(x, y, 0f);
            }
            else
            {
                bobTimer = 0f;
            }

            cameraPivot.localPosition =
                Vector3.SmoothDamp(cameraPivot.localPosition, targetLocal, ref smoothVelocity, smoothTime);
        }

        public void ResetToInitial()
        {
            if (cameraPivot != null)
                cameraPivot.localPosition = initialLocalPos;
        }
    }
}
