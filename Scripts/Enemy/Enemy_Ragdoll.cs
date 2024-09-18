using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidBodies;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();

        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (Rigidbody rb in ragdollRigidBodies)
        {
            rb.isKinematic = !active;
        }
    }

    public void CollidersActive(bool active)
    {
        foreach (Collider cd in ragdollColliders)
            cd.enabled = active;
    }
}
