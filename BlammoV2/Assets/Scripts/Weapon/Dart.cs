using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : SpawnedItem
{

    public Rigidbody RBody;
    int HitCount = 0;
    public bool ActiveDamage = true;

    protected override void Awake()
    {
        base.Awake();
    }



    public void Fire(Vector3 position, Quaternion rotation, float force)
    {
        thisTransform.position = position;
        thisTransform.rotation = rotation;
        RBody.velocity = RBody.angularVelocity = Vector3.zero;
        RBody.AddForce(thisTransform.forward * force);
        HitCount = 0;
        ActiveDamage = true;
    }


    protected override void Update()
    {
        base.Update();
        if(HitCount > 0 && Mathf.Abs(RBody.velocity.x) < FMath.Tiny  && Mathf.Abs(RBody.velocity.z) < FMath.Tiny)
        {
            ActiveDamage = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        HitCount++;
        if(HitCount > 3)
        {
            ActiveDamage = false;
        }
    }

}
