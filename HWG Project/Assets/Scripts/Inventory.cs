using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public RawImage itemImage;
}
public class Inventory : MonoBehaviour
{

    public Item inventorySlot1;
    public Item inventorySlot2;

    public TextMeshProUGUI slotText1;
    public TextMeshProUGUI slotText2;

    public GameObject outline1;
    public GameObject outline2;

    private int activeSlot = 1;
    private Vector2 scroll = new Vector2();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
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
        }
        else
        {
            slotText1.text = "";
        }

        if (inventorySlot2 != null)
        {
            slotText2.text = inventorySlot2.itemName;
        }
        else
        {
            slotText2.text = "";
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
