using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    public Image[] _hearts;
    public Sprite _fullHeart;
    public Sprite _emptyHeart;
    private Death _death;

    [Header("Health System")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("I-Frames")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set current health to max health
        currentHealth = maxHealth;
        // Update the health UI
        UpdateHealthUI();

        // Get the death script so it can be called
        _death = GetComponent<Death>();
    }

    public void TakeDamage(int damage) {
        // Stop function igf player is invincible
        if (isInvincible) return;

        // take damage based on damage value
        currentHealth -= damage;
        // clamp health so it cant go above max health or below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Call Function to update UI on health change
        UpdateHealthUI();

        // Check if the player will die
        if (currentHealth <= 0) {
            // call death function
            _death.Die();
        }
        else {
            // Call function to start i-frames
            StartCoroutine(Invincibility());
        }
    }

    void UpdateHealthUI()
    {
        // Update the health UI depending on the current health
        for (int i = 0; i < _hearts.Length; i++) {
            if (i < currentHealth) {
                _hearts[i].sprite = _fullHeart;
            }
            else {
                _hearts[i].sprite = _emptyHeart;
            }
        }
    }

    IEnumerator Invincibility() {
        // Enable i-frames
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        // disable i-frames
        isInvincible = false;
    }

}
