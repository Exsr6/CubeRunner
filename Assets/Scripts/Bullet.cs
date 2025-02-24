using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.TryGetComponent<EnemyController>(out EnemyController enemyComponent)) {
            enemyComponent.TakeDamage(1);
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
