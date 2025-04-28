using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Update() {
        // Destroy bullet after 5 seconds
        Destroy(gameObject, 5f);
    }

    // Bullet collision handler for each collidable surface
    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.TryGetComponent<EnemyController>(out EnemyController enemyComponent)) {
            enemyComponent.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.gameObject.TryGetComponent<BossAI>(out BossAI bAI)) {
            bAI.TakeDamage(1);
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<EnemyController>(out EnemyController enemyComponent)) {
            enemyComponent.TakeDamage(1);
            Destroy(gameObject);
        }
    }

}
