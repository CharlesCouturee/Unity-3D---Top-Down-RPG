using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim Control")]
    [SerializeField] Transform aim;

    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private bool isLockingToTarget;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1f)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 2.6f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensitivity = 5f;

    [Space]

    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHitInfo;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            isAimingPrecisely = !isAimingPrecisely;

        if (Input.GetKeyDown(KeyCode.L))
            isLockingToTarget = !isLockingToTarget;

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        aimLaser.enabled = player.weaponController.WeaponReady();
        if (!aimLaser.enabled)
            return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weaponController.GunPoint();
        Vector3 laserDirection = player.weaponController.BulletDirection();

        float laserTipLength = 0.5f;
        float gunDistance = player.weaponController.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();
        if (target != null && isLockingToTarget)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
                aim.position = renderer.bounds.center;
            else
                aim.position = target.position;

            return;
        }

        aim.position = GetMouseHitInfo().point;
        if (!isAimingPrecisely)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHitInfo = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHitInfo;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }

    #region Camera
    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + (aimDirection * clampedDistance);
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }
    #endregion

    public Transform Target()
    {
        Transform target = null;
        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aim;
    public bool CanAimPrecisely() => isAimingPrecisely;
}
