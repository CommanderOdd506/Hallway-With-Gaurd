using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepPlayer : MonoBehaviour
{
    public AudioClip[] grassClips;
    public AudioClip[] stoneClips;

    public AudioSource audioSource;

    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    public float runThreshold = 3f;

    private CharacterController controller;
    private float stepTimer;
    private AudioClip[] footstepClips;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        footstepClips = grassClips; // default
        stepTimer = 0f;
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > 0.1f && controller.isGrounded)
        {
            float interval = speed > runThreshold ? runStepInterval : walkStepInterval;

            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }

    public void UseGrass()
    {
        footstepClips = grassClips;
    }

    public void UseStone()
    {
        footstepClips = stoneClips;
    }
}