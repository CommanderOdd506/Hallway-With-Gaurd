using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


public class Inventory : MonoBehaviour
{

    public Item inventorySlot1;
    public Item inventorySlot2;

    public TextMeshProUGUI slotText1;
    public TextMeshProUGUI slotText2;

    public GameObject[] viewModelReferences;
    public GameObject[] droppedItemPrefabs;
    public Transform dropItemSpot;

    public RawImage slotImage1;
    public RawImage slotImage2;

    public GameObject outline1;
    public GameObject outline2;

   

    private int activeSlot = 1;
    private Vector2 scroll = new Vector2();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
        UpdateViewModel();
    }

    void OnDrop(InputValue value)
    {
        if (value.isPressed)
        {
            DropItemBySlot(activeSlot);
            Debug.Log("Dropped item in slot " + activeSlot);
        }
    }

    public void GiveItem(Item item)
    {
        if (inventorySlot1 == null)
        {
            inventorySlot1 = item;
            UpdateUI();
            UpdateViewModel();
        }
        else if (inventorySlot2 == null)
        {
            inventorySlot2 = item;
            UpdateUI();
            UpdateViewModel();
        }
        else
        {
            Debug.Log("Inventory Full");
        }
    }

    public void DropItemBySlot(int slot)
    {
        if (slot == 1 && inventorySlot1 != null)
        {
            SpawnItem(inventorySlot1);
            inventorySlot1 = null;
            UpdateUI();
            UpdateViewModel();
            
        }
        else if (slot == 2 && inventorySlot2 != null)
        {
            SpawnItem(inventorySlot2);
            inventorySlot2 = null;
            UpdateUI();
            UpdateViewModel();
            
        }
        else
        {
            Debug.Log("Invalid slot number");
        }
    }

    public void DropItem(Item item)
    {
        if (inventorySlot1 == item) 
        {
            SpawnItem(inventorySlot1);
            inventorySlot1 = null;
            UpdateUI();
            UpdateViewModel();
            
        }
        else if (inventorySlot2 == item)
        {
            SpawnItem(inventorySlot2);
            inventorySlot2 = null;
            UpdateUI();
            UpdateViewModel();
            
        }
        else
        {
            Debug.Log("Item not in inventory");
        }

    }

    private void SpawnItem(Item item)
    {
        if (item == null || dropItemSpot == null) return;

        Quaternion slightRotation = Quaternion.Euler(0f, 15f, 0f);
        Instantiate(droppedItemPrefabs[item.referenceIndex], dropItemSpot.position, slightRotation);
    }

    void OnCycleSlot(InputValue value)
    {
        if (PauseMenu.instance.IsPaused()) return;

        float direction = value.Get<float>();

        if (direction > 0f)
        {
            activeSlot = activeSlot == 1 ? 2 : 1;
            UpdateUI();
            UpdateViewModel();
        }
        else if (direction < 0f)
        {
            activeSlot = activeSlot == 1 ? 2 : 1;
            UpdateUI();
            UpdateViewModel();
        }
    }

    void UpdateViewModel()
    {
        Item currentItem = activeSlot == 1 ? inventorySlot1 : inventorySlot2;

        for (int i = 0; i < viewModelReferences.Length; i++)
        {
            viewModelReferences[i].SetActive(false);
        }


        if (currentItem == null)
            return;

        if (currentItem.referenceIndex < 0 || currentItem.referenceIndex >= viewModelReferences.Length)
            return;

        viewModelReferences[currentItem.referenceIndex].SetActive(true);
    }


    void UpdateUI()
    {
        if (inventorySlot1 != null)
        {
            slotText1.text = inventorySlot1.itemName;
            slotImage1.color = new Color(1f, 1f, 1f, 1f);
            slotImage1.texture = inventorySlot1.itemImage;  
        }
        else
        {
            slotText1.text = "";
            slotImage1.color = new Color(1f, 1f, 1f, 0f);
            slotImage1.texture = null;
        }

        if (inventorySlot2 != null)
        {
            slotText2.text = inventorySlot1.itemName;
            slotImage2.color = new Color(1f, 1f, 1f, 1f);
            slotImage2.texture = inventorySlot2.itemImage;
        }
        else
        {
            slotText2.text = "";
            slotImage2.color = new Color(1f, 1f, 1f, 0f);
            slotImage2.texture = null;
        }
        outline1.SetActive(false);
        outline2.SetActive(false);
        if (activeSlot == 1)
        {
            outline1.SetActive(true);
        }
        else
        {
            outline2.SetActive(true);
        }
    }
}
