using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void restartLevel() {
        // get current level and restart it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // unpause the game
        Time.timeScale = 1f;
    }

    public void nextLevel() {
        // get the next level in the build index and load it
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else {
            Debug.Log("No more levels!");
        }

        // unpause the game
        Time.timeScale = 1f;
    }

    public void quitgame() {
        // quit game
        Application.Quit();
    }
}
