using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

public class AbilitySystem : MonoBehaviour
{
    [Header("References")]
    Rigidbody rb;
    public Transform playerCamera;
    private PlayerController pc;
    public LayerMask isWall;
    public Transform grappleOrigin;
    public LineRenderer lineRenderer;

    [Header("DoubleJump")]
    public bool hasDoubleJump = false;
    [SerializeField] private float jumpForce = 12;

    [Header("Dash")]
    public bool hasDash = false;
    [SerializeField] private float dashForce = 30f;
    [SerializeField] private float dashDuration = 0.25f;
    private Vector3 delayedForceToApply;

    [Header("Slide")]
    public bool hasSlide = false;
    [SerializeField] private float slideForce = 10f;
    [SerializeField] private float slideDuration = 1f;
    private float startYScale;
    public float crouchYScale;

    [Header("Grapple")]
    public bool hasGrapple = false;
    public bool isGrappling = false;
    public float grappleSpeed = 10f;
    public float stopDistance = 1.5f;
    private Vector3 grapplePoint;


    [Header("Keybinds")]
    [SerializeField] private KeyCode abilityKey = KeyCode.Mouse1;

    public currentAbility cAbility;

    public enum currentAbility {
        None,
        Double,
        Dash,
        Slide,
        Grapple
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();

        pc = GetComponent<PlayerController>();

        startYScale = transform.localScale.y;
    }

    private void FixedUpdate() {
        AbilityHandler();

        if (isGrappling) {
            StartGrapple();
            DrawGrappleLine();
        }

        if (isGrappling && Input.GetKey(KeyCode.Space)) {
            StopGrapple();

        }
    }

    private void AbilityHandler() {

        switch (cAbility)
        {
            case currentAbility.Double:
                if (hasDoubleJump && Input.GetKey(abilityKey))
                {
                    dJump();
                }
                break;

            case currentAbility.Dash:
                if (hasDash && Input.GetKey(abilityKey))
                {
                    Dash();
                }
                break;

            case currentAbility.Slide:
                if (hasSlide && Input.GetKey(abilityKey) && pc.bIsGrounded)
                {
                    Slide();
                }
                break;

            case currentAbility.Grapple:
                if (hasGrapple && Input.GetKey(abilityKey))
                {
                    Grapple();
                }
                break;

            case currentAbility.None:
                break;
        }
    }

    // DOUBLE JUMP
    private void dJump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        hasDoubleJump = false;
        cAbility = currentAbility.None;
    }


    // DASHING
    private void Dash() {
        pc.dashing = true;

        Vector3 dashDirection = playerCamera.forward;
        dashDirection.Normalize();

        Vector3 forceToApply = dashDirection * dashForce;
        delayedForceToApply = forceToApply;

        rb.velocity = Vector3.zero;

        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(StopDash), dashDuration);

        hasDash = false;
        cAbility = currentAbility.None;


    }

    private void DelayedDashForce() {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void StopDash() {
        pc.dashing = false;
    }


    // SLIDING
    private void Slide() {

        pc.sliding = true;

        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        hasSlide = false;
        cAbility = currentAbility.None;

        Invoke(nameof(DelayedSlideForce), 0.1f);

        Invoke(nameof(StopSliding), slideDuration);
    }

    private void DelayedSlideForce() {
        rb.AddForce(playerCamera.forward * slideForce, ForceMode.VelocityChange);
    }

    private void StopSliding() {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        pc.movementSpeed = pc.walkSpeed;
        pc.sliding = false;
        cAbility = currentAbility.None;
    }

    // GRAPPLING
    private void Grapple() {

        RaycastHit hit;

        Debug.DrawRay(playerCamera.position, playerCamera.forward * 50, Color.red, 5);

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 20)) {
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy") {
                Debug.Log("Grapple Has hit");

                grapplePoint = hit.point;
                isGrappling = true;

                hasGrapple = false;
                cAbility = currentAbility.None;
            }
        }
    }

    private void StartGrapple() {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance > stopDistance) {
            rb.velocity = direction * grappleSpeed;
        }
        else {
            StopGrapple();
        }
    }

    void StopGrapple() {
        isGrappling = false;
        dJump();

        if (lineRenderer) {
            lineRenderer.enabled = false;
        }
    }

    void DrawGrappleLine() {
        if (lineRenderer && isGrappling) {
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }
}
