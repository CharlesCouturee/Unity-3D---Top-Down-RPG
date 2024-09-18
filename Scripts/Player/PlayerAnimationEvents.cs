using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    private void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
        weaponController.SetWeaponReady(true);
    }

    private void ReturnRig()
    {
        visualController.MaximizeRigWeight();    
        visualController.MaximizeLeftHandWeight();
    }

    private void WeaponEquipIsOver() => weaponController.SetWeaponReady(true);
    public void SwitchOnWeaponModel() => visualController.SwitchOnCurrentWeaponModel();
}
