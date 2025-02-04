using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;

    public Transform orientation;
    Rigidbody rb;

    float MovementX;
    float MovementY;

    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        MovementX = Input.GetAxisRaw("Horizontal");
        MovementY = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * MovementY + orientation.right * MovementX;

        rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
    }
}
