using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour {
    public Slider bossHealthbar;
    public BossAI boss;

    private void Start() {
        // Initialize the health bar with the boss's max health
        bossHealthbar.maxValue = boss.iMaxHealth;
        bossHealthbar.value = boss.iCurrentHealth;
    }

    void Update() {
         bossHealthbar.value = Mathf.Lerp(bossHealthbar.value, boss.iCurrentHealth, Time.deltaTime * 10f);
    }
}
