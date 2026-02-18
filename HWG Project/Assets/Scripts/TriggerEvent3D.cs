using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent3D : MonoBehaviour
{
    [Header("Filtering")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool triggerOnlyOnce = false;

    [Header("Events")]
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            onPlayerExit?.Invoke();
        }
    }
}
