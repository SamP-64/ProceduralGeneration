using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class AStar
{
    public static List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int goalCell, Tilemap tilemap)
    {
        // Initialize lists for open and closed nodes
        List<Vector3Int> openSet = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // Dictionary to store the cameFrom values (parent nodes)
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        // Dictionary to store the cost of reaching each node from the start
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();

        // Dictionary to store the estimated total cost of reaching each node
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();

        // Add start node to open set with initial score of 0
        openSet.Add(startCell);
        gScore[startCell] = 0f;
        fScore[startCell] = Heuristic(startCell, goalCell);

        while (openSet.Count > 0)
        {
            // Get the node in the open set with the lowest fScore
            Vector3Int currentCell = GetLowestFScore(openSet, fScore);

            // If current node is goal, reconstruct and return path
            if (currentCell == goalCell)
            {
                return ReconstructPath(cameFrom, currentCell);
            }

            // Move current node from open set to closed set
            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            // Get neighbors of current node
            List<Vector3Int> neighbors = GetNeighbors(currentCell, tilemap);

            foreach (Vector3Int neighbor in neighbors)
            {
                // If neighbor is in closed set, skip it
                if (closedSet.Contains(neighbor))
                    continue;

                // Calculate tentative gScore for neighbor
                float tentativeGScore = gScore[currentCell] + 1f; // Assuming each step costs 1

                // If neighbor is not in open set, add it
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                // If tentative gScore is greater than or equal to existing gScore, skip this neighbor
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                // This path is the best one found so far, record it!
                cameFrom[neighbor] = currentCell;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalCell);

                // Introduce randomness: Occasionally choose a random neighboring cell instead of the one with the lowest fScore
                if (Random.value < 0.16f) // Adjust this probability as needed
                {
                    currentCell = neighbor;
                }
            }
        }

        // If open set is empty and goal was not found, return empty list
        return new List<Vector3Int>();
    }

    static float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    static Vector3Int GetLowestFScore(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        Vector3Int lowest = openSet[0];
        float lowestScore = fScore[lowest];
        foreach (Vector3Int cell in openSet)
        {
            if (fScore[cell] < lowestScore)
            {
                lowest = cell;
                lowestScore = fScore[cell];
            }
        }
        return lowest;
    }

    static List<Vector3Int> GetNeighbors(Vector3Int cell, Tilemap tilemap)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] directions = new Vector3Int[]
        {
           Vector3Int.up,
           Vector3Int.down,
           Vector3Int.left,
           Vector3Int.right
        };

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborCell = cell + direction;

            TileBase neighborTileBase = tilemap.GetTile(neighborCell);
            Tile neighborTile = neighborTileBase as Tile;

            if (neighborTile != null && neighborTile.colliderType == Tile.ColliderType.None)
            {
                neighbors.Add(neighborCell);  // If the tile doesn't have a collider, add it as a valid neighbor
            }
        }

        return neighbors;
    }

    static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int currentCell)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        while (cameFrom.ContainsKey(currentCell))
        {
            path.Add(currentCell);
            currentCell = cameFrom[currentCell];
        }
        path.Reverse();
        return path;
    }
}
