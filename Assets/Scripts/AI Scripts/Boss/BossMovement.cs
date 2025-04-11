using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour {
    [Header("References")]
    public List<Transform> _pathPoints;
    public Transform playerTransform;

    private BossAI _bossAI;

    [Header("Variables")]
    public float fMoveSpeed = 20f;
    public float fStopDuration = 1f;

    private int iCurrentPointIndex = 0;
    private bool bIsWaiting = false;

    private void Start() {
        _bossAI = GetComponent<BossAI>();
        if (_pathPoints.Count == 0) {
            Debug.LogError("No path points assigned to the boss movement script.");
            enabled = false;
    }
}

    void Update() {
        if (bIsWaiting) return;

        // Move towards the next point
        Transform Target = _pathPoints[iCurrentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, Target.position, fMoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, Target.position) < 0.1f) {
            // Stop and wait at the point
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint() {
        bIsWaiting = true;
        _bossAI.bCanTakeDamage = true;

        Debug.Log("Waiting at point " + iCurrentPointIndex);

        float fWaitTime = fStopDuration;
        float fShootCooldown = 1f; // Fire every 1 second
        float fShootTimer = 0f;

        while (fWaitTime > 0) {
            if (playerTransform != null) {
                Vector3 lookDir = playerTransform.position - transform.position;
                lookDir.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDir);
            }

            fShootTimer -= Time.deltaTime;
            if (fShootTimer <= 0f) {
                _bossAI.Shoot();
                fShootTimer = fShootCooldown;
            }

            fWaitTime -= Time.deltaTime;
            yield return null;
        }

        // Move to the next point
        Debug.Log("Moving to point " + iCurrentPointIndex);
        iCurrentPointIndex = (iCurrentPointIndex + 1) % _pathPoints.Count;
        bIsWaiting = false;
        _bossAI.bCanTakeDamage = false;
    }

}
