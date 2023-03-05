using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [System.Serializable]
    struct Sound
    {
        public string name;
        public AudioClip clip;
    }
    [SerializeField] private Sound[] sounds;

    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        source = GetComponent<AudioSource>();
    }

    public static void PlaySound(string name)
    {
        foreach (Sound s in instance.sounds)
        {
            if (s.name == name)
            {
                instance.source.clip = s.clip;
                instance.source.Play();
            }
        }
    }
}
