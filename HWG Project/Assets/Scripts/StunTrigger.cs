using UnityEngine;

public class StunTrigger : MonoBehaviour
{
    private Inventory inventory;
    public Item axeItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("hit something");
        Guard gurt = collider.GetComponent<Guard>();
        UnityEventInteractable interactable = collider.GetComponent<UnityEventInteractable>();
        if (gurt)
        {
            gurt.Stun();
        }

        if (interactable && inventory.HasItem(axeItem))
        {
            interactable.BaseInteract();
        }

    }
}
