using UnityEngine;
using System.Collections;

[SerializeField]
[CreateAssetMenu(menuName = "DataSets/Consumables")]
public class LootData : ScriptableObject
{
    
    public string noun = "name";
    [Range(0, 100)]
    public int dropRarity;
    [TextArea]
    public string description = "Loot description //optional";
    public GameObject item;

    [Header("Drop ammount range")]
    public int min;
    public int max;
}

