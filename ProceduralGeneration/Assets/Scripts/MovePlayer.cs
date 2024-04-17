using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap tilemap; // Tilemap reference

    public void StartPosition(Vector3Int startPos)
    {
        transform.position = new Vector3(startPos.x, 0.2f, startPos.y);
    }
    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = -Input.GetAxisRaw("Vertical");

        
        Vector3 movement = transform.forward * verticalInput * moveSpeed * Time.deltaTime; // Calculate movement direction based on player's local forward direction
        movement += transform.right * horizontalInput * moveSpeed * Time.deltaTime; // Add horizontal movement

        transform.Translate(movement);

       
        Vector3Int playerTilePosition = tilemap.WorldToCell(transform.position);  // restrict movement within the bounds of the Tilemap
       
        if (!tilemap.HasTile(playerTilePosition))    // If the player tries to move onto an empty tile, restrict movement
        {
         
            transform.Translate(-movement);
        }
    }
}
