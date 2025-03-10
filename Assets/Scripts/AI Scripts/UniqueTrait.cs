using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueTrait : MonoBehaviour
{
    [Header("References")]
    private EnemyController self;
    private PlayerController pc;

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
        self = GetComponent<EnemyController>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        switch(enemy)
        {
            case enemyType.Basic:

                break;

            case enemyType.Sphere:

                break;

            case enemyType.Turret:

                break;
        }
            
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player") && enemy == enemyType.Sphere) {
            self.TakeDamage(1);
            pc.Jump();
        }
    }
}
