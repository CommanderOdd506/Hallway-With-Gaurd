using UnityEngine;
using UnityEngine.Audio;

public class ItemPickup : MonoBehaviour
{
    public Item thisItem;
    
    public AudioSource audioSource;
    public AudioClip axeClip;

public void addSelfToInventory()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null && inventory.HasSpace())
        {
            inventory.GiveItem(thisItem);

            if (thisItem.itemName == "Axe")
            {
                audioSource.PlayOneShot(axeClip);
            }
            // Immediately remove object
            Destroy(gameObject);
            
        }
    }
}