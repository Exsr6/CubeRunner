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
        // set the max health to the intialised value
        maxhealth = health;

        // find the endzone gameobject
        goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
    }

    void Update()
    {
        // set the slider to health on update()
        Healhbar.value = health;

        // set the alpha of the CanvasGroup based on enemy health, so it only shows if the enemy has been hurt
        if (health < maxhealth) {
            cg.alpha = 255f;
        }
        else {
            cg.alpha = 0;
        }
    }

    public void TakeDamage(int damage) {

        // Enemy takes damage away from the health of the enemy
        health -= damage;

        if (health <= 0) {
            // make the canvas group disappear
            cg.alpha = 0;

            // take 1 away from the kill needed
            goal.killsNeeded--;

            // destroy the enemy object
            Object.Destroy(this.gameObject);
        }
    }
}
