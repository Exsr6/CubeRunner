using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public GameObject restartUI; // Assign the Restart UI panel in Inspector

    private void Start() {
        restartUI.SetActive(false); // Hide restart UI on start
    }

    public void Die() {
        restartUI.SetActive(true); // Show the restart UI
        Time.timeScale = 0f; // Pause the game
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void RestartGame() {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "killzone") // If player collides with an enemy
        {       
            Die();
        }
    }
}
