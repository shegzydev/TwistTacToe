using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] sounds;
    Dictionary<string, Sound> soundMap = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
   
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;

            soundMap[s.name] = s;

            if (s.playOnAwake)
            {
                Play(s.name);
            }
        }
    }

    public void Play(string soundName, float pitch = 1f, float volume = 1f)
    {
        if (soundMap.TryGetValue(soundName, out Sound sound))
        {
            pitch = Random.Range(pitch - 0.01f, pitch + 0.01f);
            sound.source.volume = volume;
            sound.source.pitch = pitch;
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }

    public void Stop(string soundName)
    {
        if (soundMap.TryGetValue(soundName, out Sound sound))
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }
}