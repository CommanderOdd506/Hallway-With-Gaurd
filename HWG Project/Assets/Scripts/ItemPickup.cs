using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item thisItem;

    public void addSelfToInventory()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null && inventory.HasSpace())
        {
            inventory.GiveItem(thisItem);


            // Immediately remove object
            Destroy(gameObject);
            
        }
    }
}