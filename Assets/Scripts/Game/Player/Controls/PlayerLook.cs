using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float maxYAngle = 85f;

    private Vector2 lookInput;
    private float pitch;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetLookInput(Vector2 input) => lookInput = input;

    private void LateUpdate()
    {
        Vector2 delta = lookInput * sensitivity * Time.deltaTime;

        pitch -= delta.y;
        pitch = Mathf.Clamp(pitch, -maxYAngle, maxYAngle);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * delta.x);
    }
}