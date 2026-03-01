using UnityEngine;

public class Caslte_Ambient : MonoBehaviour
{
    public AudioClip lowFire;
    private AudioSource AudioSource;


    private void Start()
    {
      AudioSource = GetComponent<AudioSource>();
        if (AudioSource == null)
        {
            AudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (lowFire != null)
        {
            AudioSource.clip = lowFire;
        }
        AudioSource.loop = true;
        AudioSource.playOnAwake = false;
        AudioSource.volume = 0.10f;
    }
     private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
           if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.Stop();
        }
    }






}
