using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SoundMixerManager : MonoBehaviour
{ 
[SerializeField] private AudioMixer audioMixer;
[SerializeField] private Slider sliderMaster;
[SerializeField] private Slider sliderSFX;
[SerializeField] private Slider sliderMusic;

const string MASTER_VOLUME = "SavedMasterVolume";

private void Start()
{
    if (PlayerPrefs.HasKey(MASTER_VOLUME))
    {
        LoadVolume();
        Debug.Log("Caricamento audio");
    }
    else
    {
        SetMusicVolume();
        SetSFXVolume();
        SetMasterVolume();
    }
}
public void SetMasterVolume()
{
    float level = sliderMaster.value;
    audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    PlayerPrefs.SetFloat("SavedMasterVolume", level);
}

public void SetSFXVolume()
{
    float level = sliderSFX.value;
    audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
    PlayerPrefs.SetFloat("SavedSFXVolume", level);
}

public void SetMusicVolume()
{
    float level = sliderMusic.value;
    audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    PlayerPrefs.SetFloat("SavedMusicVolume", level);
}

private void LoadVolume()
{
    sliderMaster.value = PlayerPrefs.GetFloat(MASTER_VOLUME);
    sliderSFX.value = PlayerPrefs.GetFloat("SavedSFXVolume");
    sliderMusic.value = PlayerPrefs.GetFloat("SavedMusicVolume");

    SetMasterVolume();
    SetSFXVolume();
    SetMusicVolume();
}

}
