using UnityEngine;

public class WinchDoor : MonoBehaviour
{
    public Transform door;
    public Transform topPosition;
    public Transform bottomPosition;
    public WinchInteractable winchInteractable;

    public float raiseSpeed = 2f;
    public float fallSpeed = 3f;

    [Header("Door Audio")]
    public AudioClip moveClip;           // single clip for both directions
    [Range(0f, 1f)] public float moveVolume = 1f;

    private AudioSource audioSource;

    private float _progress;
    private bool _raising;
    private bool _locked;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_locked) return;

        bool isMoving = false;

        if (_raising)
        {
            _progress += Time.deltaTime * raiseSpeed;
            isMoving = true;
        }
        else if (_progress > 0f)
        {
            _progress -= Time.deltaTime * fallSpeed;
            isMoving = true;
        }

        _progress = Mathf.Clamp01(_progress);

        door.position = Vector3.Lerp(
            bottomPosition.position,
            topPosition.position,
            _progress
        );

        // Handle audio
        if (audioSource && moveClip)
        {
            if (isMoving && !audioSource.isPlaying)
            {
                audioSource.clip = moveClip;
                audioSource.volume = moveVolume;
                audioSource.loop = true;
                audioSource.Play();
            }
            else if (!isMoving && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Door fully raised
        if (_progress >= 1f)
        {
            _locked = true;

            // Stop audio
            if (audioSource && audioSource.isPlaying)
                audioSource.Stop();

            // Stop winch interactable
            if (winchInteractable != null)
            {
                winchInteractable.StopWinchAudio();
                winchInteractable.enabled = false;
            }
        }
    }

    public void WinchUp()
    {
        if (_locked) return;
        _raising = true;
    }

    public void StopWinch()
    {
        _raising = false;
    }
}