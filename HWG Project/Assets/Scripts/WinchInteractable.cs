using UnityEngine;
using UnityEngine.InputSystem;

public class WinchInteractable : Interactable
{
    public WinchDoor door;
    public float rotationSpeed;

    [Header("Audio")]
    public AudioClip winch;
    [Range(0f, 1f)] public float audioVolume = 1f;

    private AudioSource audioSource;

    [Header("Input")]
    public InputActionReference interactAction;

    private bool _isActive;
    private Camera _playerCamera;

    private void Start()
    {
        _playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }
    public void StopWinchAudio()
    {
        if (audioSource && audioSource.isPlaying)
            audioSource.Stop();
    }

    public override void BaseInteract()
    {
        _isActive = true;
    }

    void Update()
    {
        if (!_isActive)
        {
            StopWinch();
            return;
        }

        // Check if player still looking at winch
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

        if (interactAction.action.IsPressed())
        {
            door.WinchUp();
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

            // Start audio if not playing
            if (audioSource && winch && !audioSource.isPlaying)
            {
                audioSource.clip = winch;
                audioSource.volume = audioVolume;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            StopWinch();
        }
    }

    void StopWinch()
    {
        // stop movement
        door.StopWinch();

        // stop audio
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        _isActive = false;
    }
}