using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class AStar // Found Online to use
{
    public static List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int goalCell, Tilemap tilemap)
    {

        List<Vector3Int> openSet = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();
        openSet.Add(startCell);
        gScore[startCell] = 0f;
        fScore[startCell] = Heuristic(startCell, goalCell);

        while (openSet.Count > 0)
        {
            Vector3Int currentCell = GetLowestFScore(openSet, fScore);
            if (currentCell == goalCell)
            {
                return ReconstructPath(cameFrom, currentCell);
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            List<Vector3Int> neighbors = GetNeighbors(currentCell, tilemap);

            foreach (Vector3Int neighbor in neighbors)
            {

                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[currentCell] + 1f; 

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = currentCell;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalCell);

                if (Random.value < 0.16f) 
                {
                    currentCell = neighbor;
                }
            }
        }

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
