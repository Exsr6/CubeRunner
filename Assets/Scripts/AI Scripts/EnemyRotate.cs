using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotate : MonoBehaviour
{
    [Header("References")]
    private CameraController cc;

    [Header("Variables")]
    public float detectionRange = 10f;
    public float rotationSpeed = 5f;

    private void Start() {
        cc = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, cc.transform.position);

        if (distanceToPlayer < detectionRange) {
            Vector3 direction = (cc.transform.position - transform.position).normalized;
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
