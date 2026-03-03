using UnityEngine;
using UnityEngine.SceneManagement;

public class CodePuzzle : MonoBehaviour
{
    public int[] correctCode = { 2, 5, 4, 1, 3 };

    private int _currentRequiredIndex;
    public Animator caseAnim;
    private bool _solved;
    public string winScene = "Win Scene";

    public void PressButton(int buttonNum)
    {
        if (_solved) return;

        Debug.Log("Button " + buttonNum + " pressed!");
        if(buttonNum == correctCode[_currentRequiredIndex])
        {
            Debug.Log("Button " + buttonNum + " is correct!");
            _currentRequiredIndex++;
            if (_currentRequiredIndex >= correctCode.Length)
            {
                CodeEntered();
            }
        }
        else
        {
            Debug.Log("Button " + buttonNum + " is wrong!");
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
        _solved = true;
        if (caseAnim != null)
        {
            
            caseAnim.SetTrigger("Raise");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
