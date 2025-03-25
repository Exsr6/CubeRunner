using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public float walkSpeed = 7f;
    public float sprintSpeed = 17f;
    public float dashSpeed = 12f;
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public float groundDrag = 5f;

    [HideInInspector] public bool dashing = false;
    [HideInInspector] public bool sliding = false;

    [Header("Jumping")]
    public float jumpforce = 9f;
    private float jumpCooldown = 0.25f;
    bool readyToJump = true;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Slope Check")]
    private float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask isGround;
    [HideInInspector] public bool bIsGrounded;
    public LayerMask isWater;
    [HideInInspector] public bool bIsWater;

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

        speedParticles = FindObjectOfType<ParticleSystem>();
    }

    // Update is called once per frame
    void Update() {
        bIsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);
        bIsWater = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isWater);

        MovementX = Input.GetAxisRaw("Horizontal");
        MovementY = Input.GetAxisRaw("Vertical");

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

        if (Input.GetKey(jumpKey) && readyToJump && (bIsGrounded || bIsWater)) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void StateHandler() {

        // State - Dashing
        if (dashing) {
            state = MovementState.dashing;
            movementSpeed = dashSpeed;
        }

        // State - Sliding
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

    private void MovePlayer() {
        moveDirection = orientation.forward * MovementY + orientation.right * MovementX;
        Vector3 force = moveDirection.normalized * movementSpeed * 10f;

        if (OnSlope()) {
            force = GetSlopeMoveDirection() * movementSpeed * 20f;
            if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        rb.AddForce(force, ForceMode.Force);
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
            return Vector3.Angle(Vector3.up, slopeHit.normal) < maxSlopeAngle;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}
