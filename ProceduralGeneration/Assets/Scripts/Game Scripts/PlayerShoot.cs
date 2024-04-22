using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
  
    private float shootForce = 10f;
    public float cooldown = 0.1f;
    private float lastShootTime;

    private const int maxAmmo = 50;
    private int ammo = 50;
    [SerializeField] private TextMeshProUGUI ammoText;


    public static PlayerShoot Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ammoText.text = "Ammo: " + ammo;
    }

    void Update()
    {

        if (Time.time - lastShootTime < cooldown)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Shoot(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Shoot(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Shoot(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Shoot(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(MovePlayer.lastMovementDirection);
        }
    }

    private void Shoot(Vector3 direction)
    {

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo");
            return;
        }
      
        ammo--;
        ammoText.text = "Ammo: " + ammo;

        Vector3 spawnPosition = transform.position + direction.normalized; // Change spawn position depending on direction
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * shootForce, ForceMode2D.Impulse); // Apply force in the specified direction
        }

        lastShootTime = Time.time;
    }

    public void ResetAmmo()
    {
        ammo = maxAmmo;
        ammoText.text = "Ammo: " + ammo;
    }

}
