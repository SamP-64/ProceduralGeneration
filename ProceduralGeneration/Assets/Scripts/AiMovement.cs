using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AiMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap tilemap;

    private List<Vector3Int> path;
    private Vector3Int lastPlayerCell;
    [SerializeField] private float updatePathInterval = 0.1f; // Update path every 1 second
    [SerializeField] private float searchRange = 20f;
    private float lastUpdateTime;

    private void Start()
    {
        lastUpdateTime = Time.time;
        lastPlayerCell = tilemap.WorldToCell(player.position);
       // Debug.Log("Starting lastUpdateTime: " + lastUpdateTime);
    }

    private void Update()
    {
        float currentTime = Time.time;
        //Debug.Log("Current time: " + currentTime);
        if (currentTime - lastUpdateTime > updatePathInterval)
        {
          //  Debug.LogWarning("Time difference: " + (currentTime - lastUpdateTime));
            FindPlayer();
            lastUpdateTime = currentTime;
        }

        MoveAlongPath();
    }

    private void FindPlayer()
    {
       
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);  // Distance between the AI and the player

        if (distanceToPlayer <= searchRange)  // Check if the player is within the search range
        {
            Vector3Int playerCell = tilemap.WorldToCell(player.position);
            if (playerCell != lastPlayerCell)
            {
                Vector3Int startCell = tilemap.WorldToCell(transform.position);
                Vector3Int goalCell = playerCell;

                path = AStar.FindPath(startCell, goalCell, tilemap);
                lastPlayerCell = playerCell;
            }
        }
        else
        {
            path = null;
        }
    }


    private void MoveAlongPath()
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


