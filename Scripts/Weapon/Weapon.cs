using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single, 
    Auto
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;
    public ShootType shootType;

    #region Regular Mode Variables
    public int bulletsPerShot { get; private set; }
    private float defaultFireRate;
    public float fireRate = 1f;
    private float lastShootTime;
    #endregion

    #region Burst Mode Variables
    private bool burstAvailable;
    public bool burstActive;
    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Weapon Spread Variables
    [Header("Spread")]
    private float baseSpread = 1f;
    private float currentSpread = 2f;
    public float maximumSpread = 3f;
    private float spreadIncreaseRate = 0.15f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1f;
    #endregion

    #region Weapon Generic Info
    public float reloadSpeed { get; private set; }
    public float equipSpeed { get; private set; }
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    public Weapon_Data weaponData { get; private set; } // serves as default weapon data

    public Weapon(Weapon_Data weaponData)
    {
        fireRate = weaponData.fireRate;
        defaultFireRate = fireRate;
        weaponType = weaponData.weaponType;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipSpeed = weaponData.equipSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        bulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        this.weaponData = weaponData;
    }

    #region Burst Methods
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0f;
            return true;
        }

        return burstActive;
    }
    public void ToggleBurst()
    {
        if (burstAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }
    #endregion

    #region Spread Methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void IncreaseSpread() => currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }
    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1f / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload Methods
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
            return false;

        if (totalReserveAmmo > 0)
            return true;

        return false;
    }

    public void RefillBullets()
    {
        int bulletToReload = magazineCapacity;

        if (bulletToReload > totalReserveAmmo)
            bulletToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletToReload;
        bulletsInMagazine = bulletToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }
    #endregion
}
