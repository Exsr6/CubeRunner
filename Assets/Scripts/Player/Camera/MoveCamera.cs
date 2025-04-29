using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        // Set Position of the camera to the position of the cameraPosition transform
        transform.position = cameraPosition.position;
    }
}
