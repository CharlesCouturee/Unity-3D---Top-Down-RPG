using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;

    protected PlayerWeaponController weaponController;
    protected MeshRenderer mesh;
    protected Material defaultMaterial;

    private void Start()
    {
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
