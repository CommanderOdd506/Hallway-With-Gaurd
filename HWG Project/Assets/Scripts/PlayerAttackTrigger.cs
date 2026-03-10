using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PlayerAttackTrigger : MonoBehaviour
{
    public Collider meleeCollider;
    public float activationTime;
    public AudioSource audioSource;
    public AudioClip swingClip;

    public void ActivateCollider()
    {
        meleeCollider.enabled = true;
        StartCoroutine(ColliderActivationPeriod());
    }

    public void PlayAudioAxe()
    {
        audioSource.PlayOneShot(swingClip);
    }

    private IEnumerator ColliderActivationPeriod()
    {
        yield return new WaitForSeconds(activationTime);

        meleeCollider.enabled = false;
    }
}
