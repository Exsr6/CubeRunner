using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilitySystem;

public class AbilityPickups : MonoBehaviour
{
    private AbilitySystem AbilitySystem;

    public AbilityType ability;

    private void Start() {

        // find the player object in the scene and get the component
        AbilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
    }

    public void Update() {

        // rotate gameobject on deltaTime
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {

        // if the player collides with the pickup
        if (other.gameObject.CompareTag("Player")) 
        {
            // call the pickup ability function from the 
            AbilitySystem.pickupAbility(ability);

            // destroy self
            Destroy(gameObject);
        }
    }
}
