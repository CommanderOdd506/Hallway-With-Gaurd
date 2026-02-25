using System.Collections;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeOut = 2.0f;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();

            if (!audioSource.isPlaying)
            {
                audioSource.volume = 1f;
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource != null)
            {
                StartCoroutine(Fadeout(audioSource, fadeOut));
            }
        }
    }

    public static IEnumerator Fadeout(AudioSource audioSource, float fadeOut)
    {
        float startVolume = audioSource.volume;
        float timer = 0.0f;

        while (timer < fadeOut)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOut);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}