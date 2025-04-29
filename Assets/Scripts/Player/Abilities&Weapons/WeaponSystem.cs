using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public AudioSource FireSound;
    public TextMeshProUGUI ammoText;

    [Header("Variables")]
    public float bulletSpeed = 100f;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("Ammo System")]
    public int maxAmmo = 10;
    public int currentAmmo;

    private void Start() {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update() {
        // call the shoot function when the player presses the fire button
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) {
            Shoot();
            // fire rate
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot() {

        if (currentAmmo > 0) {
            // Create the bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // play the fire sound
            FireSound.Play();

            // get the component
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null) {
                // apply velocity
                rb.velocity = firePoint.forward * bulletSpeed;
            }

            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

            // Destroy bullet after 5 seconds
            Destroy(bullet, 5f);

            // Reduce ammo
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    public void UpdateAmmoUI() {
        ammoText.text = "Ammo: " + currentAmmo;
    }
}
