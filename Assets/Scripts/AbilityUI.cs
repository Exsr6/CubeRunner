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
    public Sprite emptySprite;

    private AbilitySystem abilitySystem;

    private void Start() {

        // find and get the ability system
        abilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();

        // update the ui
        UpdateUI();
    }

    public void UpdateUI() {

        // set ability type to the inventory slot
        AbilityType ability1 = abilitySystem.abilityInventory[0];
        AbilityType ability2 = abilitySystem.abilityInventory[1];

        // get sprites
        abilitySlot1.sprite = GetAbilitySprite(ability1);
        abilitySlot2.sprite = GetAbilitySprite(ability2);

        // set colours
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
