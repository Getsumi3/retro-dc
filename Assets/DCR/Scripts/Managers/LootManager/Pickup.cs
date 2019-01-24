using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LootType { hp_potion, normal_ammo, special_ammo, heavy_ammo }
public class Pickup : MonoBehaviour
{
    public string item;
    public LootType lootType;

    private void Start()
    {
        Destroy(gameObject, 60f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            switch (lootType)
            {
                case LootType.hp_potion:

                    if (other.GetComponent<PlayerBehavior>().health >= other.GetComponent<PlayerBehavior>().startingHealth)
                    {
                        other.GetComponent<PlayerBehavior>().health = other.GetComponent<PlayerBehavior>().startingHealth;
                    }
                    else
                    {
                        other.GetComponent<PlayerBehavior>().health += LootManager.instance.GetItem(item);
                        Destroy(gameObject);
                    }

                    GameManager.UpdateHealth(other.GetComponent<PlayerBehavior>().health, other.GetComponent<PlayerBehavior>().startingHealth);
                    break;

                case LootType.normal_ammo:
                    if (other.GetComponent<PlayerBehavior>().gunController.allGuns[0].gun.projectilesRemainingInMag >= PlayerBehavior.normalProjectile)
                    {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[0].gun.projectilesRemainingInMag = PlayerBehavior.normalProjectile;
                    }
                    else {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[0].gun.projectilesRemainingInMag += LootManager.instance.GetItem(item);
                        Destroy(gameObject);
                    }
                    GameManager.UpdateAmmo(other.GetComponent<PlayerBehavior>().gunController.allGuns[0].gun.projectilesRemainingInMag, PlayerBehavior.normalProjectile, "normal_ammo");
                    break;

                case LootType.special_ammo:

                    if (other.GetComponent<PlayerBehavior>().gunController.allGuns[1].gun.projectilesRemainingInMag >= PlayerBehavior.specialProjectile)
                    {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[1].gun.projectilesRemainingInMag = PlayerBehavior.specialProjectile;
                    }
                    else {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[1].gun.projectilesRemainingInMag += LootManager.instance.GetItem(item);
                        Destroy(gameObject);
                    }
                    GameManager.UpdateAmmo(other.GetComponent<PlayerBehavior>().gunController.allGuns[1].gun.projectilesRemainingInMag, PlayerBehavior.specialProjectile, "special_ammo");
                    break;

                case LootType.heavy_ammo:

                    if (other.GetComponent<PlayerBehavior>().gunController.allGuns[2].gun.projectilesRemainingInMag >= PlayerBehavior.heavyProjectile)
                    {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[2].gun.projectilesRemainingInMag = PlayerBehavior.heavyProjectile;
                    }
                    else {
                        other.GetComponent<PlayerBehavior>().gunController.allGuns[2].gun.projectilesRemainingInMag += LootManager.instance.GetItem(item);
                        Destroy(gameObject);
                    }
                    GameManager.UpdateAmmo(other.GetComponent<PlayerBehavior>().gunController.allGuns[2].gun.projectilesRemainingInMag, PlayerBehavior.heavyProjectile, "heavy_ammo");
                    break;


            }
        }
        
    }

}
