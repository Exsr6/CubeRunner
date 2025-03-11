using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Death death;

    private void Start() {
        death = FindObjectOfType<Death>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 90) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            death.Die();
        }
    }
}
