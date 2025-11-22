using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> sfxClips = new List<AudioClip>();

    private Dictionary<string, AudioClip> musicDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadClips();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadClips()
    {
        foreach (var clip in musicClips)
            if (!musicDict.ContainsKey(clip.name))
                musicDict.Add(clip.name, clip);

        foreach (var clip in sfxClips)
            if (!sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
    }

    public void PlayMusic(string clipName, bool loop = true)
    {
        if (musicDict.TryGetValue(clipName, out AudioClip clip))
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{clipName}' not found!");
        }
    }


    public void PlaySFX(string clipName, float volume = 1f, float pitch = 1f)
    {
        if (sfxDict.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource tempSource = Instantiate(sfxSourcePrefab, transform);
            tempSource.clip = clip;
            tempSource.volume = Mathf.Clamp01(volume);
            tempSource.pitch = pitch;
            tempSource.Play();
            Destroy(tempSource.gameObject, clip.length / pitch);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{clipName}' not found!");
        }
    }
    public void OnPortalClicked_PlaySFX()
    {
        PlaySFX("Portal", 0.3f);
    }
    public void OnUIClicked_PlaySFX()
    {
        PlaySFX("Click_UI", 0.3f);
    }

}
