using GridSystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Walker
{
    public Vector2 position;
    public Cell currentCell;
    public int stepsTaken;
    int maxSteps;
    public bool dead;
    public GridManager gridManager;
    
    public Walker(Cell _cell, int _maxSteps) // Walker Constructor
    {
        currentCell = _cell;
        position = _cell.position;
        maxSteps = _maxSteps;
    }

    public float GetDistanceFrom(Vector2 _from) // Returns walker distance from certain point
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

        if (currentCell != null )
        {
            Cell neighbour = currentCell.GetRandomWeightedNeighbour();
            float x = neighbour.position.x;
            float y = neighbour.position.y; // Values to stop walkers moving to the edge of the grid

            if (neighbour != null && x != 0 && y != 0 && y != gridSizeY - 1 && x != gridSizeX - 1)
            {
                currentCell = neighbour;
                currentCell.cellDebugColour = Color.cyan;
                currentCell.traversed = true;
                position = currentCell.position;
                currentCell.UpdateHeat();
            }

        }
   
        stepsTaken++;
    }
}