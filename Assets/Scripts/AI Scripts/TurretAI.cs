using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("References")]
    public LineRenderer _lineRenderer;

    private PlayerController _pc;
    private Death _death;
    private Coroutine _killCoroutine;

    [Header("Variables")]
    public float fDetectionRange = 25f;
    public float fKillDelay = 3f;

    private bool bIsTargeting = false;

    void Start() {

        // find the player controller and death handler in the scene
        _pc = FindObjectOfType<PlayerController>();
        _death = FindObjectOfType<Death>();

        // get the LineRenderer component
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        // disable the line renderer at the start
        _lineRenderer.enabled = false;
    }

    void Update() {
        // ensure the player reference exists before proceeding
        if (_pc != null) {

            // calculate the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, _pc.transform.position);

            // if the player is within detection range, start targeting
            if (distance <= fDetectionRange) {

                if (!bIsTargeting) {

                    // begin targeting sequence
                    StartTargeting();
                }

                else {
                    // continuously update the line renderer to track the player's position
                    _lineRenderer.SetPosition(0, transform.position);
                    _lineRenderer.SetPosition(1, _pc.transform.position);

                }
            }
            else {
                // stop targeting if the player moves out of range
                StopTargeting();
            }
        }
    }

    void StartTargeting() {
        bIsTargeting = true;
        _lineRenderer.enabled = true;

        // set line positions
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _pc.transform.position);

        // start the routine that will kill the player
        _killCoroutine = StartCoroutine(KillPlayerAfterDelay());
    }

    void StopTargeting() {
        bIsTargeting = false;
        _lineRenderer.enabled = false;

        // stop the routine if player moves away
        if (_killCoroutine != null) {
            StopCoroutine(_killCoroutine);
            _killCoroutine = null;
        }
    }

    IEnumerator KillPlayerAfterDelay() {
        // delay the player death by the killDelay float value
        yield return new WaitForSeconds(fKillDelay);

        // check if the player is still in range
        float distance = Vector3.Distance(transform.position, _pc.transform.position);
        if (distance <= fDetectionRange) {
            Debug.Log("Player Killed");

            // kill the player
            _death.Die();
        }
    }
}
