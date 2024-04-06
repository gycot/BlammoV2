using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponBase
{
    public ParticleSystem EffectParticle;
    public Vector2Int EffectBulletsMM;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Fire()
    {
        base.Fire();
        EffectParticle.Emit(Random.Range(EffectBulletsMM.x, EffectBulletsMM.y));
    }
}
