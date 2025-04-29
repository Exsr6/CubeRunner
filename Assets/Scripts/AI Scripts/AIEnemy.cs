using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour {
    [Header("References")]
    public Transform _player;
    public GameObject _bulletPrefab;
    public Transform _firePoint;

    private NavMeshAgent _agent;
    private Rigidbody _rb;

    [Header("Variables")]
    public float fWalkRadius = 10f;
    public float fAttackRange = 20f;
    public float fFireRate = 2f;
    public float fJumpForce = 5f;
    public float fGravityMultiplier = 2f;
    public float fJumpHeight = 2f;
    public float fJumpDuration = 0.5f;

    private float fFireCooldown = 0f;
    private float fWaitTime = 0f;
    private float fWaitCounter = 0f;
    private bool bIsJumping = false;

    void Start() {
        // Get Components
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        _agent.autoBraking = true;
        _agent.autoTraverseOffMeshLink = false;
    }

    void Update() {
        if (_agent == null || _player == null) return;

        fFireCooldown -= Time.deltaTime;

        // Get distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Check if player is within attack range
        if (distanceToPlayer < fAttackRange) {
            transform.LookAt(_player);
            _agent.isStopped = true;
            Shoot();
        }
        // else player is outside attack range
        else {
            _agent.isStopped = false;
            RandomWalk();
        }

        // Apply gravity manually when falling
        if (!_agent.isOnOffMeshLink && !bIsJumping) {
            _rb.velocity += Vector3.down * fGravityMultiplier * Time.deltaTime;
        }

        // Handle jumping over obstacles
        if (_agent.isOnOffMeshLink && !bIsJumping) {
            StartCoroutine(JumpAcrossGap());
        }

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void RandomWalk() {
        // Pick random location if not already moving
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f) {
            fWaitCounter += Time.deltaTime;
            if (fWaitCounter >= fWaitTime) {
                PickLocation();
                fWaitCounter = 0;
            }
        }
    }

    void PickLocation() {
        // Get a random point within the walk radius
        Vector3 randomDirection = Random.insideUnitSphere * fWalkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

        // Check if the random point is on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, fWalkRadius, NavMesh.AllAreas)) {
            _agent.SetDestination(hit.position);
        }
    }

    void Shoot() {
        if (fFireCooldown <= 0f) {
            Vector3 direction = (_player.position - _firePoint.position).normalized;
            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = direction * 50f;

            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

            fFireCooldown = 1f / fFireRate;
        }
    }

    // Handle jumping across gaps using navlinks
    IEnumerator JumpAcrossGap() {
        bIsJumping = true;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPos = _agent.transform.position;
        Vector3 endPos = data.endPos;
        float elapsedTime = 0f;

        _agent.isStopped = true;

        while (elapsedTime < fJumpDuration) {
            float t = elapsedTime / fJumpDuration;
            float height = Mathf.Sin(t * Mathf.PI) * fJumpHeight;
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _agent.Warp(endPos);
        _agent.isStopped = false;
        _agent.CompleteOffMeshLink();

        bIsJumping = false;
    }
}