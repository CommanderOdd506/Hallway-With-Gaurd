using UnityEngine;
using TMPro;

public class Return : MonoBehaviour
{

    public GameObject ReturnToArea;

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player")) ;
        {
            ReturnToArea.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CompareTag("Player")) ;
        {
            ReturnToArea.SetActive(false);
        }
        
    }


}
