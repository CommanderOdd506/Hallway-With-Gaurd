using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Texture itemImage;
    public int referenceIndex = -1;
}

