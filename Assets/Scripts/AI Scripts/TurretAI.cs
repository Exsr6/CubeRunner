using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("References")]
    private PlayerController pc;
    private Death death;

    public float detectionRange = 10f;
    public LineRenderer lineRenderer;
    public float killDelay = 3f;

    private bool isTargeting = false;
    private Coroutine killCoroutine;

    void Start() {

        pc = FindObjectOfType<PlayerController>();
        death = FindObjectOfType<Death>();


        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
    }

    void Update() {
        if (pc != null) {
            float distance = Vector3.Distance(transform.position, pc.transform.position);
            if (distance <= detectionRange) {
                if (!isTargeting) {
                    StartTargeting();
                }
                else {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, pc.transform.position);
                }
            }
            else {
                StopTargeting();
            }
        }
    }

    void StartTargeting() {
        isTargeting = true;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pc.transform.position);

        killCoroutine = StartCoroutine(KillPlayerAfterDelay());
    }

    void StopTargeting() {
        isTargeting = false;
        lineRenderer.enabled = false;

        if (killCoroutine != null) {
            StopCoroutine(killCoroutine);
            killCoroutine = null;
        }
    }

    IEnumerator KillPlayerAfterDelay() {
        yield return new WaitForSeconds(killDelay);

        // Check if the player is still in range
        float distance = Vector3.Distance(transform.position, pc.transform.position);
        if (distance <= detectionRange) {
            Debug.Log("Player Killed");
            death.Die();
        }
    }
}
