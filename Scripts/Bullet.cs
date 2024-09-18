using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private float impactForce;

    private Rigidbody rb;
    private BoxCollider cd;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float _flyDistance, float _impactForce)
    {
        this.impactForce = _impactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = _flyDistance + 0.4f;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisabledBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0f)
            ReturnBulletToPool();
    }

    private void DisabledBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2f * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRb = collision.collider.attachedRigidbody;

            enemy.GetHit();
            enemy.HitImpact(force, collision.contacts[0].point, hitRb);
        }

        CreateImpactFX(collision);
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;

            ObjectPool.instance.ReturnObject(newImpactFX, 1f);
        }
    }
}
