using UnityEngine;

public class AudioScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public AudioClip click;
    [SerializeField] public AudioClip error;
    [SerializeField] public AudioClip info;
    [SerializeField] public AudioClip build;
    [SerializeField] public AudioClip demolish;
    [SerializeField] public AudioSource uiSounds;

    public void PlaySound(AudioClip clip)
    {
        if (PlayerPrefs.GetInt("Audio") == 1)
            uiSounds.PlayOneShot(clip);
    }

}
