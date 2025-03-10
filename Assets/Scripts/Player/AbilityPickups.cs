using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilitySystem;

public class AbilityPickups : MonoBehaviour
{
    private AbilitySystem AbilitySystem;

    public AbilityType ability;

    private void Start() {
        AbilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
    }

    public void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {

         if (other.gameObject.CompareTag("Player")) 
         {
             AbilitySystem.pickupAbility(ability);
             Destroy(gameObject);
         }
    }
}
