using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            // Create an audio source for each sound in sounds array
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        Play("BGM");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);

        // If we found the sound, play it
        if (s == null) { Debug.LogWarning("Sound: " + name + " not found!"); }
        else { s.source.Play(); }
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);

        // If we found the sound, play it
        if (s == null) { Debug.LogWarning("Sound: " + name + " not found!"); }
        else { s.source.PlayOneShot(s.source.clip); }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);

        // If we found the sound, stop it
        if (s == null) { Debug.LogWarning("Sound: " + name + " not found!"); }
        else { s.source.Stop(); }
    }

    public void PlayPitch(string name, float pitch)
    {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);

        s.source.pitch = pitch;
        s.source.PlayOneShot(s.source.clip);
    }

    public IEnumerator PlayOneShotDelayed(string name, float delay)
    {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);

        yield return new WaitForSeconds(delay);

        // If we found the sound, play it
        if (s == null) { Debug.LogWarning("Sound: " + name + " not found!"); }
        else { s.source.Play(); }
    }


}
