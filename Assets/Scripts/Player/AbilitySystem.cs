using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AbilityPickups;
using static PlayerController;

public class AbilitySystem : MonoBehaviour
{
    [Header("References")]
    Rigidbody rb;
    public Transform playerCamera;
    private PlayerController pc;
    public LayerMask isWall;
    public Transform grappleOrigin;
    private LineRenderer lr;
    private AbilityUI abilityUI;

    [Header("DoubleJump")]
    [SerializeField] private float jumpForce = 11f;
    [HideInInspector] public bool hasDoubleJump = false;

    [Header("Dash")]
    [SerializeField] private float dashForce = 25f;
    [SerializeField] private float dashDuration = 0.25f;
    private Vector3 delayedForceToApply;
    [HideInInspector] public bool hasDash = false;

    [Header("Slide")]
    [SerializeField] private float slideForce = 10f;
    [SerializeField] private float slideDuration = 0.4f;
    [HideInInspector] public bool hasSlide = false;
    private float startYScale;
    private float crouchYScale = 0.5f;

    [Header("Grapple")]
    public float grappleSpeed = 20f;
    public float stopDistance = 1.5f;
    [HideInInspector] public bool hasGrapple = false;
    [HideInInspector] public bool isGrappling = false;
    private Vector3 grapplePoint;

    [Header("Ability Inventory")]
    public AbilityType[] abilityInventory = new AbilityType[2] { AbilityType.None, AbilityType.None };
    private int activeAbilityIndex = 0;
    private float swapCooldown = 0.5f;
    private float lastSwapTime = 0f;
    private float abilityCooldown = 0.2f;
    private float lastAbilityTime = 0f;


    [Header("Keybinds")]
    [SerializeField] private KeyCode abilityKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode swapAbilityKey = KeyCode.Q;

    public enum AbilityType {
        None,
        DoubleJump,
        Dash,
        Slide,
        Grapple
    }

    void Start() {
        rb = GetComponent<Rigidbody>();

        pc = GetComponent<PlayerController>();

        lr = GetComponent<LineRenderer>();

        abilityUI = FindObjectOfType<AbilityUI>();

        startYScale = transform.localScale.y;
    }

    private void FixedUpdate() {
        AbilityHandler();

        if (Input.GetKey(swapAbilityKey))
            SwapAbilities();

        if (isGrappling)
            StartGrapple(); DrawGrappleLine();

        if (isGrappling && Input.GetKey(KeyCode.Space))
            StopGrapple();

        Debug.Log(lastAbilityTime);

    }

    private void AbilityHandler() {
        if (Time.time - lastAbilityTime >= abilityCooldown) {
            if (Input.GetKey(abilityKey)) {
                useAbilities(abilityInventory[0]);
                lastAbilityTime = Time.time;
            }
        }

        abilityUI.UpdateUI();
    }

    public void pickupAbility(AbilityType newAbility) 
    {
        if (abilityInventory[0] == AbilityType.None) 
            abilityInventory[0] = newAbility;

        else if (abilityInventory[1] == AbilityType.None)
            abilityInventory[1] = newAbility;

        else
            abilityInventory[activeAbilityIndex] = newAbility;

        UnlockAbility(newAbility);
        abilityUI.UpdateUI();
    }
    private void UnlockAbility(AbilityType ability) {
        switch (ability) {
            case AbilityType.DoubleJump:
                hasDoubleJump = true; 
                break;
            case AbilityType.Dash: 
                hasDash = true; 
                break;
            case AbilityType.Slide: 
                hasSlide = true; 
                break;
            case AbilityType.Grapple:
                hasGrapple = true; 
                break;
        }
    }

    private void useAbilities(AbilityType ability) {
        if (!Input.GetKey(abilityKey)) return;

        switch (ability) {
            case AbilityType.DoubleJump:
                if (hasDoubleJump) {
                    dJump();
                    UpdateAbilitySlot(AbilityType.DoubleJump);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Dash:
                if (hasDash) {
                    Dash();
                    UpdateAbilitySlot(AbilityType.Dash);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Slide:
                if (hasSlide && (pc.bIsGrounded || pc.bIsWater)) {
                    Slide();
                    UpdateAbilitySlot(AbilityType.Slide);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Grapple:
                if (hasGrapple) {
                    Grapple();
                    UpdateAbilitySlot(AbilityType.Grapple);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;
        }

        abilityUI.UpdateUI();
    }

    private void SwapAbilities() {
        if (Time.time - lastSwapTime >= swapCooldown)
        {
            AbilityType temp = abilityInventory[0];
            abilityInventory[0] = abilityInventory[1];
            abilityInventory[1] = temp;
            Debug.Log($"Swapped to: {abilityInventory[activeAbilityIndex]}");

            lastSwapTime = Time.time;
            abilityUI.UpdateUI();
        }
    }

    public AbilityType GetAbilityInSlot(int index) {
        return abilityInventory[index];
    }

    private void UpdateAbilitySlot(AbilityType usedAbility) {
        for (int i = 0; i < abilityInventory.Length; i++) {
            if (abilityInventory[i] == usedAbility) {
                abilityInventory[i] = AbilityType.None;
                break;
            }
        }
    }

    public int GetActiveAbilityIndex() {
        return activeAbilityIndex;
    }

    #region AbilityLogic
    // DOUBLE JUMP
    private void dJump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        hasDoubleJump = false;
    }


    // DASHING
    private void Dash() {
        pc.dashing = true;

        Vector3 dashDirection = playerCamera.forward.normalized;

        Vector3 forceToApply = dashDirection * dashForce;
        delayedForceToApply = forceToApply;

        rb.velocity = Vector3.zero;

        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(StopDash), dashDuration);

        hasDash = false;
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
    }

    // GRAPPLING
    private void Grapple() {

        RaycastHit hit;

        Debug.DrawRay(playerCamera.position, playerCamera.forward * 75, Color.red, 5);

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 75))
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy") {
                Debug.Log("Grapple Has hit");

                grapplePoint = hit.point;
                isGrappling = true;
                lr.enabled = true;

                hasGrapple = false;
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

        if (lr) {
            lr.enabled = false;
        }
    }

    void DrawGrappleLine() {
        if (lr && isGrappling) {
            lr.SetPosition(0, grappleOrigin.position);
            lr.SetPosition(1, grapplePoint);
        }
    }
    #endregion
}
