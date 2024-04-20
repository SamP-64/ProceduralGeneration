using GridSystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Walker
{
    public Vector2 position;
    public Cell currentCell;
    private Vector2[] m_directions = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    public int stepsTaken;
    int maxSteps;
    public bool dead;

    public Walker(Cell _cell, int _maxSteps)
    {
        currentCell = _cell;
        position = _cell.position;
        maxSteps = _maxSteps;
    }

    public float GetDistanceFrom(Vector2 _from)
    {
        float dist = Vector2.Distance(_from, position);
        return dist;
    }

    public void Move(int gridSizeX, int gridSizeY)
    {

        if (stepsTaken >= maxSteps)
        {
            dead = true;
            return;
        }

        Vector2 selectedDirection = m_directions[Random.Range(0, m_directions.Length)];
       
        float x = 0;
        float y = 0;

        

        if (currentCell != null )
        {
            Cell neighbour = currentCell.GetRandomWeightedNeighbour();
            x = neighbour.position.x;
            y = neighbour.position.y; // Values to stop walkers moving to the edge of the grid

            if (neighbour != null && x != 0 && y != 0 && y != gridSizeY - 1 && x != gridSizeX - 1)
            {
                currentCell = neighbour;
                currentCell.cellDebugColour = Color.black;
                currentCell.traversed = true;
                position = currentCell.position;
                currentCell.UpdateNeighbourHeat();
            }

        }
   
     
        stepsTaken++;
    }


}