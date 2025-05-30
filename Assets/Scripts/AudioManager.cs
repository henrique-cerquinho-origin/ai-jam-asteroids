using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    public SoundEffect[] soundEffects;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    private Dictionary<string, SoundEffect> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize sound dictionary
        soundDictionary = new Dictionary<string, SoundEffect>();
        foreach (var sound in soundEffects)
        {
            soundDictionary[sound.name] = sound;
        }

        // Set initial volumes
        UpdateVolumes();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlaySFX(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            sfxSource.pitch = sound.pitch;
            sfxSource.loop = sound.loop;
            
            if (sound.loop)
            {
                sfxSource.clip = sound.clip;
                sfxSource.volume = sound.volume * sfxVolume * masterVolume;
                sfxSource.Play();
            }
            else
            {
                sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume * masterVolume);
            }
        }
        else
        {
            Debug.LogWarning($"Sound effect {soundName} not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopAllSFX()
    {
        sfxSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        musicSource.volume = musicVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }
} 