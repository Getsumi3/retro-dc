using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance;

    public List<LootData> lootTable;
    List<string> nounsInInventory = new List<string>();

    // Use this for initialization
    void Awake()
    { 
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    LootData GetLootValue(string noun)
    {
        for (int i = 0; i < lootTable.Count; i++)
        {
            if (lootTable[i].noun == noun)
            {
                return lootTable[i];
            }
        }
        return null;
    }

    public void CalculateLoot(string noun, Vector3 pos)
    {
        for (int i = 0; i < lootTable.Count; i++)
        {
            int calc_dropChance = Random.Range(0, 101);

            if (calc_dropChance > GetLootValue(noun).dropRarity)
            {
            }

            if (calc_dropChance <= GetLootValue(noun).dropRarity)
            {
                GameObject item = Instantiate(GetLootValue(noun).item, pos, Quaternion.identity) as GameObject;
            }
        }
    }

    public float GetItem(string noun)
    {

        return Random.Range(GetLootValue(noun).min, GetLootValue(noun).max);

    }
}