using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap tilemap; // Tilemap reference

    private void Update()
    {
        // Input handling
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = -Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on player's local forward direction
        Vector3 movement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        movement += transform.right * horizontalInput * moveSpeed * Time.deltaTime; // Add horizontal movement

        transform.Translate(movement);

        // Optionally, restrict movement within the bounds of the Tilemap
        Vector3Int playerTilePosition = tilemap.WorldToCell(transform.position);
        if (!tilemap.HasTile(playerTilePosition))
        {
            // If the player tries to move onto an empty tile, restrict movement
            transform.Translate(-movement);
        }
    }
}
