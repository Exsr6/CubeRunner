using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Slider _Healhbar;
    public CanvasGroup _cg;

    private EndGoal _goal;


    [Header("Enemy Settings")]
    public int iHealth = 2;

    private int iMaxhealth;

    private void Start() {
        // set the max health to the intialised value
        iMaxhealth = iHealth;

        // find the endzone gameobject
        _goal = GameObject.Find("EndZone").GetComponent<EndGoal>();
    }

    void Update()
    {
        // set the slider to health on update()
        _Healhbar.value = iHealth;

        // set the alpha of the CanvasGroup based on enemy health, so it only shows if the enemy has been hurt
        if (iHealth < iMaxhealth) {
            _cg.alpha = 255f;
        }
        else {
            _cg.alpha = 0;
        }
    }

    public void TakeDamage(int damage) {

        // Enemy takes damage away from the health of the enemy
        iHealth -= damage;

        if (iHealth <= 0) {
            // make the canvas group disappear
            _cg.alpha = 0;

            // take 1 away from the kill needed
            _goal.iKillsNeeded--;

            // destroy the enemy object
            Destroy(this.gameObject);
        }
    }
}
