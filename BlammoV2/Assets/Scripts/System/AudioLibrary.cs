using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour {

    public static AudioLibrary Instance;

    public AudioClip SilentSample;
    
    float GlobalVol = 1;
    public float MaxAmbientVol = 1;



    public float GenericAudioSFxVolume = .5f;    
    public AudioClip Pickup;
    public AudioClip Splash,SplashOut;
    public AudioClip Russle;
    public AudioClip Timber;
    public AudioClip SetDown;
    public AudioClip Growl;


    public GenericSoundBit MiniVictory;
    public GenericSoundBit LandedSfx;

    float[] GenericAudioSFxLastFired;


    public GenericUISfx GenericUI;


    public AudioSource AmbientSfxA;
    public AudioSource AmbientSfxB;
    public bool PrimaryAmbientA = true;

    public AudioLookup[] Lookups;
    Dictionary<string, GenericSoundBit> LookupDict = new Dictionary<string, GenericSoundBit>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);


        GenericAudioSFxLastFired = new float[(int)GenericAudioSFx.Max];
        for (int i = 0; i < GenericAudioSFxLastFired.Length; i++)
        {
            GenericAudioSFxLastFired[i] = -1;
        }
        for (int i=0; i<Lookups.Length; i++)
        {
            LookupDict.Add(Lookups[i].Name, Lookups[i].Sound);
        }
    }

    // Use this for initialization
    void Start () {
    }

    /*
    public void PlayAmbient(BiomeDefinition bio, TimeController time)
    {
        if(bio == null)
        {
            return;
        }
        //figure out the clip we want
        AudioClip clip;
        if(time.CurrentState == TimeState.Dawn || time.CurrentState == TimeState.Day)
        {
            clip = bio.DayAudio;
        }
        else
        {
            if(bio.NightAudio == null)
            {
                clip = bio.DayAudio;
            }
            else
            {
                clip = bio.NightAudio;
            }
        }
        SetAmbient(clip);
    }
    */

    public void Play(GenericAudioSFx fx, Vector3 pos)
    {
        Play(fx, pos, 1);
    }
    public void Play(GenericAudioSFx fx, Vector3 pos, float volFactor)
    {
        if(Time.time - .1f < GenericAudioSFxLastFired[(int)fx])
        {
            return;
        }
        float vol = GenericAudioSFxVolume * volFactor;
        GenericAudioSFxLastFired[(int)fx] = Time.time;
        PlaySFX(GetClip(fx), vol, pos);
    }

    AudioClip GetClip(GenericAudioSFx fx)
    {
        switch (fx)
        {
            case GenericAudioSFx.None:
                {
                    return null;
                }
            case GenericAudioSFx.Pickup:
                {
                    return Pickup;
                }
            case GenericAudioSFx.Russle:
                {
                    return Russle;
                }
            case GenericAudioSFx.SplashIn:
                {
                    return Splash;
                }
            case GenericAudioSFx.SplashOut:
                {
                    return SplashOut;
                }
            case GenericAudioSFx.SetDown:
                {
                    return SetDown;
                }
            case GenericAudioSFx.Growl:
                {
                    return Growl;
                }
        }
        return null;
    }




    public MultiSoundController WorldAudioSource;
    public MultiSoundController WorldAudioSource2D;
    public float CameraAudioPlayOffset = 3;
    //public CameraManager CamManager;
    public float MaxSplitCamSoundDistance = 1;
    public AnimationCurve SplitCamAudioCurve = new AnimationCurve(new Keyframe(1f, 0f), new Keyframe(0f, 1f));


    public float GetVolumeFactor(Vector3 pos)
    {
        float distance=0;
        //distance = Vector2.Distance(pos, CameraManager.Instance.Cam1.thisTransform.position);
        if (distance < MaxSplitCamSoundDistance)
        {
            return SplitCamAudioCurve.Evaluate(distance / MaxSplitCamSoundDistance);

        }
        else
        {
            return 0;
        }
    }

    public float OneShotMinimumVolume = .1f;
    public void PlaySFX(GenericSoundBit audio, Vector3 pos)
    {
        if (audio.HasSounds)
        {
            PlaySFX(audio.GetRando(), audio.SoundVolume, pos);
        }
    }
    public void PlaySFX(AudioClip audio, Vector3 pos)
    {
        PlaySFX(audio, GenericAudioSFxVolume, pos);
    }
    public void PlaySFX(AudioClip audio, float volume, Vector3 pos)
    {
        WorldAudioSource.PlayOneShot(pos, audio, volume);
       
    }




    GenericSoundBit GetUISFX(GenericUIAudioSFx sfx)
    {
        switch(sfx)
        {
            case GenericUIAudioSFx.Submit:
                {
                    //Debug.Log("Submit SFX");
                    return GenericUI.Submit;
                }
            case GenericUIAudioSFx.ToggleA:
                {
                    return GenericUI.ToggleA;
                }
            case GenericUIAudioSFx.ToggleB:
                {
                    return GenericUI.ToggleB;
                }
            case GenericUIAudioSFx.ToggleC:
                {
                    return GenericUI.ToggleC;
                }
            case GenericUIAudioSFx.SlideIn:
                {
                    return GenericUI.SlideIn;
                }
            case GenericUIAudioSFx.SlideOut:
                {
                    return GenericUI.SlideOut;
                }
            case GenericUIAudioSFx.Stamp:
                {
                    return GenericUI.Stamp;
                }
        }
        return null;
    }
    public void PlaySFX2D(GenericUIAudioSFx audio)
    {
        PlaySFX2D(GetUISFX(audio));
    }
    public void PlaySFX2D(GenericAudioSFx fx, float vol)
    {
        PlaySFX2D(GetClip(fx),vol);
    }


    public void PlaySFX2D(GenericSoundBit audio)
    {
        if (audio!=null && audio.HasSounds)
        {
            PlaySFX2D(audio.GetRando(), audio.SoundVolume);
        }
    }
    public void PlaySFX2D(AudioClip audio)
    {
        PlaySFX2D(audio, GenericAudioSFxVolume);
    }
    public void PlaySFX2D(AudioClip audio, float volume)
    {
        if (audio != null)
        {
            //Debug.Log("Play2d "+audio);
            WorldAudioSource2D.PlayLocalOneShot(audio, volume);
        }
    }



    bool AmbientLocked = false;
    float CurAudioBleedTime = 0;
    public float FullAudioBleedTime = 2;
    float PreviousActiveVolume = 0;

    float ActiveAmbientFactor = 1;
    public float ActiveAmbientFactorROC = 1;
    public float SpeakerAmbientFactor = .5f;

    public void UpdateAmbient(AudioClip aud,float roomAmbientFactor)
    {

        float desiredAmbientFactor = 1;
        /*TalkingSpeaker talker = TalkingManager.Instance.GetTalker();
        if (talker != null)
        {
            desiredAmbientFactor = SpeakerAmbientFactor;
        }*/

        if(desiredAmbientFactor < ActiveAmbientFactor)
        {
            ActiveAmbientFactor -= ActiveAmbientFactorROC * Time.deltaTime;
            if(ActiveAmbientFactor < desiredAmbientFactor)
            {
                ActiveAmbientFactor = desiredAmbientFactor;
            }
        }
        else if(desiredAmbientFactor > ActiveAmbientFactor)
        {
            ActiveAmbientFactor += ActiveAmbientFactorROC * Time.deltaTime;
            if (ActiveAmbientFactor > desiredAmbientFactor)
            {
                ActiveAmbientFactor = desiredAmbientFactor;
            }
        }



        AudioSource foc, alt;
        if (PrimaryAmbientA)
        {
            foc = AmbientSfxA;
            alt = AmbientSfxB;
        }
        else
        {
            foc = AmbientSfxB;
            alt = AmbientSfxA;
        }
        
        bool transitioning = CurAudioBleedTime < FullAudioBleedTime;
        if (foc.clip!= aud)
        {
            if(alt.clip == aud && transitioning)
            {
                CurAudioBleedTime = FullAudioBleedTime - CurAudioBleedTime;
            }
            else
            {
                //stop the alt
                if (alt.isPlaying)
                {
                    alt.Stop();
                }
                alt.volume = 0;
                alt.clip = aud;
                alt.Play();
                PreviousActiveVolume = foc.volume;
                CurAudioBleedTime = 0;
            }
            PrimaryAmbientA = !PrimaryAmbientA;
            AmbientLocked = false;
        }
        else if (!AmbientLocked) 
        {
            CurAudioBleedTime += Time.deltaTime;
            if (CurAudioBleedTime > FullAudioBleedTime)
            {
                AmbientLocked = true;   //we need to lock it so we dont reset it if we get a fresh audbleed passed in, but the clip is the same
                if (alt.isPlaying)
                {
                    alt.Stop();
                }
                if (!foc.isPlaying)
                {
                    foc.Play();
                }
                foc.volume = MaxAmbientVol* roomAmbientFactor *ActiveAmbientFactor;
            }
            else
            {
                float bleed = CurAudioBleedTime / FullAudioBleedTime;
                foc.volume = (bleed * MaxAmbientVol* roomAmbientFactor * ActiveAmbientFactor);
                alt.volume = (1 - bleed) * PreviousActiveVolume * roomAmbientFactor * ActiveAmbientFactor; //old factor already has ambient in it!
                if (!alt.isPlaying)
                {
                    alt.Play();
                }
                if (!foc.isPlaying)
                {
                    foc.Play();
                }
            }
        }
        else
        {
            foc.volume = MaxAmbientVol * roomAmbientFactor * ActiveAmbientFactor;
        }

    }


    public GenericSoundBit Lookup(string name)
    {
        if (LookupDict.ContainsKey(name))
        {
            return LookupDict[name];
        }
        return null;
    }


}

[System.Serializable]
public class GenericUISfx
{    
    public GenericSoundBit ToggleA;
    public GenericSoundBit ToggleB;
    public GenericSoundBit ToggleC;
    public GenericSoundBit Submit;
    public GenericSoundBit SlideIn;
    public GenericSoundBit SlideOut;
    public GenericSoundBit Stamp;
}


[System.Serializable]
public class AudioLookup
{
    public string Name;
    public GenericSoundBit Sound;
}


public enum GenericUIAudioSFx
{
    None = 0,
    ToggleA=1,
    ToggleB = 2,
    ToggleC = 3,
    Submit =4,
    SlideIn =5,
    SlideOut = 6,
    Stamp =7,
    Max = 8
}


public enum GenericAudioSFx
{
    None=0,
    Pickup=1,
    SplashIn=2,
    Russle=3,
    SetDown=4,
    SplashOut = 5,
    Growl=6,
    Max =7
}

