using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource walkAudio;
    public AudioSource sprintAudio;
    public Rigidbody rb;

    public float movementThreshold = 0.1f;
    public float sprintSpeedThreshold = 6f;

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        bool isMoving = speed > movementThreshold;
        bool isSprinting = speed > sprintSpeedThreshold;

        if (isSprinting)
        {
            if (!sprintAudio.isPlaying)
                sprintAudio.Play();

            walkAudio.Stop();
        }
        else if (isMoving)
        {
            if (!walkAudio.isPlaying)
                walkAudio.Play();

            sprintAudio.Stop();
        }
        else
        {
            walkAudio.Stop();
            sprintAudio.Stop();
        }
    }
}