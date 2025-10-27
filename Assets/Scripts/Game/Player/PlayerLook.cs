using UnityEngine;

namespace Game.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private float maxYAngle = 85f;
        
        private Vector2 _lookInput;
        private float _pitch;

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetLookInput(Vector2 input) => _lookInput = input;

        private void LateUpdate()
        {
            Vector2 delta = _lookInput * (sensitivity * Time.deltaTime);

            _pitch -= delta.y;
            _pitch = Mathf.Clamp(_pitch, -maxYAngle, maxYAngle);

            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            transform.Rotate(Vector3.up * delta.x);
        }
    }
}