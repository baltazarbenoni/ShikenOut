using UnityEngine;
//C 2025 Daniel Snapir alias Baltazar Benoni
public class FXManager : MonoBehaviour
{
    [SerializeField] AudioSource UIAudioSource;
    [SerializeField] AudioSource FXSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioClip defaultChickenSound;
    [SerializeField] AudioClip eggPickSound;
    [SerializeField] AudioClip eggDropSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip hurtSound;

    void OnEnable()
    {
        Actions.ChangeAudio += ChangeAudio;
    }
    void OnDisable()
    {
        Actions.ChangeAudio -= ChangeAudio;
    }
    void Start()
    {
        
    }
    //check which of the two audioSources to use.
    //then get the correct clip, and play it.
    void ChangeAudio(int source, string clipName)
    {
        AudioSource sourceToUse = source > 0 ? UIAudioSource : FXSource;
        if(sourceToUse == null)
        {
            Debug.LogWarning("AudioSource " + source + "not set!");
            return;
        }
        else if(!sourceToUse.isPlaying)
        {
            AudioClip clip = GetAudioClip(clipName);
            AssignNewClipToAudioSource(sourceToUse, clip);
        }
    }
    AudioClip GetAudioClip(string clipName)
    {
        return clipName switch
        {
            "dash" => dashSound,
            "pickup" => eggPickSound,
            "drop" => eggDropSound,
            "hurt" => hurtSound,
            "button" => buttonSound,
            _ => defaultChickenSound
        };
    }
    void AssignNewClipToAudioSource(AudioSource source, AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
        Debug.Log("Clip : " + clip + " playing! in " + source);
    }
}
