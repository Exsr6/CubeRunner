using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Zones : MonoBehaviour
{
    [Header("References")]
    private Timer timer;
    private EndGoal goal;
    private PlayerController playerController;

    void Start()
    {
        timer = GameObject.Find("TimerManager").GetComponent<Timer>();
        goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "speedzone")
        {
            playerController.movementSpeed = 12;
        }

        if (other.gameObject.tag == "endzone" && goal.killsNeeded <= 0)
        {
            Debug.Log("You have reached the end!");
            FindObjectOfType<Timer>().CompleteLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "speedzone")
        {
            playerController.movementSpeed = 7;
        }
    }
}
