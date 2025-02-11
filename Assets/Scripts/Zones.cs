using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zones : MonoBehaviour
{
    private Timer timer;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.Find("TimerManager").GetComponent<Timer>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "speedzone")
        {
            playerController.movementSpeed = 12;
        }

        if (other.gameObject.tag == "endzone")
        {
            Debug.Log("You have reached the end!");
            timer.TimerRunning = false;
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
