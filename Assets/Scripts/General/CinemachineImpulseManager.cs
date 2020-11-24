using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CinemachineImpulseManager : MonoBehaviour
{
    public static CinemachineImpulseManager Instance;

    public ImpulseSource[] impulseSources;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
    }

    public static void Play(string name)
    {
        ImpulseSource s = Array.Find(Instance.impulseSources, source => source.name == name);

        // If we found the sound, play it
        if (s == null) { Debug.LogWarning("ImpulseSource: " + name + " not found!"); }
        else { s.cinemachineImpulseSource.GenerateImpulse(); }
    }

}
