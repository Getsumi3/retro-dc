using UnityEngine;
using System.Collections;

[SerializeField]
[CreateAssetMenu(menuName = "DataSets/DungeonSet")]
public class DungeonData : ScriptableObject
{

    public string noun = "Setup name // optional";
    [TextArea]
    public string description = "Setup description // optional"; 

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] propsTiles;
    public GameObject[] outerWallTiles;
}
