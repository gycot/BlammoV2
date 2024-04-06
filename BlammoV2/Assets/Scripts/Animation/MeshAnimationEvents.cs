using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshAnimationEvents : MonoBehaviour {

    ActorAnimator Owner;
    public bool ManuallyCheckComplete = false;

    public Dictionary<CharacterState, Dictionary<PlayClipTime, SoundEventEffect>> SimpleStateSFX = new Dictionary<CharacterState, Dictionary<PlayClipTime, SoundEventEffect>>();

    [FormerlySerializedAs("SimpleSFXLoad")]
    public List<SoundEventEffect> SimpleFXLoad = new List<SoundEventEffect>();


    private void Awake()
    {
        Owner = GetComponentInParent<ActorAnimator>();
        if(Owner==null)
        {
            Debug.LogError("Missing Owner " + name);
        }
        //Owner.EventMan = this;

        for (int i = 0; i < SimpleFXLoad.Count; i++)
        {
            if(!SimpleStateSFX.ContainsKey(SimpleFXLoad[i].State))
            {
                SimpleStateSFX.Add(SimpleFXLoad[i].State, new Dictionary<PlayClipTime, SoundEventEffect>());
            }
            if(SimpleStateSFX[SimpleFXLoad[i].State].ContainsKey(SimpleFXLoad[i].Time))
            {
                Debug.LogError("Dupe SFX Event in " + name + " " + Owner.name);
            }
            else
            {
                SimpleStateSFX[SimpleFXLoad[i].State].Add(SimpleFXLoad[i].Time, SimpleFXLoad[i]);
            }
        }
    }




    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	/*void Update () {
       
		if(ManuallyCheckComplete)
        {
            switch(Owner.SetAnimationState)
            {
                case CharacterState.StandToSit:
                case CharacterState.Attack:
                case CharacterState.SitRotate:
                case CharacterState.Place:
                case CharacterState.CantEvenFail:
                case CharacterState.TalkingCustom:
                    {
                        AnimatorStateInfo stat = Owner.SkeletonAnimation.GetCurrentAnimatorStateInfo(0);
                        if (stat.normalizedTime > 0)
                        {
                            Owner.CompleteEvent(null);
                        }
                        break;
                    }

            }
        }
	}*/



    void Step()
    {
        Owner.StepEvent();
    }
    void Action()
    {
        //Owner.ActionEvent();
        PlaySimpleEventSFX(PlayClipTime.Action);
    }
    void Sfx()
    {
        //Owner.SfxEvent();
    }
    void Swing()
    {
        //Owner.SwingEvent();
        PlaySimpleEventSFX(PlayClipTime.Swing);
    }
    void Complete()
    {
        Owner.CompleteEvent();
    }
    void Effects()
    {
        //Owner.EffectsEvent();
    }
    void SfxLookup(string eventN)
    {
        //Debug.Log("SfxLookup " + eventN);
        GenericSoundBit bit = AudioLibrary.Instance.Lookup(eventN);
        if(bit==null)
        {
            Debug.LogError("SfxLookup Failure " + eventN);
        }
        //Owner.Owner.PlaySFX(bit);
    }
    void Shake()
    {
        //CameraManager.Instance.AddFullGenericShake(ShakeTypes.Tiny);
    }



    public ParticleEventEffect[] AddlParticleEffects;


    void PlaySimpleEventSFX(PlayClipTime clip)
    {
        /*CharacterState state = Owner.SetAnimationState;
        if(SimpleStateSFX.ContainsKey(state) && SimpleStateSFX[state].ContainsKey(clip))
        {
            SoundEventEffect ev = SimpleStateSFX[state][clip];
            Owner.Owner.PlaySFX(ev.SFX);
            if (ev.Particle != null) ev.Particle.Emit(Owner.Owner.position, Vector3.one);
            if (ev.OwnedParticle != null) ev.OwnedParticle.Emit(Owner.Owner.position);
            
        }*/


    }


}


[System.Serializable]
public class ParticleEventEffect
{
    public CharacterState State;
    public FocusedParticleManager Particle;
    public Transform ParticleSource;
    public GenericSoundBit Sfx;

}

[System.Serializable]
public class SoundEventEffect
{
    public CharacterState State;
    public PlayClipTime Time;
    public GenericSoundBit SFX;
    public FocusedParticleManager Particle;
    public MultiParticleEmitter OwnedParticle;
}




[System.Serializable]
public class GenericSoundBit
{

    public float SoundVolume;
    public AudioClip[] Sounds;
    public bool HasSounds
    {
        get
        {
            return Sounds.Length > 0;
        }
    }
    public AudioClip GetRando()
    {
        if (Sounds == null || Sounds.Length == 0)
        {
            return null;
        }
        return Sounds[UnityEngine.Random.Range(0, Sounds.Length)];
    }

    public void Play(AudioSource source)
    {
        Play(source, 1);
    }
    public void Play(AudioSource source, float volFactor)
    {
        if (HasSounds)
        {
            source.pitch = UnityEngine.Random.Range(.9f, 1.1f);
            source.PlayOneShot(GetRando(), SoundVolume * volFactor);
        }
    }
    public void Play(Vector3 pos)
    {
        Play(pos, 1);
    }
    public void Play(Vector3 pos, float volFactor)
    {
        if (HasSounds)
        {
            AudioLibrary.Instance.PlaySFX(GetRando(), SoundVolume * volFactor, pos);
        }
    }
    public void Play2D()
    {

        if (HasSounds)
        {
            AudioLibrary.Instance.PlaySFX2D(GetRando(), SoundVolume);
        }
    }

}

[System.Serializable]
public class AnimationCharacterSoundState : GenericSoundBit
{
    public PlayClipTime PlayClipTime = PlayClipTime.Start;

    public AnimationCharacterSoundState(string anim, bool loop)
    {
        Sounds = new AudioClip[0];
    }



}


public enum PlayClipTime
{
    Start,
    Action,
    NotSpecified,
    Spawn,
    SfxTrigger,
    Swing,
    Particle,
    Hit,
    Miss,
    Zoom,
    HitGround

}
