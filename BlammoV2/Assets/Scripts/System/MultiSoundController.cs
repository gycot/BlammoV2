using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class MultiSoundController : MonoBehaviour
{
    public AudioSource FirstAudio;
    public int Qty;
    int CurAudio = 0;
    AudioSource[] Sources;

    private void Awake()
    {
        Transform parent = FirstAudio.transform.parent;
        Sources = new AudioSource[Qty];
        Sources[0] = FirstAudio;
        for (int x=1; x<Qty; x++)
        {
            Sources[x] = GameObject.Instantiate<AudioSource>(FirstAudio, parent);
            Sources[x].transform.position = FirstAudio.transform.position;
        }

    }
    private void Start()
    {
        //purge
        for (int i = 0; i < Sources.Length; i++)
        {
            Sources[i].PlayOneShot(AudioLibrary.Instance.SilentSample, 0.01f);
        }


    }

    AudioSource GetNext()
    {
        CurAudio = (CurAudio + 1) % Qty;
        return Sources[CurAudio];

    }

    public void PlayLocalSFX(GenericSoundBit sound)
    {
        if (sound != null)
        {
            DirectPlaySFX(GetNext(),sound.GetRando(), sound.SoundVolume);
        }
    }

    private void DirectPlaySFX(AudioSource audioSource, AudioClip audioClip, float soundVolume)
    {
        audioSource.pitch = UnityEngine.Random.Range(.9f, 1.1f);
        //audioSource.clip = audioClip;
        //audioSource.volume = soundVolume;
        //audioSource.Play();
        audioSource.PlayOneShot(audioClip, soundVolume);
    }

    public void PlayLocalOneShot(AudioClip sound, float vol)
    {
        if (sound != null)
        {
            DirectPlaySFX(GetNext(), sound, vol);
        }
    }


    public void PlayOneShot(Vector3 pos, AudioClip sound, float vol)
    {
        if (sound == null) return;
        StartCoroutine(DelayPlay(pos, sound, vol));
    }


    private IEnumerator DelayPlay(Vector3 pos, AudioClip audio, float volume)
    {
        AudioSource src = GetNext();
        src.transform.position = pos;
        yield return null;
        DirectPlaySFX(src, audio, volume);
    }


}
