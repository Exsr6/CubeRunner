using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Zones : MonoBehaviour
{
    [Header("References")]
    public GameObject _levelCompleteUI;

    private EndGoal _goal;
    private PlayerController _playerController;

    void Start()
    {
        // get and find components
        _goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
        _playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collision handler with zones
        if (other.gameObject.tag == "speedzone")
        {
            _playerController.movementSpeed = 12;
        }

        if (other.gameObject.tag == "endzone" && _goal.iKillsNeeded <= 0)
        {
            Debug.Log("You have reached the end!");
            Time.timeScale = 0f; // Pause the game
            FindObjectOfType<Timer>().CompleteLevel();
        }

        if (other.gameObject.tag == "sandboxEnd" && _goal.iKillsNeeded <= 0) {

            Time.timeScale = 0;

            // get and find the gameobjects
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            if (player != null) {
                // get components
                PlayerController movementScript = player.GetComponent<PlayerController>();
                WeaponSystem weaponScript = player.GetComponent<WeaponSystem>();
                CameraController cameraScript = camera.GetComponent<CameraController>();
                // make it so the player cant do anything when the level is done
                movementScript.enabled = false;
                weaponScript.enabled = false;
                cameraScript.enabled = false;
            }

            _levelCompleteUI.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Exit Collision handler with zones
        if (other.gameObject.tag == "speedzone")
        {
            _playerController.movementSpeed = 7;
        }
    }
}
