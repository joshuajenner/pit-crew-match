using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public AudioSource swipe;
    public AudioSource swapBack;
    public AudioSource match;


    void Start()
    {
        SettingsManager.volumeChanged.AddListener(UpdateVolume);
        UpdateVolume();

        Board.tilesSwiped.AddListener(PlaySwipe);
        Board.tilesSwipedBack.AddListener(PlaySwipeBack);
        Board.tilesMatched.AddListener(PlayMatch);
    }


    public void UpdateVolume()
    {
        swipe.volume = SettingsManager.GetChannelVolume(SettingsManager.AudioChannel.SFX);
        swapBack.volume = SettingsManager.GetChannelVolume(SettingsManager.AudioChannel.SFX);
        match.volume = SettingsManager.GetChannelVolume(SettingsManager.AudioChannel.SFX);
    }

    private void PlaySwipe()
    {
        swipe.Play();
    }

    private void PlaySwipeBack()
    {
        swapBack.Play();
    }

    private void PlayMatch()
    {
        match.Play();
    }
}
