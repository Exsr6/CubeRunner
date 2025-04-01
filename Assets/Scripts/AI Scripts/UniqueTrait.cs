using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueTrait : MonoBehaviour
{
    [Header("References")]
    private EnemyController _self;
    private PlayerController _pc;

    [Header("Enemy Type")]
    public enemyType enemy;

    public enum enemyType
    {
        None,
        Basic,
        Sphere,
        Turret
    }

    private void Start() {
        // get the enemycontroller component
        _self = GetComponent<EnemyController>();

        // find the player object in the scene and get it
        _pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {

        // check if player collides with the enemy type sphere
        if (other.transform.CompareTag("Player") && enemy == enemyType.Sphere) {
            // enemy takes damage
            _self.TakeDamage(1);

            // player forced to jump (kind of like when mario jumps on an goomba)
            _pc.Jump();
        }
    }
}
