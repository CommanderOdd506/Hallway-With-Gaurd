using UnityEngine;
using UnityEngine.InputSystem;

public class WinchInteractable : Interactable
{
    public WinchDoor door;
    public float rotationSpeed;

    [Header("Input")]
    public InputActionReference interactAction; // Drag your Interact action here
    

    private bool _isActive;
    private Camera _playerCamera;

    private void Start()
    {
        _playerCamera = Camera.main;
    }

    public override void BaseInteract()
    {
        _isActive = true;
    }

    void Update()
    {
        if (!_isActive) return;

        // Check if still looking at this winch
        RaycastHit hit;
        bool stillLooking = Physics.Raycast(
            _playerCamera.transform.position,
            _playerCamera.transform.forward,
            out hit,
            3f
        );

        if (!stillLooking || hit.collider.gameObject != gameObject)
        {
            StopWinch();
            return;
        }

        // This now supports keyboard + controller
        if (interactAction.action.IsPressed())
        {
            door.WinchUp();
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
        else
        {
            StopWinch();
        }
    }

    void StopWinch()
    {
        _isActive = false;
        door.StopWinch();
    }
}