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
        currentHealth = maxHealth;
        UpdateHealthUI();

        _death = GetComponent<Death>();
    }

    public void TakeDamage(int damage) {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0) {
            _death.Die();
        }
        else {
            StartCoroutine(Invincibility());
        }
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < _hearts.Length; i++) {
            if (i < currentHealth) {
                _hearts[i].sprite = _fullHeart; // Red heart
            }
            else {
                _hearts[i].sprite = _emptyHeart; // Grey heart
            }
        }
    }

    IEnumerator Invincibility() {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

}
