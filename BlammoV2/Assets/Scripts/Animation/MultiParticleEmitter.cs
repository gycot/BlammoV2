using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System;

public class MultiParticleEmitter : MonoBehaviour
{
    public Vector3 ReverseScale = new Vector3(-1, 1, 1);
    public Vector3 ForwardScale = Vector3.one;

    public bool TestFire = false;

    public Transform thisTransform;

    public AudioSource UISound;

    public bool DynamicScale = false;
    public Vector3 DynamicScaleSize1 = Vector3.one;


    public AudioClip EmitterSound;
    public float EmitterVol = 1;

    [FormerlySerializedAs("AllowRotate")]
    public bool AllowTransformRotate = true;
    public bool AllowEmitRotate = false;

    //GameObject[] ParticlesGO;
    public ParticleEmitSet[] Particles;
    ParticleSystem.ShapeModule[] ParticleShapes;
    void Awake()
    {
        thisTransform = transform;
        //ParticlesGO = new GameObject[Particles.Length];
        ParticleShapes = new ParticleSystem.ShapeModule[Particles.Length];
        for (int i=0; i<Particles.Length; i++)
        {
            ParticleShapes[i] = Particles[i].Particle.shape;
        }

    }


    ParticleSystem.EmitParams Params = new ParticleSystem.EmitParams();
    public bool StopStartEachEmit = true;



    void Update()
    {
        if (TestFire)
        {
            TestFire = false;
            Emit(thisTransform.position);
        }
    }

    bool First = true;
    void EmitParticle(Vector3 scale, Vector3 rotation)//, Vector3 rotation)
    {
        
        scale.x = Mathf.Abs(scale.x);
        scale.y = Mathf.Abs(scale.y);
        scale.z = Mathf.Abs(scale.z);

        if (EmitterSound != null)
        {
            AudioLibrary.Instance.PlaySFX(EmitterSound, EmitterVol, thisTransform.position);
        }
        if (UISound != null)
        {
            UISound.volume = EmitterVol;// * WorldSettings.Instance.SFXVolume;
            UISound.Play();
        }

        /*
        if(reverse)
        {
            thisTransform.localScale = ReverseScale;
        }
        else
        {
            thisTransform.localScale = ForwardScale;
        }*/

        ParticleEmitSet[] set;
        ParticleSystem.ShapeModule[] shapes;
        set = Particles;
        shapes = ParticleShapes;
        for (int i = 0; i < set.Length; i++)
        {

            if (set[i].Enabled)
            {
                set[i].Particle.enableEmission = false;
                if (First || StopStartEachEmit)
                {
                    //set[i].Particle.Simulate(1);
                    //set[i].Particle.Play();
                    set[i].Particle.Stop();
                    set[i].Particle.Clear();
                    set[i].Particle.Play();

                }



                if(AllowEmitRotate)
                {
                    Params.rotation = rotation.y;
                }
                if (DynamicScale)
                {
                    shapes[i].scale = Vector3.Scale(scale, DynamicScaleSize1);
                    //Debug.Log("EMIT SCALE " + shapes[i].scale +" "+name);
                    int newCount = (int)((scale.x * scale.y) * set[i].EmitCount);
                    set[i].Particle.Emit(Params,newCount);
                }
                else
                {
                    set[i].Particle.Emit(Params, (int)Particles[i].EmitCount);
                }
            }
        }
        First = false;
    }


    public void Emit(Vector3 position, Vector3 scale, Vector3 rotation)
    {

        if (thisTransform == null)
        {
            Awake();
        }
        thisTransform.position = position;
        if (AllowTransformRotate)
        {
            thisTransform.localRotation = Quaternion.Euler(rotation);
        }
        EmitParticle(scale, rotation);
    }


    public void EmitR(Vector3 position, Vector3 rotation)
    {
        Emit(position, Vector3.one, rotation);
    }
    public void EmitS(Vector3 position, Vector3 scale)
    {
        Emit(position, scale, Vector3.zero);
    }
    public void Emit(Vector3 position)
    {
        Emit(position, Vector3.one, Vector3.zero);
    }


    public void EditorBuildFromChildren()
    {
        ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
        Particles = new ParticleEmitSet[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            Particles[i] = new ParticleEmitSet();
            Particles[i].Particle = parts[i];
            Particles[i].EmitCount = (int)parts[i].emissionRate;

        }
    }
    public void EditorEnableEmission(bool enable)
    {
        for (int i = 0; i < Particles.Length; i++)
        {
            if (Particles[i].Enabled)
            {
                Particles[i].Particle.enableEmission = enable;
            }
        }
    }

}

[System.Serializable]
public class ParticleEmitSet {
    public float EmitCount;
    public bool Enabled = true;
    public ParticleSystem Particle;
}