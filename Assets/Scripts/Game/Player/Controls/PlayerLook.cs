using UnityEngine;

namespace Core.Game.Player.Controls
{
    [RequireComponent(typeof(Core.Game.Player.PlayerReferences))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private float maxYAngle = 85f;

        private PlayerReferences refs;
        private PlayerInputHandler input;
        private Transform camPivot;
        private float pitch;

        private void Awake()
        {
            refs = GetComponent<PlayerReferences>();
            input = GetComponent<PlayerInputHandler>();
            camPivot = refs.CameraPivot;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            Vector2 delta = input.LookInput * sensitivity * Time.deltaTime;

            pitch -= delta.y;
            pitch = Mathf.Clamp(pitch, -maxYAngle, maxYAngle);

            camPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.Rotate(Vector3.up * delta.x);
        }
    }
}
