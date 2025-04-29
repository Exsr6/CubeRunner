using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AbilityPickups;
using static PlayerController;

public class AbilitySystem : MonoBehaviour
{
    [Header("References")]
    Rigidbody _rb;
    public Transform _playerCamera;
    private PlayerController _pc;
    public LayerMask _isWall;
    public Transform _grappleOrigin;
    private LineRenderer _lr;
    private AbilityUI _abilityUI;
    public AudioSource _UseAbilitySound;
    public AudioSource _SwapAbilitySound;

    [Header("DoubleJump")]
    [SerializeField] private float fJumpForce = 11f;
    [HideInInspector] public int iDoubleJumpCount = 0;

    [Header("Dash")]
    [SerializeField] private float fDashForce = 25f;
    [SerializeField] private float fDashDuration = 0.25f;
    private Vector3 delayedForceToApply;
    [HideInInspector] public int iDashCount = 0;

    [Header("Slide")]
    [SerializeField] private float fSlideForce = 10f;
    [SerializeField] private float fSlideDuration = 0.4f;
    [HideInInspector] public int iSlideCount = 0;
    private float fStartYScale;
    private float fCrouchYScale = 0.5f;

    [Header("Grapple")]
    public float fGrappleSpeed = 20f;
    public float fStopDistance = 1.5f;
    [HideInInspector] public int iGrappleCount = 0;
    [HideInInspector] public bool bIsGrappling = false;
    private Vector3 grapplePoint;

    [Header("Ability Inventory")]
    public AbilityType[] abilityInventory = new AbilityType[2] { AbilityType.None, AbilityType.None };
    private int iActiveAbilityIndex = 0;
    private float fSwapCooldown = 0.5f;
    private float fLastSwapTime = 0f;
    private float fAbilityCooldown = 0.2f;
    private float fLastAbilityTime = 0f;


    [Header("Keybinds")]
    [SerializeField] private KeyCode _abilityKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode _swapAbilityKey = KeyCode.Q;

    public enum AbilityType {
        None,
        DoubleJump,
        Dash,
        Slide,
        Grapple
    }

    void Start() {

        // Get Components
        _rb = GetComponent<Rigidbody>();

        _pc = GetComponent<PlayerController>();

        _lr = GetComponent<LineRenderer>();

        // Get UI Reference
        _abilityUI = FindObjectOfType<AbilityUI>();

        fStartYScale = transform.localScale.y;
    }

    private void FixedUpdate() {
        AbilityHandler();

        // Input For Swapping Abilities
        if (Input.GetKey(_swapAbilityKey))
            SwapAbilities();

        // Start and Draw Grapple Line
        if (bIsGrappling) {
            StartGrapple();
            DrawGrappleLine();
        }

        // Stop Grapple if the player presses jump
        if (bIsGrappling && Input.GetKey(KeyCode.Space))
            StopGrapple();
    }

    private void AbilityHandler() {
        // Cooldown to stop the player from spamming the ability key
        if (Time.time - fLastAbilityTime >= fAbilityCooldown) {
            // Input for using ability
            if (Input.GetKey(_abilityKey)) {
                useAbilities(abilityInventory[0]);
                // reset ability time
                fLastAbilityTime = Time.time;
            }
        }
        // Update UI
        _abilityUI.UpdateUI();
    }

