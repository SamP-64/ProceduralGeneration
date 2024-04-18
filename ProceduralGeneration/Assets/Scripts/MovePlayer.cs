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
        transform.position = new Vector3(startPos.x,  startPos.y, 0);
    }

    public GridManager gridManager;
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

        transform.Translate(movement);

        Vector3Int playerTilePosition = tilemap.WorldToCell(transform.position);  // restrict movement within the bounds of the Tilemap

        if (!tilemap.HasTile(playerTilePosition))    // If the player tries to move onto an empty tile, restrict movement
        {

            transform.Translate(-movement);
        }
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Collision detection
    //    Debug.Log("Collision detected with: " + collision.transform);

    //    Vector3Int cellPosition = tilemap.WorldToCell(collision.transform.position);

    //    Debug.Log(tilemap.GetTile(cellPosition).name);
    //    if (tilemap.GetTile(cellPosition).name == "Stairs")
    //    {
    //        Debug.Log("Collision detected with stairs ");
    //    }
    //}

   

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 hitPosition = collision.GetContact(0).point;
        Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);
        TileBase tile = tilemap.GetTile(cellPosition);

        if (tile != null)
        {
            Debug.Log("Collided with tile: " + tile.name);
        }
        if ( collision.gameObject.name == "StairsCollider")
        {
            Debug.Log("Next");
            gridManager.Generate();
        }
    }


}
