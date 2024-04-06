using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartGun : WeaponBase
{
    public Dart DartPrefab;
    public Transform GunNose;

    public int NumFired = 1;
    public float SwirlRadius = 0f;
    public float FireSpreadRadius = 10;

    public float Force; 
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }

    public float RapidFireTimeBetwixt = .25f;

    float LastFire = -1;

    public override void Fire()
    {
        base.Fire();
        for (int i = 0; i < NumFired; i++)
        {
            Vector3 basePos = GunNose.position;

            float xF = Random.Range(-1f, 1f);
            float yF = Random.Range(-1f, 1f);
            if (SwirlRadius > 0)
            {


                //Vector3 addl = new Vector3(0, Random.Range(0, FireSpawnRadius), 0);
                //addl = Quaternion.Euler(0, 0, Random.Range(0, 360)) * addl;
                Vector3 addl = Vector3.zero;
                addl.x = xF * SwirlRadius;
                addl.y = yF * SwirlRadius;
                addl = GunNose.rotation * addl;
                basePos += addl;
            }
            Quaternion rot = GunNose.rotation;
            if(FireSpreadRadius > 0)
            {
                rot = rot * Quaternion.Euler(xF * FireSpreadRadius, yF * FireSpreadRadius, 0);//, , 0);
            }



            Dart dart = (Dart)SpawnItemManager.Instance.GetItem(DartPrefab);
            dart.Fire(basePos, rot, Force);
        }
        LastFire = Time.time;
    }
    public override void HoldFire()
    {
        base.HoldFire();
        if (Time.time > LastFire + RapidFireTimeBetwixt)
        {
            Fire();
        }

    }

}
