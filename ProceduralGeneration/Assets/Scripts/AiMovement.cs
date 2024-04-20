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
        Vector3Int goalCell = tilemap.WorldToCell(player.position);

        // Use A* or other pathfinding algorithm to calculate path
        path = AStar.FindPath(startCell, goalCell, tilemap);
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
