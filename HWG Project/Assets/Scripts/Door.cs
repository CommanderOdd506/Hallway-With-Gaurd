using UnityEngine;
using UnityEngine.Audio;

public class Door : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip doorClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PlayAudioDoor()
    {
        audioSource.PlayOneShot(doorClip);
    }
}
