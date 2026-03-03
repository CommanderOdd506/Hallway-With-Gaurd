using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item thisItem;
    public AudioClip pickupSound;

    public void addSelfToInventory()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null && inventory.HasSpace())
        {
            inventory.GiveItem(thisItem);

            // Play sound at position even after object is destroyed
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Immediately remove object
            Destroy(gameObject);
            
        }
    }
}