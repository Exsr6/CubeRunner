using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilitySystem;

public class AbilityPickups : MonoBehaviour
{
    [Header("References")]
    private AbilitySystem _AbilitySystem;
    private AudioSource _AbilityPickupSound;

    [Header("Variables")]
    public float fRespawnTime = 5f;
    private Vector3 vSpawnPosition;

    public AbilityType ability;

    private void Start() {

        // find the player object in the scene and get the component
        _AbilitySystem = GameObject.Find("Player").GetComponent<AbilitySystem>();
        _AbilityPickupSound = GetComponent<AudioSource>();

        vSpawnPosition = transform.position;
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
            _AbilitySystem.pickupAbility(ability);

            _AbilityPickupSound.Play();

            // destroy self
            gameObject.SetActive(false);
            if (IsSandboxScene()) {
                Invoke(nameof(Respawn), fRespawnTime);
            }
        }
    }
    void Respawn() {
        gameObject.SetActive(true);
        transform.position = vSpawnPosition;
    }

    bool IsSandboxScene() {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Sandbox";
    }
}
