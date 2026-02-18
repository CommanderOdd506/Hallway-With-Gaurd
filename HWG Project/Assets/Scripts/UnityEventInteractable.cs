using UnityEngine;
using UnityEngine.Events;

public class UnityEventInteractable : Interactable
{
    [Header("Interaction Events")]
    [SerializeField] private UnityEvent onInteract;

    public override void BaseInteract()
    {
        onInteract?.Invoke();
    }
}
