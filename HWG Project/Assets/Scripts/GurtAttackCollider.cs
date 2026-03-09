using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GurtAttackCollider : MonoBehaviour
{
    public string loseScene = "Lose Scene";
    public FadingScript fade;
    public float waitTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CaptureSequence());
        }
    }

    private IEnumerator CaptureSequence()
    {
        if (fade != null)
        {
            fade.FadeOut();
        }

        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(loseScene);
    }
}