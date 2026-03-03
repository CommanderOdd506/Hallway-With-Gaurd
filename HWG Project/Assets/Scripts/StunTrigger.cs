using UnityEngine;

public class StunTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("hit something");
        Guard gurt = collider.GetComponent<Guard>();

        if (gurt)
        {
            gurt.Stun();
        }

    }
}
