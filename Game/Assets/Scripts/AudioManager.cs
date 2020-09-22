using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake() { instance = this; }
    public List<AudioClip> sfxLibrary;
    public AudioClip sfx_jump, sfx_hurt, sfx_running;
    public GameObject soundObject;

    public void PlaySFX(string SFXName)
    {
        switch (SFXName)
        {
            case "jump":
                SoundObjectCreation(sfx_jump);
                
                break;
            case "hurt":
                SoundObjectCreation(sfx_hurt);
                break;
            case "running":
                SoundObjectCreation(sfx_running);
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
