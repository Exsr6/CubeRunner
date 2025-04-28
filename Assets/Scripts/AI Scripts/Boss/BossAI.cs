using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour {

    [Header("References")]
    public Transform _playerTransform;
    public Transform _shootPoint;
    public GameObject _projectilePrefab;
    public Material[] _Colourstages;
    private Renderer _bossRenderer;

    [Header("Variables")]
    public bool bCanTakeDamage = true;
    public int iMaxHealth = 100;
    public int iCurrentHealth;

    public enum BossAttackingState {
        Pistol,
        Burst,
        Shotgun
    }

    private BossAttackingState currentState;

    void Start() {
        // set current health to max health
        iCurrentHealth = iMaxHealth;
        // get boss renderer component
        _bossRenderer = GetComponent<Renderer>();
    }

    private void Update() {
        //Debug.Log("Current Health: " + iCurrentHealth);

    }

    public void TakeDamage(int damage) {
        // Can Only take damage if variable is true
        if (bCanTakeDamage) {
            // Take Damage
            iCurrentHealth -= damage;
            // Update attack mode / boss state Function
            UpdateAttackMode();
            // If health is less than 0, set to 0 and destroy the boss object
            if (iCurrentHealth < 0) {
                iCurrentHealth = 0;
                Destroy(gameObject); // Destroy the boss object when health reaches 0
                Time.timeScale = 0f; // Pause the game
                FindObjectOfType<Timer>().CompleteLevel();
            }
        }
    }

    void UpdateAttackMode() {
        // Update health percentage
        float healthPercentage = (float)iCurrentHealth / iMaxHealth;

        // Change attack mode based on health percentage
        if (healthPercentage > 0.66f) {
            currentState = BossAttackingState.Pistol;
            _bossRenderer.material = _Colourstages[0];
        }
        else if (healthPercentage > 0.33f) {
            currentState = BossAttackingState.Burst;
            _bossRenderer.material = _Colourstages[1];
        }
        else {
            currentState = BossAttackingState.Shotgun;
            _bossRenderer.material = _Colourstages[2];
        }
    }

    public void Shoot() {
        // Call the appropriate shooting logic based on the current state
        switch (currentState) {
            case BossAttackingState.Pistol:
                PistolLogic();
                Debug.Log("Pistol Logic");
                break;
            case BossAttackingState.Burst:
                StartCoroutine(BurstLogic(3, 0.15f));
                Debug.Log("Burst Logic");
                break;
            case BossAttackingState.Shotgun:
                ShotgunLogic(5, 30f);
                Debug.Log("Shotgun Logic");
                break;
        }
    }

    void FireBullet(Vector3 targetPos) {
        // Calculate the direction from the shoot point to the target position
        Vector3 dir = (targetPos - _shootPoint.position).normalized;
        FireBullet(_shootPoint.position, dir);
    }

    void FireBullet(Vector3 origin, Vector3 direction) {
        // Create the bullet and set its rigid body velocity
        GameObject bullet = Instantiate(_projectilePrefab, origin, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
        if (rb != null)
            rb.velocity = direction * 25f;
    }

    void PistolLogic() {
        // Call the FireBullet function with the player's position
        FireBullet(_playerTransform.position);
    }

    IEnumerator BurstLogic(int ShotCount, float delayBetweenShots) {
        // Loop through the number of shots and fire bullets
        for (int i = 0; i < ShotCount; i++) {
            FireBullet(_playerTransform.position);
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    void ShotgunLogic(int PelletCount, float spreadAngle) {
        // Calculate the forward direction from the shoot point to the player
        Vector3 forward = (_playerTransform.position - _shootPoint.position).normalized;

        // loop through the number of pellets and fire bullets in a spread pattern
        for (int i = 0; i < PelletCount; i++) {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 direction = rotation * forward;

            // Call the FireBullet function with the shoot point and direction
            FireBullet(_shootPoint.position + direction * 0.5f, direction);
        }
    }
}
