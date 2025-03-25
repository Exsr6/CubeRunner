using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("References")]
    private PlayerController pc;
    private Death death;

    [Header("Variables")]
    public float detectionRange = 25f;
    public LineRenderer lineRenderer;
    public float killDelay = 3f;

    private bool isTargeting = false;
    private Coroutine killCoroutine;

    void Start() {

        // find the player controller and death handler in the scene
        pc = FindObjectOfType<PlayerController>();
        death = FindObjectOfType<Death>();

        // get the LineRenderer component
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // disable the line renderer at the start
        lineRenderer.enabled = false;
    }

    void Update() {
        // ensure the player reference exists before proceeding
        if (pc != null) {

            // calculate the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, pc.transform.position);

            // if the player is within detection range, start targeting
            if (distance <= detectionRange) {

                if (!isTargeting) {

                    // begin targeting sequence
                    StartTargeting();
                }

                else {
                    // continuously update the line renderer to track the player's position
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, pc.transform.position);

                }
            }
            else {
                // stop targeting if the player moves out of range
                StopTargeting();
            }
        }
    }

    void StartTargeting() {
        isTargeting = true;
        lineRenderer.enabled = true;

        // set line positions
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pc.transform.position);

        // start the routine that will kill the player
        killCoroutine = StartCoroutine(KillPlayerAfterDelay());
    }

    void StopTargeting() {
        isTargeting = false;
        lineRenderer.enabled = false;

        // stop the routine if player moves away
        if (killCoroutine != null) {
            StopCoroutine(killCoroutine);
            killCoroutine = null;
        }
    }

    IEnumerator KillPlayerAfterDelay() {
        // delay the player death by the killDelay float value
        yield return new WaitForSeconds(killDelay);

        // check if the player is still in range
        float distance = Vector3.Distance(transform.position, pc.transform.position);
        if (distance <= detectionRange) {
            Debug.Log("Player Killed");

            // kill the player
            death.Die();
        }
    }
}
