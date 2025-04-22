using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour {

    [Header("References")]
    public Transform playerTransform;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    public bool bCanTakeDamage = true;


    public int iMaxHealth = 100;
    public int iCurrentHealth;

    public enum BossAttackingState {
        Pistol,
        Burst,
        Shotgun
    }

    private BossAttackingState currentState;

    // Start is called before the first frame update
    void Start() {
        iCurrentHealth = iMaxHealth;
    }

    private void Update() {
        //Debug.Log("Current Health: " + iCurrentHealth);

    }

    public void TakeDamage(int damage) {
        if (bCanTakeDamage) {
            iCurrentHealth -= damage;
            UpdateAttackMode();
            if (iCurrentHealth < 0) {
                iCurrentHealth = 0;
                Destroy(gameObject); // Destroy the boss object when health reaches 0
                Time.timeScale = 0f; // Pause the game
                FindObjectOfType<Timer>().CompleteLevel();
            }
        }
    }

    void UpdateAttackMode() {
        float healthPercentage = (float)iCurrentHealth / iMaxHealth;

        if (healthPercentage > 0.66f) {
            currentState = BossAttackingState.Pistol;
        }
        else if (healthPercentage > 0.33f) {
            currentState = BossAttackingState.Burst;
        }
        else {
            currentState = BossAttackingState.Shotgun;
        }
    }

    public void Shoot() {
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
        Vector3 dir = (targetPos - shootPoint.position).normalized;
        FireBullet(shootPoint.position, dir);
    }

    void FireBullet(Vector3 origin, Vector3 direction) {
        GameObject bullet = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
        if (rb != null)
            rb.velocity = direction * 25f;
    }

    void PistolLogic() {
        FireBullet(playerTransform.position);
    }

    IEnumerator BurstLogic(int ShotCount, float delayBetweenShots) {
        for (int i = 0; i < ShotCount; i++) {
            FireBullet(playerTransform.position);
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    void ShotgunLogic(int PelletCount, float spreadAngle) {
        Vector3 forward = (playerTransform.position - shootPoint.position).normalized;

        for (int i = 0; i < PelletCount; i++) {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 direction = rotation * forward;

            FireBullet(shootPoint.position + direction * 0.5f, direction);
        }
    }
}
