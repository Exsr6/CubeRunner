using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotate : MonoBehaviour
{
    [Header("References")]
    private CameraController cc;

    [Header("Variables")]
    public float detectionRange = 1000f;
    public float rotationSpeed = 10f;

    private void Start() {
        // find the object of cameracontroller
        cc = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        // Find the distance value from the enemy position to the camera location
        float distanceToPlayer = Vector3.Distance(transform.position, cc.transform.position);

        // Check distance against distance variable
        if (distanceToPlayer < detectionRange) {

            // get the direction
            Vector3 direction = (cc.transform.position - transform.position).normalized;

            // gets the look rotation
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            // uses the look rotation and lerps the rotation using deltatime and rotation speed variable
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
