using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator anim;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Traget;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool shouldIncrease_LeftHandIKWeight;

    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();    
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weaponController.CurrentWeapon().weaponType;
        foreach (WeaponModel model in weaponModels)
        {
            if (model.weaponType == weaponType)
                weaponModel = model;
        }

        return weaponModel;
    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire");

    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.weaponController.CurrentWeapon().reloadSpeed;

        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }


    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipSpeed = player.weaponController.CurrentWeapon().equipSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();

        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipSpeed);
        anim.SetTrigger("EquipWeapon");
    }

    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach(BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == player.weaponController.CurrentWeapon().weaponType)
                continue;

            if (player.weaponController.WeaponInSlots(backupModel.weaponType) != null)
            {
                if (backupModel.HangTypeIs(HangType.LowBackHang))
                    lowHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.BackHang))
                    backHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.SideHang))
                    sideHangWeapon = backupModel;
            }
        }

        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in backupWeaponModels)
            backupModel.Activate(false);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = (int)(CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (player.weaponController.HasOnlyOneWeapon() == false)
            SwitchOnBackupWeaponModel();

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        foreach (WeaponModel model in weaponModels)
            model.gameObject.SetActive(false);
    }

    private void SwitchAnimationLayer(int _layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(_layerIndex, 1);
    }

    #region Animation Rigging Methods
    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncrease_LeftHandIKWeight = false;
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncrease_RigWeight = false;
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        //leftHandIK_Traget.localPosition = targetTransform.localPosition;
        //leftHandIK_Traget.localRotation = targetTransform.localRotation;
    }

    public void MaximizeRigWeight() => shouldIncrease_RigWeight = true;
    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIKWeight = true;
    #endregion
}
