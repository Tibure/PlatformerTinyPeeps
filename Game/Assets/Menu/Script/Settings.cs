using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource sound;
    public Slider sliderVolume;
    public Toggle toggleMute;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        if (volume == sliderVolume.minValue)
        {
            toggleMute.isOn = true;
            Mute(true);
        }
        else
        {
            toggleMute.isOn = false;
        }
    }

    public void Mute(bool isMute)
    {
        if (isMute)
        {
            audioMixer.SetFloat("volume", -100);
        }
        else
        {
            SetVolume(sliderVolume.value);
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void MenuClick()
    {
        sound.PlayOneShot(sound.clip); 
    }
}
