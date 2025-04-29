using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour {
    [Header("References")]
    public List<Transform> _pathPoints;
    public List<float> _waitDurations;
    public Transform _playerTransform;
    public GameObject _forceField;

    private BossAI _bossAI;

    [Header("Variables")]
    public float fMoveSpeed = 20f;
    public float fDefaultStopDuration = 1f;

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

        Vector3 lookDir = _playerTransform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        if (bIsWaiting) return;

        if (iCurrentPointIndex != _pathPoints.Count) {
            // Move towards the next point
            Transform Target = _pathPoints[iCurrentPointIndex];
            transform.position = Vector3.MoveTowards(transform.position, Target.position, fMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, Target.position) < 0.1f) {
                // Stop and wait at the point
                StartCoroutine(WaitAtPoint());
            }
        }
        else {
            _bossAI.bCanTakeDamage = true;
        }
    }

    private IEnumerator WaitAtPoint() {
        // Make the boss wait at the current point and make vulnerable
        bIsWaiting = true;
        _bossAI.bCanTakeDamage = true;
        _forceField.SetActive(false);

        Debug.Log("Waiting at point " + iCurrentPointIndex);

        float fWaitTime = (iCurrentPointIndex < _waitDurations.Count) ? _waitDurations[iCurrentPointIndex] : fDefaultStopDuration;
        float fShootCooldown = 0.5f;
        float fShootTimer = 0f;

        // While waiting, shoot at the player
        while (fWaitTime > 0) {

            fShootTimer -= Time.deltaTime;
            if (fShootTimer <= 0f) {
                _bossAI.Shoot();
                fShootTimer = fShootCooldown;
            }

            fWaitTime -= Time.deltaTime;
            yield return null;
        }

        // Move to the next point and make invulnerable
        Debug.Log("Moving to point " + iCurrentPointIndex);
        iCurrentPointIndex = (iCurrentPointIndex + 1);
        bIsWaiting = false;
        _bossAI.bCanTakeDamage = false;
        _forceField.SetActive(true);
    }

}
