using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Slider Healhbar;
    public CanvasGroup cg;
    private EndGoal goal;


    [Header("Enemy Settings")]
    private int maxhealth;
    public int health = 2;

    private void Start() {
        maxhealth = health;
        goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
    }

    // Update is called once per frame
    void Update()
    {
        Healhbar.value = health;

        if (health < maxhealth) {
            cg.alpha = 255f;
        }
        else {
            cg.alpha = 0;
        }
    }

    public void TakeDamage(int damage) {

        health -= damage;

        if (health <= 0) {
            cg.alpha = 0;
            goal.killsNeeded--;
            Object.Destroy(this.gameObject);
        }
    }
}
