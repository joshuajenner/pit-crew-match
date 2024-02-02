using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    private const string masterVolume = "master_vol";
    private const string musicVolume = "music_vol";
    private const string sfxVolumne = "sfv_vol";

    public enum AudioChannel
    {
        Master,
        Music,
        SFX
    }

    public static void UpdateVolume(AudioChannel channel, float newValue)
    {
        PlayerPrefs.SetFloat(GetChannelKey(channel), newValue);
        PlayerPrefs.Save();
    }

    public static float GetChannelVolume(AudioChannel channel)
    {
        return PlayerPrefs.GetFloat(GetChannelKey(channel));
    }

    private static string GetChannelKey(AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                return masterVolume;
            case AudioChannel.Music:
                return musicVolume;
            case AudioChannel.SFX:
                return sfxVolumne;
        }
        return "";
    }
}
