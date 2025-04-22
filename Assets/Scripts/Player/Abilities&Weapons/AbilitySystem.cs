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
        _rb = GetComponent<Rigidbody>();

        _pc = GetComponent<PlayerController>();

        _lr = GetComponent<LineRenderer>();

        _abilityUI = FindObjectOfType<AbilityUI>();

        fStartYScale = transform.localScale.y;
    }

    private void FixedUpdate() {
        AbilityHandler();

        if (Input.GetKey(_swapAbilityKey))
            SwapAbilities();

        if (bIsGrappling)
            StartGrapple(); DrawGrappleLine();

        if (bIsGrappling && Input.GetKey(KeyCode.Space))
            StopGrapple();

        Debug.Log(iDoubleJumpCount);
    }

    private void AbilityHandler() {
        if (Time.time - fLastAbilityTime >= fAbilityCooldown) {
            if (Input.GetKey(_abilityKey)) {
                useAbilities(abilityInventory[0]);
                fLastAbilityTime = Time.time;
            }
        }

        _abilityUI.UpdateUI();
    }

    public void pickupAbility(AbilityType newAbility) 
    {
        if (abilityInventory[0] == AbilityType.None) {
            abilityInventory[0] = newAbility;
            UnlockAbility(newAbility);
        }

        else if (abilityInventory[1] == AbilityType.None) {
            abilityInventory[1] = newAbility;
            UnlockAbility(newAbility);
        }

        else {
            AbilityType replacedAbility = abilityInventory[iActiveAbilityIndex];
            switch (replacedAbility) {
                case AbilityType.DoubleJump:
                    iDoubleJumpCount = Mathf.Max(0, iDoubleJumpCount - 1);
                    break;
                case AbilityType.Dash:
                    iDashCount = Mathf.Max(0, iDashCount - 1);
                    break;
                case AbilityType.Slide:
                    iSlideCount = Mathf.Max(0, iSlideCount - 1);
                    break;
                case AbilityType.Grapple:
                    iGrappleCount = Mathf.Max(0, iGrappleCount - 1);
                    break;
            }

            abilityInventory[iActiveAbilityIndex] = newAbility;
            UnlockAbility(newAbility);
        }

        _abilityUI.UpdateUI();
    }
    private void UnlockAbility(AbilityType ability) {
        switch (ability) {
            case AbilityType.DoubleJump:
                iDoubleJumpCount++; 
                break;
            case AbilityType.Dash:
                iDashCount++;
                break;
            case AbilityType.Slide:
                iSlideCount++;
                break;
            case AbilityType.Grapple:
                iGrappleCount++;
                break;
        }
    }

    private void useAbilities(AbilityType ability) {
        if (!Input.GetKey(_abilityKey)) return;

        switch (ability) {
            case AbilityType.DoubleJump:
                if (iDoubleJumpCount > 0) {
                    dJump();
                    UpdateAbilitySlot(AbilityType.DoubleJump);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Dash:
                if (iDashCount > 0) {
                    Dash();
                    UpdateAbilitySlot(AbilityType.Dash);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Slide:
                if (iSlideCount > 0 && (_pc.bIsGrounded || _pc.bIsWater)) {
                    Slide();
                    UpdateAbilitySlot(AbilityType.Slide);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;

            case AbilityType.Grapple:
                if (iGrappleCount > 0) {
                    Grapple();
                    UpdateAbilitySlot(AbilityType.Grapple);

                    if (abilityInventory[0] == AbilityType.None) {
                        SwapAbilities();
                    }
                }
                break;
        }

        _abilityUI.UpdateUI();
    }

    private void SwapAbilities() {
        if (Time.time - fLastSwapTime >= fSwapCooldown)
        {
            AbilityType temp = abilityInventory[0];
            abilityInventory[0] = abilityInventory[1];
            abilityInventory[1] = temp;
            Debug.Log($"Swapped to: {abilityInventory[iActiveAbilityIndex]}");

            fLastSwapTime = Time.time;
            _abilityUI.UpdateUI();
        }
    }

    public AbilityType GetAbilityInSlot(int index) {
        return abilityInventory[index];
    }

    private void UpdateAbilitySlot(AbilityType usedAbility) {
        for (int i = 0; i < abilityInventory.Length; i++) {
            if (abilityInventory[i] == usedAbility) {
                abilityInventory[i] = AbilityType.None;

                switch (usedAbility) {
                    case AbilityType.DoubleJump:
                        iDoubleJumpCount = Mathf.Max(0, iDoubleJumpCount - 1);
                        break;
                    case AbilityType.Dash:
                        iDashCount = Mathf.Max(0, iDashCount - 1);
                        break;
                    case AbilityType.Slide:
                        iSlideCount = Mathf.Max(0, iSlideCount - 1);
                        break;
                    case AbilityType.Grapple:
                        iGrappleCount = Mathf.Max(0, iGrappleCount - 1);
                        break;
                }
                break;
            }
        }
    }

    public int GetActiveAbilityIndex() {
        return iActiveAbilityIndex;
    }

    #region AbilityLogic
    // DOUBLE JUMP
    private void dJump() {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * fJumpForce, ForceMode.Impulse);
    }


    // DASHING
    private void Dash() {
        _pc.dashing = true;

        Vector3 dashDirection = _playerCamera.forward.normalized;

        Vector3 forceToApply = dashDirection * fDashForce;
        delayedForceToApply = forceToApply;

        _rb.velocity = Vector3.zero;

        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(StopDash), fDashDuration);
    }

    private void DelayedDashForce() {
        _rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void StopDash() {
        _pc.dashing = false;
    }


    // SLIDING
    private void Slide() {

        _pc.sliding = true;

        transform.localScale = new Vector3(transform.localScale.x, fCrouchYScale, transform.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        Invoke(nameof(DelayedSlideForce), 0.1f);

        Invoke(nameof(StopSliding), fSlideDuration);
    }

    private void DelayedSlideForce() {
        _rb.AddForce(_playerCamera.forward * fSlideForce, ForceMode.VelocityChange);
    }

    private void StopSliding() {
        transform.localScale = new Vector3(transform.localScale.x, fStartYScale, transform.localScale.z);
        _pc.movementSpeed = _pc.walkSpeed;
        _pc.sliding = false;
    }

    // GRAPPLING
    private void Grapple() {

        RaycastHit hit;

        Debug.DrawRay(_playerCamera.position, _playerCamera.forward * 75, Color.red, 5);

        if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, 75))
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy") {
                Debug.Log("Grapple Has hit");

                grapplePoint = hit.point;
                bIsGrappling = true;
                _lr.enabled = true;
            }
    }

    private void StartGrapple() {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance > fStopDistance) {
            _rb.velocity = direction * fGrappleSpeed;
        }
        else {
            StopGrapple();
        }
    }

    void StopGrapple() {
        bIsGrappling = false;
        dJump();

        if (_lr) {
            _lr.enabled = false;
        }
    }

    void DrawGrappleLine() {
        if (_lr && bIsGrappling) {
            _lr.SetPosition(0, _grappleOrigin.position);
            _lr.SetPosition(1, grapplePoint);
        }
    }
    #endregion
}
