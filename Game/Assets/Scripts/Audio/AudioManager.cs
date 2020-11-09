using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake() { instance = this; }
    public List<AudioClip> sfxLibrary;
    public AudioClip sfx_hurt, sfx_portalOpen, sfx_portalClose, sfx_EmeraldCollect;
    public GameObject soundObject;

    public void PlaySFX(string SFXName)
    {
        switch (SFXName)
        {
            case "hurt":
                SoundObjectCreation(sfx_hurt);
                break;
            case "portalOpen":
                SoundObjectCreation(sfx_portalOpen);
                break;
            case "portalClose":
                SoundObjectCreation(sfx_portalClose);
                break;
            case "emeraldCollect":
                SoundObjectCreation(sfx_EmeraldCollect);
                break;

        }
    }

    public void SoundObjectCreation(AudioClip clip)
    {
        GameObject newObject = Instantiate(soundObject, transform);
        newObject.GetComponent<AudioSource>().clip = clip;
        newObject.GetComponent<AudioSource>().Play();
    }
}
