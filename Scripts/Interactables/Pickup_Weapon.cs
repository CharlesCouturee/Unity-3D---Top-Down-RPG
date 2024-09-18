using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Weapon weapon;

    [SerializeField] private BackupWeaponModel[] models;

    private bool oldWeapon;

    private void Start()
    {
        if (!oldWeapon)
            weapon = new Weapon(weaponData);

        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + new Vector3(0, 0.75f, 0f);
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        SetupItemModel();
    }

    private void SetupItemModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);

            if (model.weaponType == weapon.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnObject(gameObject);
    }
}
