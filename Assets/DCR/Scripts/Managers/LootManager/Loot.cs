using UnityEngine;
using System.Collections;

public class Loot : MonoBehaviour
{

    public string[] items;

    public void DropItem(Vector3 pos)
    {

        LootManager.instance.CalculateLoot(items[Random.Range(0, items.Length)], pos);   
    }
}
