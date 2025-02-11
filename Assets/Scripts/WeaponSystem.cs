using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    public GameObject bulletPrefab; // Assign your bullet prefab in the Inspector
    public Transform firePoint; // Assign a GameObject (like an empty GameObject) at the gun's muzzle
    public float bulletSpeed = 100f;
    public float fireRate = 0.2f; // Time between shots

    private float nextFireTime = 0f;

    void Update() {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot() {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
        Destroy(bullet, 5f); // Destroy bullet after 5 seconds
    }
}
