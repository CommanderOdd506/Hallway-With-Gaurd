using UnityEngine;
using UnityEngine.Audio;

public class EatAnimation : MonoBehaviour
{
    public GameObject viewModelApple;
    public GameObject eatApple;
    public Inventory inventory;
    public Item apple;
    private AudioSource audioSource;
    public AudioClip appleMunch;
    public PlayerMovement playerMovement;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SpawnEatApple()
    {
        inventory.SetAnimating(true);
        eatApple.SetActive(true);
        viewModelApple.SetActive(false);
    }

    public void PlayAudio()
    {
        audioSource.PlayOneShot(appleMunch);
    }

    public void DespawnEatApple()
    {
        eatApple.SetActive(false);
        inventory.RemoveItem(apple);
        inventory.SetAnimating(false);
        playerMovement.EatApple();
    }


}
