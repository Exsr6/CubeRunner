using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilitySystem;

public class AbilityPickups : MonoBehaviour
{
    private AbilitySystem AbilitySystem;

    public AbilityType ability;
    public enum AbilityType {
        None,
        doubleJump,
        dash,
        slide,
        Grapple
    }

    private void Start() {
        AbilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
    }

    public void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Player")
        {
            switch (ability) {
                case AbilityType.None:
                    break;

                case AbilityType.doubleJump:
                    AbilitySystem.hasDoubleJump = true;
                    AbilitySystem.cAbility = currentAbility.Double;
                    break;

                case AbilityType.dash:
                    AbilitySystem.hasDash = true;
                    AbilitySystem.cAbility = currentAbility.Dash;
                    break;

                case AbilityType.slide:
                    AbilitySystem.hasSlide = true;
                    AbilitySystem.cAbility = currentAbility.Slide;
                    break;

                case AbilityType.Grapple:
                    AbilitySystem.hasGrapple = true;
                    AbilitySystem.cAbility = currentAbility.Grapple;
                    break;
            }

            Destroy(gameObject);
        }
    }
}
