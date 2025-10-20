using UnityEngine;

namespace Core.Game.Player.Controls
{
    [RequireComponent(typeof(PlayerReferences))]
    public class PlayerHeadBob : MonoBehaviour
    {
        [Header("Bob Settings")]
        [SerializeField] private float bobSpeed = 8f;
        [SerializeField] private float bobAmount = 0.05f;
        [SerializeField] private float bobSideAmount = 0.02f;
        [SerializeField] private float midpoint = 0.0f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private float moveThreshold = 0.1f;

        private PlayerReferences refs;
        private Transform cameraPivot;
        private CharacterController controller;
        private Vector3 initialLocalPos;
        private float bobTimer;
        private Vector3 smoothVelocity;

        private void Awake()
        {
            refs = GetComponent<PlayerReferences>();
            controller = refs.Controller;
            cameraPivot = refs.CameraPivot;

            if (cameraPivot == null || controller == null)
            {
                Debug.LogWarning("PlayerHeadBob: Missing references. Disabling.");
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

        public void ResetToInitial() =>
            cameraPivot.localPosition = initialLocalPos;
    }
}
