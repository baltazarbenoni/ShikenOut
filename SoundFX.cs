using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class SoundFX : MonoBehaviour
{
    [SerializeField] AudioSource UIAudioSource;
    [SerializeField] AudioSource FXSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioClip eggPickSound;
    [SerializeField] AudioClip eggDropSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] List<AudioClip> chickenSounds = new ();
    [SerializeField] List<AudioClip> UISounds = new ();


    void Start()
    {
        UIAudioSource.loop = false;
        FXSource.loop = false;
    }
    void OnEnable()
    {
        Actions.ChangeAudio += ChangeAudioClip;
    }
    void OnDisable()
    {
        Actions.ChangeAudio -= ChangeAudioClip;
    }
    //check which of the two audioSources to use.
    //if this source is still playing, check again in the next frame. but no more than 30 times.
    //then get the correct clip, and play it.
    void ChangeAudioClip(int source, string clipName)
    {
        AudioSource sourceToUse = source > 0 ? UIAudioSource : FXSource;
        if(sourceToUse == null)
        {
            Debug.LogWarning("AudioSource " + source + "not set!");
            return;
        }
        //If source is not playing, play new clip. Else, if animationSound is about to play, override current clip.
        else if(!sourceToUse.isPlaying || clipName == "dash")
        {
            AudioClip clipToUse = GetClip(source);
            AssignNewClipToAudioSource(sourceToUse, clipToUse);
        }
    }
    void AssignNewClipToAudioSource(AudioSource source, AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }
    AudioClip GetClip(int clipDeterminant)
    {
        if(clipDeterminant == 0)
        {
            return GetRandomAudio(UISounds);
        }
        else 
        {
            return GetRandomAudio(chickenSounds);
        }
    }

    AudioClip GetRandomAudio(List<AudioClip> audioClips)
    {
        if(audioClips.Count == 0)
        {
            Debug.Log("Selected audioClipList is empty! " + audioClips);
            AudioClip defaultSound = buttonSound; 
            //return some default sound.
            return defaultSound;
        }
        else
        {
            int rndNum = UnityEngine.Random.Range(0, audioClips.Count - 1);
            return audioClips[rndNum];
        }
    }
}
