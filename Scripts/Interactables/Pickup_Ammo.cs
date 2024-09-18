using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    SmallBox,
    BigBox
}

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}

public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType;

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.BigBox)
            currentAmmoList = bigBoxAmmo;

        foreach(AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);   

            AddBulletsToWeapon(weapon, GetBulletsAmount(ammo));
        }

        ObjectPool.instance.ReturnObject(gameObject);
    }

    private int GetBulletsAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmounnt = Random.Range(min, max);

        return Mathf.RoundToInt(randomAmmoAmounnt);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null)
            return;

        weapon.totalReserveAmmo += amount;
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);
            if (i == ((int)boxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }

        }
    }
}
