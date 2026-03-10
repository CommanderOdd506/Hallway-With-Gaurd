using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


public class Inventory : MonoBehaviour
{
    [SerializeField] private float scrollThreshold = 0.5f;
    [SerializeField] private float scrollCooldown = 0.2f;

    private float lastScrollTime;

    public Item inventorySlot1;
    public Item inventorySlot2;
    public GameObject arms;
    public Animator armsAnimator;

    public TextMeshProUGUI slotText1;
    public TextMeshProUGUI slotText2;

    public GameObject[] viewModelReferences;
    public GameObject[] droppedItemPrefabs;
    public Transform dropItemSpot;

    public RawImage slotImage1;
    public RawImage slotImage2;

    public GameObject outline1;
    public GameObject outline2;

    private Item currentlyEquippedItem;
    private bool _isAnimating;
    private PlayerMovement playerMovement;

    private int activeSlot = 1;
    private Vector2 scroll = new Vector2();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
        UpdateViewModel();
        playerMovement = GetComponent<PlayerMovement>();
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
            SetArmVisibility();
        }
        else if (inventorySlot2 == null)
        {
            inventorySlot2 = item;
            UpdateUI();
            UpdateViewModel();
            SetArmVisibility();
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
            SetArmVisibility();
        }
        else if (slot == 2 && inventorySlot2 != null)
        {
            SpawnItem(inventorySlot2);
            inventorySlot2 = null;
            UpdateUI();
            UpdateViewModel();
            SetArmVisibility();
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
            SetArmVisibility();
        }
        else if (inventorySlot2 == item)
        {
            SpawnItem(inventorySlot2);
            inventorySlot2 = null;
            UpdateUI();
            UpdateViewModel();
            SetArmVisibility();
        }
        else
        {
            Debug.Log("Item not in inventory");
        }

    }

    public void RemoveItem(Item item)
    {
        if (inventorySlot1 == item)
        {
            inventorySlot1 = null;
            UpdateUI();
            UpdateViewModel();
            SetArmVisibility();
        }
        else if (inventorySlot2 == item)
        {
            inventorySlot2 = null;
            UpdateUI();
            UpdateViewModel();
            SetArmVisibility();
        }
        else
        {
            Debug.Log("Item not in inventory");
        }

    }

    public bool HasItem(Item item)
    {
        if (inventorySlot1 == item && activeSlot == 1)
        {
            return true;
        }
        else if (inventorySlot2 == item && activeSlot == 2)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool HasSpace()
    {
        if (inventorySlot1 != null && inventorySlot2 != null)
        {
            return false;
        }
        else
        {
            return true;
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
        if (_isAnimating) return;

        float direction = value.Get<float>();

        // Ignore tiny scroll values (high-res mouse wheels)
        if (Mathf.Abs(direction) < scrollThreshold)
            return;

        // Prevent rapid swapping
        if (Time.time - lastScrollTime < scrollCooldown)
            return;

        lastScrollTime = Time.time;

        activeSlot = activeSlot == 1 ? 2 : 1;

        UpdateUI();
        UpdateViewModel();
        SetArmVisibility();
    }

    void OnAttack(InputValue value)
    {
        if (PauseMenu.instance.IsPaused()) return;
        if(_isAnimating) return;

        if (currentlyEquippedItem.canAttack)
        {
            armsAnimator.SetTrigger("Hit");
        }

        if (currentlyEquippedItem.canEat)
        {
            armsAnimator.SetTrigger("Eat");
        }

    }

    public void SetAnimating(bool animating)
    {
        _isAnimating = animating;
    }

    void UpdateViewModel()
    {
        Item currentItem = activeSlot == 1 ? inventorySlot1 : inventorySlot2;

        // If nothing changed, do nothing
        if (currentItem == currentlyEquippedItem)
            return;

        currentlyEquippedItem = currentItem;

        // Turn everything off
        for (int i = 0; i < viewModelReferences.Length; i++)
        {
            viewModelReferences[i].SetActive(false);
        }

        if (currentItem == null)
            return;

        if (currentItem.referenceIndex < 0 || currentItem.referenceIndex >= viewModelReferences.Length)
            return;

        viewModelReferences[currentItem.referenceIndex].SetActive(true);

        armsAnimator.SetTrigger("Draw");
    }

    void SetArmVisibility()
    {
        if (activeSlot == 1 && inventorySlot1 != null)
        {
            arms.SetActive(true);
        }
        else if(activeSlot == 2 && inventorySlot2 != null)
        {
            arms.SetActive(true);
        }
        else
        {
            arms.SetActive(false);
        }
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
            slotText2.text = inventorySlot2.itemName;
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
