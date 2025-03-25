using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Zones : MonoBehaviour
{
    [Header("References")]
    private EndGoal goal;
    private PlayerController playerController;

    void Start()
    {
        // get and find components
        goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collision handler with zones
        if (other.gameObject.tag == "speedzone")
        {
            playerController.movementSpeed = 12;
        }

        if (other.gameObject.tag == "endzone" && goal.killsNeeded <= 0)
        {
            Debug.Log("You have reached the end!");
            Time.timeScale = 0f; // Pause the game
            FindObjectOfType<Timer>().CompleteLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Exit Collision handler with zones
        if (other.gameObject.tag == "speedzone")
        {
            playerController.movementSpeed = 7;
        }
    }
}
