using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    private const string masterVolume = "master_vol";
    private const string musicVolume = "music_vol";
    private const string sfxVolumne = "sfv_vol";

    private const float defaultVolume = 0.5f;

    public enum AudioChannel
    {
        Master,
        Music,
        SFX
    }

    public static void UpdateSetting(AudioChannel channel, float newValue)
    {
        PlayerPrefs.SetFloat(GetChannelKey(channel), Mathf.Clamp(newValue,0, 1));
        PlayerPrefs.Save();
    }

    public static float GetChannelVolume(AudioChannel channel)
    {
        float volume = PlayerPrefs.GetFloat(GetChannelKey(channel), defaultVolume);

        if (channel == AudioChannel.Master)
        {
            return volume;
        }
        else
        {
            return volume * PlayerPrefs.GetFloat(GetChannelKey(AudioChannel.Master), defaultVolume);
        }
    }

    public static float GetChannelSetting(AudioChannel channel)
    {
        return PlayerPrefs.GetFloat(GetChannelKey(channel), defaultVolume);
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
