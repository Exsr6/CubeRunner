using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject _MainMenuCanvas;
    public GameObject _SettingsCanvas;
    public GameObject _LevelSelectCanvas;

    public void PlayButton() {
        _MainMenuCanvas.SetActive(false);
        _LevelSelectCanvas.SetActive(true);
    }

    public void SettingsButton() {
        _MainMenuCanvas.SetActive(false);
        _SettingsCanvas.SetActive(true);
    }

    public void BackButton(GameObject CurrentCanvas)
    {
        CurrentCanvas.SetActive(false);
        _MainMenuCanvas.SetActive(true);
    }
    public void LevelSelect(string levelName) {
        SceneManager.LoadScene(levelName);
    }

    public void QuitButton() {
        // Quit the game
        Application.Quit();
        Debug.Log("Game is exiting");
    }

}
