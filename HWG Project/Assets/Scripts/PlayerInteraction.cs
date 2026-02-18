using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionRange = 3f;

    private bool _interactPressed;
    private Camera playerCamera;


    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (!_interactPressed)
            return;

        _interactPressed = false; // consume input immediately

        RaycastHit hit;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            interactionRange,
            interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                Debug.Log("Ineracted");
                interactable.BaseInteract();
            }
        }
    }

    void OnInteract(InputValue value)
    {

        Debug.Log("Interact pressed");
        if (value.isPressed)
        {
            _interactPressed = true;
        }
    }

    void OnDrawGizmos()
    {
        Camera cam = playerCamera != null ? playerCamera : GetComponentInChildren<Camera>();
        if (cam == null) return;

        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, interactionRange, interactableLayer))
        {
            Gizmos.color = Color.green; // Hitting something
        }
        else
        {
            Gizmos.color = Color.red; // Not hitting anything
        }

        Gizmos.DrawRay(origin, direction * interactionRange);
    }
}
