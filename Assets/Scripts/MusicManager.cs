using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip menu;
    public AudioClip game;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = SettingsManager.GetChannelVolume(SettingsManager.AudioChannel.Music);
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        source.clip = menu;
        source.Play();
    }

    public void PlayGameMusic()
    {
        source.clip = game;
    }
}
