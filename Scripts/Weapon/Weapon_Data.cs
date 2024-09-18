using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Sytem/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Regular Shot")]
    public ShootType shootType;
    public float fireRate;
    public int bulletsPerShot = 1;

    [Header("Burst Shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType weaponType;
    [Range(1f, 3f)]
    public float reloadSpeed = 1f;
    [Range(1f, 3f)]
    public float equipSpeed = 1f;
    [Range(4f, 8f)]
    public float gunDistance = 4f;
    [Range(4f, 8f)]
    public float cameraDistance = 6f;
}
