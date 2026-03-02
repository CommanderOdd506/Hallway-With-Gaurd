using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerEvent3D : MonoBehaviour
{
    [Header("Filtering")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool triggerOnlyOnce = false;
    public bool hasCooldown;
    public float cooldown = 0.5f;

    [Header("Events")]
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private bool hasTriggered = false;
    private bool canTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        // Block if single-use and already triggered
        if (triggerOnlyOnce && hasTriggered)
            return;

        // Block if cooldown is active
        if (!canTrigger)
            return;

        hasTriggered = true;
        onPlayerEnter?.Invoke();

        if (hasCooldown)
            StartCoroutine(Cooldown());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            onPlayerExit?.Invoke();
        }
    }

    private IEnumerator Cooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(cooldown);
        canTrigger = true;
    }
}