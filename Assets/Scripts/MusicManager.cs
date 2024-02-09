using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip menu;
    public AudioClip game;

    public AudioSource source;

    private void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        SettingsManager.volumeChanged.AddListener(UpdateVolume);
        UpdateVolume();
        PlayMenuMusic();
    }

    public void UpdateVolume()
    {
        source.volume = SettingsManager.GetChannelVolume(SettingsManager.AudioChannel.Music);
    }

    public void PlayMenuMusic()
    {
        source.clip = menu;
        source.Play();
    }

    public void PlayGameMusic()
    {
        source.clip = game;
        source.Play();
    }

    public void Pause()
    {
        source.Pause();
    }

    public void Play()
    {
        source.UnPause();
    }
}
