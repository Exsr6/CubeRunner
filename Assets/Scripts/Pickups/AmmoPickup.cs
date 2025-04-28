using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private WeaponSystem _weaponSystem;

    void Start()
    {
        _weaponSystem = GameObject.Find("Player").GetComponent<WeaponSystem>();
    }

    public void Update() {

        // rotate gameobject on deltaTime
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {

        // if the player collides with the pickup add ammo and update UI
        if (other.gameObject.CompareTag("Player")) {
            _weaponSystem.currentAmmo += 10;
            _weaponSystem.UpdateAmmoUI();
            Destroy(gameObject);
        }
    }
}
