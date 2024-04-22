using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap tilemap; // Tilemap reference
    public GridManager gridManager;

    public static Vector3 lastMovementDirection = Vector3.up; // most recent movement direction

    private void Start()
    {
        gridManager = gridManager.GetComponent<GridManager>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.up * verticalInput * moveSpeed * Time.deltaTime; // Calculate movement direction based on player's local forward direction
        movement += transform.right * horizontalInput * moveSpeed * Time.deltaTime; // Add horizontal movement

        if (movement != Vector3.zero) // Check if movement is non-zero before updating lastMovementDirection
        {
            lastMovementDirection = movement.normalized; // Update the last movement direction
        }

        transform.Translate(movement);

        Vector3Int playerTilePosition = tilemap.WorldToCell(transform.position);  // restrict movement within the bounds of the Tilemap

        if (!tilemap.HasTile(playerTilePosition))    // If the player tries to move onto an empty tile, restrict movement
        {
            transform.Translate(-movement);
        }

        gridManager.CheckPickup(playerTilePosition);
    }

    public void StartPosition(Vector2 startPos)
    {
        transform.position = new Vector2(startPos.x, startPos.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "StairsCollider")
        {
            gridManager.Generate(); // Move to next floor
        }
    }
}
