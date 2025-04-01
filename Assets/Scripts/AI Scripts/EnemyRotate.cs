using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotate : MonoBehaviour
{
    [Header("References")]
    private CameraController _cc;

    [Header("Variables")]
    public float fDetectionRange = 1000f;
    public float fRotationSpeed = 10f;

    private void Start() {
        // find the object of cameracontroller
        _cc = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        // Find the distance value from the enemy position to the camera location
        float distanceToPlayer = Vector3.Distance(transform.position, _cc.transform.position);

        // Check distance against distance variable
        if (distanceToPlayer < fDetectionRange) {

            // get the direction
            Vector3 direction = (_cc.transform.position - transform.position).normalized;

            // gets the look rotation
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            // uses the look rotation and lerps the rotation using deltatime and rotation speed variable
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * fRotationSpeed);
        }
    }
}
