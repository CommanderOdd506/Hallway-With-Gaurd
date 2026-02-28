using UnityEngine;
using UnityEngine.Events;

public class UnityEventInteractable : Interactable
{
    [Header("Interaction Events")]
    [SerializeField] private UnityEvent onInteract;

    public bool requireItem = false;
    public Item itemRequired;

    public override void BaseInteract()
    {
        if (requireItem)
        {
            Inventory inventory = FindObjectOfType<Inventory>();
            if (inventory != null)
            {
                if (inventory.HasItem(itemRequired))
                {
                    onInteract?.Invoke();
                }
            }

        }
        else
        {
            onInteract?.Invoke();
        }

            
    }
}
