using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public MouseLook mouseLook;
    public Toggle fullscreenToggle;

    public Slider mouseSens;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeMouseSensetivity();
    }
    public void OnEnable()
    {
        LoadUISettings();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadUISettings()
    {
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", .3f);
        mouseSens.value = savedSens;

        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        bool isFullscreen = savedFullscreen == 1;
    }
    public void ChangeMouseSensetivity()
    {

        float sens = mouseSens.value;

       mouseLook.SetSensitivity(sens);

        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        PlayerPrefs.Save();
    }
    public void ChangeFullscreen()
    {
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