    public void pickupAbility(AbilityType newAbility) 
    {
        // if the slot 1 is none then make it the slot 1 ability
        if (abilityInventory[0] == AbilityType.None) {
            abilityInventory[0] = newAbility;
            UnlockAbility(newAbility);
        }

        // if the slot 2 is none then make it the slot 2 ability
        // only after checking the slot 1 first
        else if (abilityInventory[1] == AbilityType.None) {
            abilityInventory[1] = newAbility;
            UnlockAbility(newAbility);
        }

        // then if both slots are full, swap the ability with the new picked up one
        else {
            AbilityType replacedAbility = abilityInventory[iActiveAbilityIndex];
            switch (replacedAbility) {
                // if the replaced ability is Doublejump, then remove 1 from the double jump count
                case AbilityType.DoubleJump:
                    iDoubleJumpCount = Mathf.Max(0, iDoubleJumpCount - 1);
                    break;
                // if the replaced ability is Dash, then remove 1 from the dash count
                case AbilityType.Dash:
                    iDashCount = Mathf.Max(0, iDashCount - 1);
                    break;
                // if the replaced ability is Slide, then remove 1 from the slide count
                case AbilityType.Slide:
                    iSlideCount = Mathf.Max(0, iSlideCount - 1);
                    break;
                // if the replaced ability is Grapple, then remove 1 from the grapple count
                case AbilityType.Grapple:
                    iGrappleCount = Mathf.Max(0, iGrappleCount - 1);
                    break;
            }

            // set the replaced ability to the new ability
            abilityInventory[iActiveAbilityIndex] = newAbility;
            // "unlock" the new ability
            UnlockAbility(newAbility);
        }

        // UpdateUI and play the sound
        _abilityUI.UpdateUI();
        _SwapAbilitySound.Play();
    }
    private void UnlockAbility(AbilityType ability) {
        switch (ability) {
            // Increase Double Jump Count
            case AbilityType.DoubleJump:
                iDoubleJumpCount++; 
                break;
            // Increase Dash Count
            case AbilityType.Dash:
                iDashCount++;
                break;
            // Increase Slide Count
            case AbilityType.Slide:
                iSlideCount++;
                break;
            // Increase Grapple Count
            case AbilityType.Grapple:
                iGrappleCount++;
                break;
        }
    }

