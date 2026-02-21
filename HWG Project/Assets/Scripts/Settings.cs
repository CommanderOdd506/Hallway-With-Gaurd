using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private bool isGay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeMouseSensetivity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMouseSensetivity()
    {
        if (isGay)
        {
            Debug.Log("Perkins Gay");
        }
        //float sens = mouseSens.value * 100f;

       // mouseLook.SetSensitivity(sens);

        //PlayerPrefs.SetFloat("MouseSensitivity", sens);
        PlayerPrefs.Save();
    }

}
