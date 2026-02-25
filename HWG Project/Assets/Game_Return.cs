using UnityEngine;
using TMPro;

public class Game_Return : MonoBehaviour
{

    public GameObject textObject;


    private void Start()
    {
        textObject.SetActive(false);
    }




    private void OnTriggerEnter(Collider other)
    {
       
        if (CompareTag("Player")) ;
        {
            textObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CompareTag("Player")) ;
        {
            textObject.SetActive(false);
        }
        
    }
    
    

}
