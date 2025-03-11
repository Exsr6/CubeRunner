using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AbilitySystem;

public class TutorialPickup : MonoBehaviour
{
    private AbilitySystem AbilitySystem;
    private TutorialManager tm;

    public AbilityType ability;

    private void Start() {
        AbilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
        tm = FindObjectOfType<TutorialManager>();
    }

    public void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            AbilitySystem.pickupAbility(ability);
            Destroy(gameObject);
            if (tm.stepIndex == 2) {
                tm.CompleteStep();
            }
            else {
                Debug.Log("That step has already been complete!");
            }
        }
    }
}
