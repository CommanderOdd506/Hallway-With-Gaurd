using UnityEngine;
using UnityEngine.SceneManagement;

public class CodePuzzle : MonoBehaviour
{
    public int[] correctCode = { 2, 5, 4, 1, 3 };

    private int _currentRequiredIndex;
    public Animator caseAnim;
    private bool _solved;
    public string winScene = "Win Scene";
    public GameObject goldenDuck;
    public AudioClip correct;
    public AudioClip incorrect;
    public AudioClip solved;

    [Range(0f, 1f)] public float audioVolume = 1.0f;

    private AudioSource AudioSource;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void PressButton(int buttonNum)
    {
        if (_solved) return;

        Debug.Log("Button " + buttonNum + " pressed!");
        if (buttonNum == correctCode[_currentRequiredIndex])
        {
            Debug.Log("Button " + buttonNum + " is correct!");

            if (AudioSource && correct)
                AudioSource.PlayOneShot(correct, audioVolume);



            _currentRequiredIndex++;
            if (_currentRequiredIndex >= correctCode.Length)
            {
                CodeEntered();
            }
        }
        else
        {
            Debug.Log("Button " + buttonNum + " is wrong!");

            if (AudioSource && incorrect)
            {
                AudioSource.PlayOneShot(incorrect, audioVolume);

            }

            _currentRequiredIndex = 0;
        }
    }

    public void WinGame()
    {
        SceneManager.LoadScene(winScene);

    }

    private void CodeEntered()
    {
        Debug.Log("Correct code Entered!");
        goldenDuck.layer = 6;
        _solved = true;
        if (AudioSource && solved)
            AudioSource.PlayOneShot(solved, audioVolume);


        if (caseAnim != null)
        {

            caseAnim.SetTrigger("Raise");
        }
    }


}