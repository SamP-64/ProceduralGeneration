using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AiMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform player;
    public Tilemap tilemap;

    private List<Vector3Int> path;
    void Update()
    {
        FindPlayer();
        MoveAlongPath();
    }

    void FindPlayer()
    {

        Vector3Int startCell = tilemap.WorldToCell(transform.position);

        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // distance between the AI and the player
        Debug.Log(distanceToPlayer);
        float maxSearchDistance = 20.0f;

        if (distanceToPlayer <= maxSearchDistance)
        {
            Vector3Int goalCell = tilemap.WorldToCell(player.position);

            path = AStar.FindPath(startCell, goalCell, tilemap);
        }
        else
        {
            path = null; // Clear the path if the player is too far away
           // Roam(startCell);
            
        }
    }
    
    void MoveAlongPath()
    {
        if (path != null && path.Count > 0)
        {
            Vector3Int nextCell = path[0];
            Vector3 nextPosition = tilemap.GetCellCenterWorld(nextCell);

            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPosition) < 0.01f)
            {
                path.RemoveAt(0);
            }
        }
    }
}
