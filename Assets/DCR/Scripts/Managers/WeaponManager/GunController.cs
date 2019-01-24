using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    [System.Serializable]
    public class GunVisuals
    {
        public Gun gun;
        public GameObject arms;
    }

    //public GunVisuals[] gunVisuals;

    public Transform weaponHold;
    public GunVisuals[] allGuns;
    Gun equippedGun;
    GameObject equippedArms;

    void Start()
    {
    }

    public void EquipGun(Gun gunToEquip, GameObject armsToEquip)
    {
        armsToEquip.SetActive(true);
        equippedArms = armsToEquip;
        equippedGun = gunToEquip;
    }

    public void EquipGun(int weaponIndex)
    {
        foreach (GunVisuals gun in allGuns)
        {
            gun.arms.SetActive(false);
        }
        EquipGun(allGuns[weaponIndex].gun, allGuns[weaponIndex].arms);
    }

    public void DisableAllGuns()
    {
        foreach (GunVisuals gun in allGuns)
        {
            gun.arms.SetActive(false);
        }
    }

    public void OnTriggerHold()
    {

        equippedGun.OnTriggerHold();
    }

    public void OnTriggerRelease()
    {

        equippedGun.OnTriggerRelease();
        
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

}