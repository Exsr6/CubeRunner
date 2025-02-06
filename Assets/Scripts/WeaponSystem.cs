using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    [Header("Properties")]
    public float fireRate;
    public float damage;
    public float bulletDistance;

    public int magazineSize;
    private int bulletsRemaining;

    bool isAutomatic;

    [Header("Keybinds")]
    public KeyCode fireKey = KeyCode.Mouse0;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform cameraPosition;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    private void Fire()
    {
        
    }
}
