using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _moveInput;
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction sprintAction;

    public float walkSpeed;
    public float sprintSpeed;
    private bool jumpPressed;
    public float jumpBuffer = 0.12f;
    public float gravity = -30f;
    public float groundStickForce = -5f;
    public float jumpHeight = 1.1f;

    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 20f;
    public float staminaRegenPerSecond = 15f;

    public Slider staminaSlider;

    private float _currentStamina;
    private float _timeSinceJumpPressed = 0.13f;
    private Vector3 _velocity;
    private float _currentSpeed;
    private bool _isSprinting;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
        _currentStamina = maxStamina;
    }

    void Update()
    {
        if (PauseMenu.instance.IsPaused()) return;

        bool sprintHeld = sprintAction.IsPressed();
        bool canSprint = _currentStamina > 0f;

        _isSprinting = sprintHeld && canSprint;

        if (_isSprinting)
        {
            _currentStamina -= staminaDrainPerSecond * Time.deltaTime;
        }
        else
        {
            _currentStamina += staminaRegenPerSecond * Time.deltaTime;
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0f, maxStamina);

        if (staminaSlider != null)
        {
            staminaSlider.value = _currentStamina / maxStamina;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        bool isGrounded = controller.isGrounded;

        _timeSinceJumpPressed += Time.deltaTime;

        bool bufferedJump = _timeSinceJumpPressed <= jumpBuffer;

        float targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, 20f * Time.deltaTime);

        if (bufferedJump && isGrounded)
        {
            _timeSinceJumpPressed = jumpBuffer + 1f;
            _velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }

        if (isGrounded && _velocity.y < 0f)
            _velocity.y = groundStickForce;
        else
            _velocity.y += gravity * Time.deltaTime;

        Vector3 worldDirection = (forward * _moveInput.y + right * _moveInput.x);
        worldDirection = worldDirection.normalized;
        Vector3 horizontal = worldDirection * _currentSpeed;

        Vector3 motion = horizontal + new Vector3(0f, _velocity.y, 0f);
        controller.Move(motion * Time.deltaTime);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            _timeSinceJumpPressed = 0f;
        }
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
}
