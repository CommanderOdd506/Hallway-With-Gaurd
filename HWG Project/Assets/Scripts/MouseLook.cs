using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public Transform head;
    private float _pitch;
    public float pitchMin = -80f;
    public float pitchMax = 80f;
    public float sens = 0.3f;

    Vector2 _lookInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }
    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.instance.IsPaused()) return;

        float mouseX = _lookInput.x;
        float mouseY = _lookInput.y;

        playerBody.Rotate(Vector3.up * mouseX * sens * 0.5f);

        _pitch -= mouseY * sens * 0.5f;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
