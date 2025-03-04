using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour {

    [Header("Variables")]
    float MovementX;
    float MovementY;
    Vector3 moveDirection;

    [Header("References")]
    public Transform orientation;
    public Transform playerCamera;
    Rigidbody rb;
    private ParticleSystem speedParticles;

    [Header("Movement")]
    public float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float groundDrag;

    public bool dashing;
    public bool sliding;

    [Header("Jumping")]
    public float jumpforce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Slope Check")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask isGround;
    public bool bIsGrounded;
    public LayerMask isWater;
    public bool bIsWater;

    public MovementState state;
    public enum MovementState {
        walking,
        sprinting,
        sliding,
        dashing,
        air
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        speedParticles = GameObject.Find("SpeedParticles").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update() {
        bIsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);
        bIsWater = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isWater);

        PlayerInput();
        SpeedControl();
        StateHandler();

        if (state == MovementState.walking || state == MovementState.sprinting) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0;
        }
    }

    private void FixedUpdate() {
        MovePlayer();
        sParticles();
    }

    void PlayerInput() {
        MovementX = Input.GetAxisRaw("Horizontal");
        MovementY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && (bIsGrounded || bIsWater)) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler() {

        // State - Dashing
        if (dashing) {
            state = MovementState.dashing;
            movementSpeed = dashSpeed;
        }

        else if (sliding) {
            state = MovementState.sliding;
            movementSpeed = sprintSpeed;
        }
        
        // State - Walking
        else if (bIsGrounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }

        else if (bIsWater) {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }

        // State - Air
        else
        {
            state = MovementState.air;
        }
    }

    void MovePlayer() {
        moveDirection = orientation.forward * MovementY + orientation.right * MovementX;

        if (OnSlope()) {
            rb.AddForce(GetSlopeMoveDirection() * movementSpeed * 20f ,ForceMode.Force);

            if (rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (bIsGrounded) {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!bIsGrounded) {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }


    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed) {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private void sParticles() {
        if (movementSpeed > 7) {
            speedParticles.Play();
        }
        else {
            speedParticles.Stop();
        }
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.1f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}
