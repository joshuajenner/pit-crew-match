using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    [SerializeField]
    public SettingsManager.AudioChannel channel;


    void Start()
    {
        slider = GetComponent<Slider>();
        GetSavedVolume();
        slider.onValueChanged.AddListener(delegate { HandleVolumeChanged(); });
    }

    private void GetSavedVolume()
    {
        slider.value = SettingsManager.GetChannelVolume(channel);
    }

    private void HandleVolumeChanged()
    {
        SettingsManager.UpdateVolume(channel, slider.value);
    }
}
