using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioSource alarmAudioSource;
    [SerializeField] AudioSource defaultAudioSource;
    [SerializeField] AudioClip mainMusic;
    [SerializeField] AudioClip alarmMusic;
    [SerializeField] AudioClip transitionMusic;
    AudioClip[] clips = new AudioClip[3];
    [SerializeField] CurrentMusic currentMusic;
    [SerializeField] int foxCount = 3;
    [SerializeField] CurrentMusic changeEnCours;
    AudioSource currentSource;
    bool returningToMain;
    enum CurrentMusic
    {
        Default,
        Alarm,
        Transition,
        Null
    }
    void Awake()
    {
        InitAudioReferences();
        InitReferences();
    }
    void OnEnable()
    {
        float checkInterval = 10.665f / 2;
        //Actions.FoxStatusChange += CheckFoxStatus;
        Invoke("FetchFoxCount", 0.5f);
        InvokeRepeating("CheckFoxStatus", checkInterval, checkInterval);
    }
    void OnDisable()
    {
        //Actions.FoxStatusChange -= CheckFoxStatus;
    }
    void Start()
    {
        CheckFoxStatus();
        //StartCoroutine(FadeOutEnumerator());
    }
    void FixedUpdate()
    {
        if (!returningToMain)
        {
            ReturnToMainMusic();
        }
    }
    //Return to main theme in the end of transition and alarm clips.
    void ReturnToMainMusic()
    {
        returningToMain = true;
        float timeRemaining = alarmAudioSource.clip.length - alarmAudioSource.time;
        if (timeRemaining > 0.02f || changeEnCours != CurrentMusic.Null)
        {
            returningToMain = false;
            return;
        }
        else if (currentMusic == CurrentMusic.Transition)
        {
            ChangeMusicNoDelay((int)CurrentMusic.Default);
        }
        returningToMain = false;
    }
    void InitReferences()
    {
        clips[(int)CurrentMusic.Default] = mainMusic;
        clips[(int)CurrentMusic.Alarm] = alarmMusic;
        clips[(int)CurrentMusic.Transition] = transitionMusic;
    }
    void FetchFoxCount()
    {
        int foxPatrolling = FoxFieldOfView.FoxStatus[0];
        int foxChasing = FoxFieldOfView.FoxStatus[1];
        foxCount = foxPatrolling + foxChasing;
    }
    bool InitAudioSource(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            Debug.Log("Audio Source not set!!! " + audioSource.name);
            return false;
        }
        return true;
    }
    void InitAudioReferences()
    {
        bool audioSourcesFound = InitAudioSource(alarmAudioSource) && InitAudioSource(defaultAudioSource);
        if (!audioSourcesFound)
        {
            return;
        }
        else
        {
            Debug.Log("Assignin audio settings");
            defaultAudioSource.clip = mainMusic;
            defaultAudioSource.Play();
            defaultAudioSource.loop = true;
            alarmAudioSource.clip = alarmMusic;
            alarmAudioSource.loop = true;
            currentSource = defaultAudioSource;
        }
        changeEnCours = CurrentMusic.Null;
        currentMusic = CurrentMusic.Default;
    }
    void CheckFoxStatus()
    {
        //VerifyCurrentMusic();
        currentSource = GetCurrentSource();
        Debug.Log("CHECKING FOX STATUS");
        int foxPatrolling = FoxFieldOfView.FoxStatus[0];
        int foxChasing = FoxFieldOfView.FoxStatus[1];
        //If music recently changed, return.
        if (currentSource.time < 10f && currentMusic != CurrentMusic.Transition)
        {
            Debug.Log("Checking fox but not updating music!");
            Debug.Log("Source time is : " + currentSource.time);

            return;
        }
        else
        {
            SelectNewMusic(foxPatrolling, foxChasing);
        }
    }
    AudioSource GetCurrentSource()
    {
        return currentMusic switch
        {
            CurrentMusic.Default => defaultAudioSource,
            CurrentMusic.Alarm => alarmAudioSource,
            CurrentMusic.Transition => alarmAudioSource,
            _ => defaultAudioSource
        };
    }

    void SelectNewMusic(int foxPatrol, int foxChase)
    {
        Debug.Log("Current music is " + currentMusic);
        Debug.Log("FOX CHASE COUNT : " + foxChase);
        Debug.Log("FOX PATROL COUNT : " + foxPatrol);
        if (foxPatrol + foxChase > foxCount)
        {
            int foxAmount = foxPatrol + foxChase;
            Debug.LogWarning("Music manager, foxCount is off! fox count is " + foxAmount);
        }
        //If default is playing and some foxes are alarmed.
        if (currentMusic == CurrentMusic.Default && foxChase != 0)
        {
            Debug.Log("Change : default is playing and some foxes are alarmed.");
            ChangeMusic((int)CurrentMusic.Alarm);
        }
        //If alarm is playing and no foxes are alarmed.
        else if (currentMusic == CurrentMusic.Alarm && foxChase == 0)
        {
            Debug.Log("Change : alarm is playing and no foxes are alarmed.");
            ChangeMusic((int)CurrentMusic.Transition);
            bool allFoxesPatrolling = foxPatrol == foxCount;
            Debug.Log("All foxes patrolling : " + allFoxesPatrolling);
        }
        else
        {
            Debug.Log("Current music is " + currentMusic);
            Debug.Log("FOX CHASE COUNT : " + foxChase);
            Debug.Log("Fox status change but transition is playing");
        }
    }
    void ChangeMusic(int clipToPlayIndex)
    {
        Debug.Log("Changing music!");
        //If change is already happening.
        if (changeEnCours == (CurrentMusic)clipToPlayIndex)
        {
            Debug.Log("Changing music already, return");
            return;
        }
        //If no change is pending.
        else if (changeEnCours == CurrentMusic.Null)
        {
            changeEnCours = (CurrentMusic)clipToPlayIndex;
        }
        //If a different change is pending, override it.
        else
        {
            changeEnCours = (CurrentMusic)clipToPlayIndex;
            StopAllCoroutines();
        }
        float timeRemaining = currentSource.clip.length - currentSource.time;
        Debug.Log("Time remaining is " + timeRemaining);
        //Song is about to end, wait for it to end.
        if (timeRemaining < 3f)
        {
            StartCoroutine(ChangeMusicDelay(timeRemaining, clipToPlayIndex));
        }
        //Else change song immediately.
        else
        {
            ChangeMusicNoDelay(clipToPlayIndex);
        }
    }
    void ChangeMusicNoDelay(int clipIndex)
    {
        //Get new audio source and clip.
        AudioSource audioSource = GetNewSource();
        AudioClip clipToPlay = clips[clipIndex];
        //Fade out main theme.
        if (currentMusic == CurrentMusic.Default)
        {
            Debug.Log("Fading out!");
            StartCoroutine(FadeOutEnumerator());
        }
        //Else stop current song immediately.
        else if(audioSource != currentSource)
        {
            currentSource.Stop();
        }
        audioSource.Stop();
        audioSource.clip = clipToPlay;
        audioSource.Play();
        Debug.Log("Playing new clip!");
        currentSource = audioSource;
        changeEnCours = CurrentMusic.Null;
        currentMusic = (CurrentMusic)clipIndex;
    }
    AudioSource GetNewSource()
    {
        return currentMusic switch
        {
            CurrentMusic.Default => alarmAudioSource,
            CurrentMusic.Alarm => alarmAudioSource,
            CurrentMusic.Transition => defaultAudioSource,
            _ => defaultAudioSource
        };
    }
    IEnumerator ChangeMusicDelay(float delay, int clipIndex)
    {
        Debug.Log("Waiting to change music");
        yield return new WaitForSeconds(delay);
        ChangeMusicNoDelay(clipIndex);
    }
    IEnumerator FadeOutEnumerator()
    {
        for (float i = 1; i > -40; i -= 0.1f)
        {
            yield return new WaitForSeconds(0.02f);
            mainMixer.SetFloat("defaultMusicVolume", Mathf.Log(i) * 20);
        }
        Debug.Log("Got to the end of volume enumeration");
        defaultAudioSource.Stop();
        mainMixer.SetFloat("defaultMusicVolume", Mathf.Log(1f));
    }
    void VerifyCurrentMusic()
    {
        if (defaultAudioSource.isPlaying && currentMusic == CurrentMusic.Transition)
        {
            Debug.LogWarning("Both sources playing, applying correction");
            alarmAudioSource.Stop();
            currentMusic = CurrentMusic.Default;
            changeEnCours = CurrentMusic.Null;
            currentSource = GetCurrentSource();
        }
        else if (!defaultAudioSource.isPlaying && !alarmAudioSource.isPlaying)
        {
            Debug.LogWarning("No sources playing, applying correction");
            defaultAudioSource.Stop();
            defaultAudioSource.Play();
            currentMusic = CurrentMusic.Default;
            changeEnCours = CurrentMusic.Null;
        }

    }

}
