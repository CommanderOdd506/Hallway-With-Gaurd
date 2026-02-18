using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionRange = 3f;

    public GameObject interactPrompt;

    private bool _interactPressed;
    private Camera playerCamera;


    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (PauseMenu.instance.IsPaused()) return;

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            interactionRange,
            interactableLayer
        );

        if (hitSomething)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {

                if (interactPrompt != null && !interactPrompt.activeSelf)
                    interactPrompt.SetActive(true);

                if (_interactPressed)
                {
                    _interactPressed = false;
                    interactable.BaseInteract();
                    Debug.Log("Interacted with: " + interactable.name);
                }
            }
            else
            {
                if (interactPrompt != null && interactPrompt.activeSelf)
                    interactPrompt.SetActive(false);
            }
        }
        else
        {

            if (interactPrompt != null && interactPrompt.activeSelf)
                interactPrompt.SetActive(false);
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
