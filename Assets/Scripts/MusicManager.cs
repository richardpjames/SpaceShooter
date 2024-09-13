using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private static AudioSource audioSource;
    [SerializeField] AudioClip backgroundMusic;

    private void Awake()
    {
        // Ensure that this is the only instance
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }
    }
    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        // If there is a clip passed in then change the song
        if(audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        // If reset then stop to replay
        if(resetSong )
        {
            audioSource.Stop();
        }
        // Play the background music
        audioSource.Play();
    }

    // For pausing our music (before replaying/resetting)
    public void PauseBackgroundMusic()
    {
        audioSource.Pause();
    }
}
