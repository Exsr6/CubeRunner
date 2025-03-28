using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Variables")]
    public float bulletSpeed = 100f;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    void Update() {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) {
            Shoot();
            // fire rate
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot() {
        // Create the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // get the component
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) {
            // apply velocity
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        // Destroy bullet after 5 seconds
        Destroy(bullet, 5f);
    }
}
