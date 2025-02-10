using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour {
    float MovementX;
    float MovementY;
    Vector3 moveDirection;

    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float dashSpeed;
    public float groundDrag;

    private float dashCooldown = 2f;
    private float dashTimer = 1f;
    private Vector3 dashDirection;
    bool isDashing = false;
    bool canDash = true;


    [Header("Jumping")]
    public float jumpforce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale;
    private float startYScale;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.Mouse1;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask isGround;
    bool bIsGrounded;

    [Header("References")]
    public Transform orientation;
    public Transform playerCamera;
    Rigidbody rb;

    public MovementState state;
    public enum MovementState {
        walking,
        sprinting,
        crouching,
        dashing,
        air
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update() {
        bIsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);

        PlayerInput();
        SpeedControl();
        StateHandler();

        if (bIsGrounded) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0;
        }
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    void PlayerInput() {
        MovementX = Input.GetAxisRaw("Horizontal");
        MovementY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && bIsGrounded) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            if (bIsGrounded) {
                rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
            }
        }

        if (Input.GetKeyUp(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler() {

        // State - Crouching
        if (Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;
        }

        // State - Dashing
        else if (Input.GetKey(dashKey) && canDash && !isDashing) {
            state = MovementState.dashing;

            StartCoroutine(Dash());
        }

        // State - Sprinting
        else if (bIsGrounded && Input.GetKey(sprintKey)) {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }

        // State - Walking
        else if (bIsGrounded) {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }

        // State - Air
        else {
            state = MovementState.air;
        }
    }

    void MovePlayer() {
        moveDirection = orientation.forward * MovementY + orientation.right * MovementX;

        if (bIsGrounded) {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!bIsGrounded) {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }


    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed) {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private IEnumerator Dash() {
        isDashing = true;
        canDash = false;
        if (isDashing) {

            
            dashDirection = playerCamera.forward;
            dashDirection.y = Mathf.Clamp(dashDirection.y, -0.5f, 0.5f);

            dashDirection = dashDirection.normalized;

            rb.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);
            
        }
        yield return new WaitForSeconds(dashTimer);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }
}
