using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class GurtAttackCollider : MonoBehaviour
{
    public string loseScene = "Lose Scene";
    public FadingScript fade;
    public float waitTime = 2f;

    public GameObject normalVoice;

    public AudioClip[] caughtClips;
    private AudioSource audioSource;
    private bool _activated = false;


    void Start()
    {
        Debug.Log(caughtClips.Length);
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_activated)
        {
            _activated = true;
            StartCoroutine(CaptureSequence());
            PlayerMovement movement = GetComponent<Collider>().GetComponent<PlayerMovement>();
            if(movement)
            {
                movement.enabled = false;
            }

            
        }
    }

    private IEnumerator CaptureSequence()
    {
        if (fade != null)
        {
            fade.FadeOut();
        }
        normalVoice.SetActive(false);
        int clipIndex = Random.Range(0, caughtClips.Length);
        audioSource.PlayOneShot(caughtClips[clipIndex]);
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(loseScene);
    }
}