using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AbilitySystem;

public class AbilityUI : MonoBehaviour {
    public Image abilitySlot1;
    public Image abilitySlot2;

    public Sprite doubleJumpSprite;
    public Sprite dashSprite;
    public Sprite slideSprite;
    public Sprite grappleSprite;
    public Sprite emptySprite; // Default empty icon

    private AbilitySystem abilitySystem;

    private void Start() {
        abilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
        UpdateUI();
    }

    public void UpdateUI() {

        AbilityType ability1 = abilitySystem.abilityInventory[0];
        AbilityType ability2 = abilitySystem.abilityInventory[1];

        abilitySlot1.sprite = GetAbilitySprite(ability1);
        abilitySlot2.sprite = GetAbilitySprite(ability2);

        abilitySlot1.color = Color.white;
        abilitySlot2.color = Color.gray;
    }

    private Sprite GetAbilitySprite(AbilitySystem.AbilityType ability) {
        switch (ability) {
            case AbilitySystem.AbilityType.DoubleJump: return doubleJumpSprite;
            case AbilitySystem.AbilityType.Dash: return dashSprite;
            case AbilitySystem.AbilityType.Slide: return slideSprite;
            case AbilitySystem.AbilityType.Grapple: return grappleSprite;
            default: return emptySprite;
        }
    }
}
