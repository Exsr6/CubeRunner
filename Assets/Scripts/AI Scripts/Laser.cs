using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Death _death;

    private void Start() {
        _death = FindObjectOfType<Death>();
    }
    void Update()
    {
        // Rotate the laser around the Z axis
        transform.Rotate(new Vector3(0, 0, 90) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        // Check if the object that entered the trigger is the player
        if (other.gameObject.tag == "Player") {
            _death.Die();
        }
    }
}
