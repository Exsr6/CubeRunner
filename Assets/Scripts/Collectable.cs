using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string CollectableID;

    void Start()
    {
        // if player has already collected then set to false on start
        if (PlayerPrefs.GetInt(CollectableID, 0) == 1) {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {

        // collect collectable if player collides
        if (other.CompareTag("Player")) {
            CollectPresent();
        }
    }

    void CollectPresent() {

        // Set variable int and save it
        PlayerPrefs.SetInt(CollectableID, 1);
        PlayerPrefs.Save();

        // disable gameobject
        gameObject.SetActive(false);
    }
}