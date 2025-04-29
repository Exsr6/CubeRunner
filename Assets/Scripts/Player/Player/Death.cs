using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public GameObject restartUI;

    private PlayerController movementScript;
    private WeaponSystem weaponScript;
    private CameraController cameraScript;

    private void Start() {
        restartUI.SetActive(false); // Hide restart UI on start

        // get and find the gameobjects
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player != null) {
            // get components
            movementScript = player.GetComponent<PlayerController>();
            weaponScript = player.GetComponent<WeaponSystem>();
            cameraScript = camera.GetComponent<CameraController>();
        }
    }

    public void Die() {
        // Show the restart UI
        restartUI.SetActive(true);

        // make it so the player cant do anything when the level is done
        movementScript.enabled = false;
        weaponScript.enabled = false;
        cameraScript.enabled = false;

        // Pause the game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void RestartGame() {

        // reset values
        movementScript.enabled = true;
        weaponScript.enabled = true;
        cameraScript.enabled = true;
        // Resume the game
        Time.timeScale = 1f;
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter(Collision other) {
        // If player collides with an enemy
        if (other.transform.tag == "killzone")
        {
            // Call the Die function
            Die();
        }
    }
}
