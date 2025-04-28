using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _MainMenuCanvas;
    [SerializeField] private GameObject _SettingsCanvas;
    [SerializeField] private GameObject _LevelSelectCanvas;
    public AudioMixer audioMixer;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Variables")]
    private float MasterVolume;
    private float MusicVolume;
    private float SFXVolume;

    private void Start() {
        // Load saved volume settings
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Set the audio mixer volumes
        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSFXVolume(SFXVolume);

        // Set the sliders to the saved values
        masterSlider.value = MasterVolume;
        musicSlider.value = MusicVolume;
        sfxSlider.value = SFXVolume;
    }

    public void PlayButton() {
        // Make MainMenu inactive and LevelSelect active
        _MainMenuCanvas.SetActive(false);
        _LevelSelectCanvas.SetActive(true);
    }

    public void SettingsButton() {
        // Make MainMenu inactive and Settings active
        _MainMenuCanvas.SetActive(false);
        _SettingsCanvas.SetActive(true);
    }

    public void BackButton(GameObject CurrentCanvas)
    {
        // Make the current canvas inactive and MainMenu active
        CurrentCanvas.SetActive(false);
        _MainMenuCanvas.SetActive(true);
    }
    public void LevelSelect(string levelName) {
        // Load the selected level
        SceneManager.LoadScene(levelName);
    }

    public void QuitButton() {
        // Quit the game
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void SetMasterVolume(float volume) {
        // Set the master volume in the audio mixer
        MasterVolume = volume;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
        // Update the master volume text
        masterVolumeText.text = (volume * 100).ToString("0");
    }

    public void SetSFXVolume(float volume) {
        // Set the SFX volume in the audio mixer
        SFXVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(SFXVolume) * 20);
        // Update the SFX volume text
        sfxVolumeText.text = (volume * 100).ToString("0");
    }

    public void SetMusicVolume(float volume) {
        // Set the music volume in the audio mixer
        MusicVolume = volume;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20);
        // Update the music volume text
        musicVolumeText.text = (volume * 100).ToString("0");
    }

    public void VolumeApply() {
        // Save the volume settings to PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.Save();
    }

}
