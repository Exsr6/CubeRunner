using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueTrait : MonoBehaviour
{
    [Header("References")]
    private EnemyController self;
    private PlayerController pc;

    [Header("Enemy Type")]
    public bool isSphere;
    public bool isSquare;

    private void Start() {
        self = GetComponent<EnemyController>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player") && isSphere) {
            self.TakeDamage(1);
            pc.Jump();
        }
    }
}