    private void useAbilities(AbilityType ability) {
        if (!Input.GetKey(_abilityKey)) return;

        switch (ability) {
            case AbilityType.DoubleJump:
                // Check if the player has a double jump count above 0
                if (iDoubleJumpCount > 0) {
                    // Call Double Jump function
                    dJump();
                    // Update the ability slot
                    UpdateAbilitySlot(AbilityType.DoubleJump);
                    // Play the ability sound
                    _UseAbilitySound.Play();
                    // After use make the second slot ability the first if the ability isn't empty
                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Dash:
                // Check if the player has a dash count above 0
                if (iDashCount > 0) {
                    // Call Dash function
                    Dash();
                    // Update the ability slot
                    UpdateAbilitySlot(AbilityType.Dash);
                    // Play the ability sound
                    _UseAbilitySound.Play();
                    // After use make the second slot ability the first if the ability isn't empty
                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Slide:
                // Check if the player has a slide count above 0
                if (iSlideCount > 0 && (_pc.bIsGrounded || _pc.bIsWater)) {
                    // Call Slide function
                    Slide();
                    // Update the ability slot
                    UpdateAbilitySlot(AbilityType.Slide);
                    // Play the ability sound
                    _UseAbilitySound.Play();
                    // After use make the second slot ability the first if the ability isn't empty
                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Grapple:
                // Check if the player has a grapple count above 0
                if (iGrappleCount > 0) {
                    // Call Grapple function
                    Grapple();
                    // Update the ability slot
                    UpdateAbilitySlot(AbilityType.Grapple);
                    // Play the ability sound
                    _UseAbilitySound.Play();
                    // After use make the second slot ability the first if the ability isn't empty
                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;
        }

        // Update the UI after using an ability
        _abilityUI.UpdateUI();
    }

    private void SwapAbilities() {
        // if statement makes it so the player can't spam the swap key
        if (Time.time - fLastSwapTime >= fSwapCooldown)
        {
            // create temp to create a copy of the first ability
            AbilityType temp = abilityInventory[0];
            // set the second ability to the first ability
            abilityInventory[0] = abilityInventory[1];
            // set the first ability to the temp ability
            abilityInventory[1] = temp;
            Debug.Log($"Swapped to: {abilityInventory[iActiveAbilityIndex]}");

            // reset swap time
            fLastSwapTime = Time.time;
            // update UI
            // Update the UI after swapping the ability
        }
    }

    public AbilityType GetAbilityInSlot(int index) {
        // Get the ability in the slot
        return abilityInventory[index];
    }

    private void UpdateAbilitySlot(AbilityType usedAbility) {
        for (int i = 0; i < abilityInventory.Length; i++) {
            if (abilityInventory[i] == usedAbility) {
                abilityInventory[i] = AbilityType.None;

                switch (usedAbility) {
                    // Decrease the double jump count
                    case AbilityType.DoubleJump:
                        iDoubleJumpCount = Mathf.Max(0, iDoubleJumpCount - 1);
                        break;
                    // Decrease the dash count
                    case AbilityType.Dash:
                        iDashCount = Mathf.Max(0, iDashCount - 1);
                        break;
                    // Decrease the slide count
                    case AbilityType.Slide:
                        iSlideCount = Mathf.Max(0, iSlideCount - 1);
                        break;
                    // Decrease the grapple count
                    case AbilityType.Grapple:
                        iGrappleCount = Mathf.Max(0, iGrappleCount - 1);
                        break;
                }
                break;
            }
        }
    }

    public int GetActiveAbilityIndex() {
        // Get the active ability index
        return iActiveAbilityIndex;
    }

    #region AbilityLogic
    // DOUBLE JUMP
    private void dJump() {
        // reset upward velocity so momentum doesn't carry over
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // Add Force Upwards
        _rb.AddForce(transform.up * fJumpForce, ForceMode.Impulse);
    }


    // DASHING
    private void Dash() {
        _pc.dashing = true;

        // Get Dash Direction
        Vector3 dashDirection = _playerCamera.forward.normalized;

        // Calculate Dash Force
        Vector3 forceToApply = dashDirection * fDashForce;
        // Delay the force to apply slightly
        delayedForceToApply = forceToApply;

        // reset velocity so momentum doesn't carry over
        _rb.velocity = Vector3.zero;

        // Delay the force to apply slightly
        Invoke(nameof(DelayedDashForce), 0.025f);

        // Stop the dash after a certain duration
        Invoke(nameof(StopDash), fDashDuration);
    }

    private void DelayedDashForce() {
        // Apply the delayed force
        _rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void StopDash() {
        // set dashing to false
        _pc.dashing = false;
    }


    // SLIDING
    private void Slide() {

        // set sliding to true
        _pc.sliding = true;

        // Make the player crouch slighly to make it look like the player is sliding
        transform.localScale = new Vector3(transform.localScale.x, fCrouchYScale, transform.localScale.z);
        // Add force downwards to make the player slide
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // apply the slide force after a delay
        Invoke(nameof(DelayedSlideForce), 0.1f);

        // call stop sliding after a certain duration
        Invoke(nameof(StopSliding), fSlideDuration);
    }

    private void DelayedSlideForce() {
        // Apply the delayed force forwards
        _rb.AddForce(_playerCamera.forward * fSlideForce, ForceMode.VelocityChange);
    }

    private void StopSliding() {
        // reset back to normal scale
        transform.localScale = new Vector3(transform.localScale.x, fStartYScale, transform.localScale.z);
        // set movement speed back to normal
        _pc.movementSpeed = _pc.walkSpeed;
        // set sliding to false
        _pc.sliding = false;
    }

    // GRAPPLING
    private void Grapple() {

        RaycastHit hit;

        // Debug raycast for testing
        Debug.DrawRay(_playerCamera.position, _playerCamera.forward * 75, Color.red, 5);

        // Check if raycast hits a wall or enemy
        if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, 75))
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy") {
                Debug.Log("Grapple Has hit");

                grapplePoint = hit.point;
                bIsGrappling = true;
                _lr.enabled = true;
            }
    }

    private void StartGrapple() {
        // get grapple direction and distance
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        // set the players velocity to the grapple direction until the stop distance is reached
        if (distance > fStopDistance) {
            _rb.velocity = direction * fGrappleSpeed;
        }
        // call stop grapple when stop distance is reached
        else {
            StopGrapple();
        }
    }

    void StopGrapple() {
        // set grappling to false
        bIsGrappling = false;
        // Jump after grappling using the same doublejump function
        dJump();

        // set line renderer to false
        if (_lr) {
            _lr.enabled = false;
        }
    }

    void DrawGrappleLine() {
        if (_lr && bIsGrappling) {
            // set linerender positions
            _lr.SetPosition(0, _grappleOrigin.position);
            _lr.SetPosition(1, grapplePoint);
        }
    }
    #endregion
}
