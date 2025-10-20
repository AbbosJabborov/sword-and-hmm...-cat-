using UnityEngine;
namespace Core.Game.Player.Controls
{
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 6f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;
    private bool isSliding;
    private float currentSpeed;
    private PlayerReferences refs;


    private void Awake()
    {
        refs = GetComponent<PlayerReferences>();
        controller = refs.Controller;
    	currentSpeed = walkSpeed;
    }

    // These will be called from your InputHandler (via input actions)
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetRunning(bool running) => isRunning = running;

    public void Jump()
    {
        if (isGrounded && !isSliding)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;
    }

    public void Slide()
    {
        if (isGrounded && !isSliding && isRunning)
        {
            isSliding = true;
            currentSpeed = slideSpeed;
            Invoke(nameof(EndSlide), 0.8f); // Slide duration
        }
    }

    private void EndSlide()
    {
        isSliding = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        // Choose target speed
        float targetSpeed = walkSpeed;
        if (isRunning && !isCrouching) targetSpeed = runSpeed;
        if (isCrouching) targetSpeed = crouchSpeed;
        if (isSliding) targetSpeed = slideSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        // Move direction based on camera facing
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // Stick to ground
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }
}
}