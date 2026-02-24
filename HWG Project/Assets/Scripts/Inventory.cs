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
        }
        else if (inventorySlot2 == null)
        {
            inventorySlot2 = item;
            UpdateUI();
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
            inventorySlot1 = null;
            UpdateUI();
        }
        else if (slot == 2 && inventorySlot2 != null)
        {
            inventorySlot2 = null;
            UpdateUI();
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
            inventorySlot1 = null;
            UpdateUI();
        }
        else if (inventorySlot2 == item)
        {
            inventorySlot2 = null;
            UpdateUI();
        }
        else
        {
            Debug.Log("Item not in inventory");
        }

    }

    void OnCycleSlot(InputValue value)
    {
        if (PauseMenu.instance.IsPaused()) return;

        float direction = value.Get<float>();

        if (direction > 0f)
        {
            activeSlot = activeSlot == 1 ? 2 : 1;
            UpdateUI();
        }
        else if (direction < 0f)
        {
            activeSlot = activeSlot == 1 ? 2 : 1;
            UpdateUI();
        }
    }
    // Update is called once per frame
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
