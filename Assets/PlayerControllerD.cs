using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterSetting settings;
    public Transform cameraTarget; // Assign this to a camera follow target

    private Vector2 moveInput;
    private float mouseX;
    private float verticalVelocity;
    private bool isJumping;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        // Rotate player horizontally
        transform.Rotate(0f, mouseX * settings.rotationSpeed * Time.deltaTime, 0f);
    }

    private void HandleMovement()
    {
        // Horizontal movement
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        move *= settings.moveSpeed;

        ApplyGravity();

        // Apply vertical velocity
        move.y = verticalVelocity;

        // Move player using CharacterController
        controller.Move(move * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (isJumping)
            {
                verticalVelocity = settings.jumpForce;
                isJumping = false;
            }
            else
            {
                verticalVelocity = -2f; // small downward force to stay grounded
            }
        }
        else
        {
            verticalVelocity += settings.gravity * Time.deltaTime;
        }
    }

    // Input callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        Vector2 lookDelta = value.Get<Vector2>();
        mouseX = lookDelta.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            isJumping = true;
        }
    }
}
