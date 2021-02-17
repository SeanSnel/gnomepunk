using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GnomeMover : MonoBehaviour, IExplodable
{
    private Rigidbody rb;


    public float acceleration = 5;
    public float maxSpeed = 2;
    public float MaxSpeedSqr { get; private set; }
    public LayerMask gnomeMask = 0;
    public float overlapRadius;
    public float overlapFactor;

    [HideInInspector]
    public float xPos;
    [HideInInspector]
    public float yPos;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MaxSpeedSqr = Mathf.Pow(maxSpeed, 2);
    }

    public void RunAwayFrom(Vector3 position, float radius)
    {
        float distance = Vector3.Distance(rb.position, position);
        Vector3 pushDirection = (rb.position - position);
        pushDirection.y = 0;
        pushDirection.z = 0;
        pushDirection = pushDirection.normalized;
        Vector3 pushForce = pushDirection * acceleration * (-Mathf.Pow(distance.Remap(0, radius, 0, 1), 2) + 1);
        if (rb.velocity.sqrMagnitude < MaxSpeedSqr)
        {
            rb.AddForce(pushForce, ForceMode.Acceleration);
        }
    }

    public void Explode(Vector3 centerPosition, float centerForce, float radius)
    {
        UnlockRigidbody();
        rb.AddExplosionForce(centerForce, centerPosition, radius, .5f, ForceMode.Impulse);
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        StopCoroutine(nameof(Recover));
        StartCoroutine(nameof(Recover));
    }

    private IEnumerator Recover()
    {
        do
        {
            yield return null;
        } while (!rb.IsSleeping());

        yield return new WaitForSeconds(0.25f);

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.rotation = Quaternion.identity;
        LockRigidbody();
    }

    private void UnlockRigidbody()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    private void LockRigidbody()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


}
