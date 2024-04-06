
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FocusedParticleManager : MonoBehaviour
{
    



    public int NumberInstancesPerPart = 3;

    public Vector3 GlobalRotAddl = new Vector3(0, 90, 0);
    public MultiParticleEmitter ChildManagedTemplate;
    ManagedParticle PartMan;
    Transform thisTransform;

    private void Awake()
    {
        thisTransform = transform;
        PartMan = new ManagedParticle(ChildManagedTemplate, NumberInstancesPerPart);
    }
       
    public void Emit(Vector3 pos, Vector3 scale)
    {
        EmitFinal( pos,scale,Vector3.zero);
    }

    public void Emit(Vector3 pos, Vector3 scale, Vector3 rote)
    {
        EmitFinal( pos, scale, rote+ GlobalRotAddl);
    }
    void EmitFinal(Vector3 pos, Vector3 scale, Vector3 rot)
    {
        Transform parentT = thisTransform;

        PartMan.Emit(pos, scale, rot, parentT);     
    }


    public void ReturnAllTempOwnedParticles(Transform tempParent)
    {
        PartMan.ReturnAllTempOwnedParticles(tempParent, thisTransform);
    }

    public Vector3 DialogueWorldPosOffset = new Vector3(0, -1, 0);
    public void DialogableEmit(Vector3 pos, bool dialogueOn)
    {
        SetDialogueLayer(dialogueOn);
        if(!dialogueOn)
        {
            pos += DialogueWorldPosOffset;
        }
        Emit(pos, Vector3.one);
    }

    void SetDialogueLayer(bool dialogueOn)
    {
        string desiredLayer;
        if (dialogueOn)
        {
            desiredLayer = "CameraUI";
        }
        else
        {
            desiredLayer = "Default";
        }
        int layer = LayerMask.NameToLayer(desiredLayer);
        PartMan.SetParticleLayer(layer);
    }
}




[System.Serializable]
public class ManagedParticle
{
    public MultiParticleEmitter Template;
    public int NumberInstances = 3;
    [System.NonSerialized]
    public MultiParticleEmitter[] Emitter;
    [System.NonSerialized]
    public int ActiveIndex = 0;


    public ManagedParticle(MultiParticleEmitter template, int instances)
    {
        Template = template;
        NumberInstances = instances;


        if (NumberInstances < 1)
        {
            NumberInstances = 1;
        }
        Emitter = new MultiParticleEmitter[NumberInstances];
        Emitter[0] = Template;
        for (int i = 1; i < NumberInstances; i++)
        {
            Emitter[i] = GameObject.Instantiate<MultiParticleEmitter>(Template, Template.transform.parent);
        }
    }

    public void SetParticleLayer(int layer)
    {
        if (layer == Emitter[0].Particles[0].Particle.gameObject.layer)
        {
            return;
        }
        for (int i = 0; i < Emitter.Length; i++)
        {
            for (int j = 0; j < Emitter[i].Particles.Length; j++)
            {
                Emitter[i].Particles[j].Particle.gameObject.layer = layer;
            }
        }
    }


    public void Emit(Vector3 pos, Vector3 scale, Vector3 rot, Transform parentT)
    {
        ActiveIndex = (ActiveIndex + 1) % Emitter.Length;

        if (Emitter[ActiveIndex].thisTransform.parent != parentT)
        {
            Emitter[ActiveIndex].thisTransform.SetParent(parentT);
        }

        Emitter[ActiveIndex].Emit(pos, scale, rot);
    }
    public void Emit(Vector3 pos, Vector3 scale, Vector3 rot)
    {
        ActiveIndex = (ActiveIndex + 1) % Emitter.Length;
        Emitter[ActiveIndex].Emit(pos, scale, rot);

    }

    public void ReturnAllTempOwnedParticles(Transform tempParent, Transform trueParent)
    {
        for (int i = 0; i < Emitter.Length; i++)
        {
            if (Emitter[i].thisTransform.parent == tempParent)
            {
                Emitter[i].thisTransform.SetParent(trueParent);
            }

        }
    }
}

