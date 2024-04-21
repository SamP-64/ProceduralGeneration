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

    private int ammo = 50;
    [SerializeField] private TextMeshProUGUI ammoText;

  
    void Update()
    {

        if (Time.time - lastShootTime < cooldown)
            return;

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

    void Shoot(Vector3 direction)
    {

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }
      
        ammo--;
        ammoText.text = "Ammo: " + ammo;

        GameObject projectile = Instantiate(projectilePrefab, transform.position + direction.normalized * 2, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Apply force in the specified direction
            rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
        }

        // Update last shoot time
        lastShootTime = Time.time;
    }
}
