using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string playScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(playScene);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
