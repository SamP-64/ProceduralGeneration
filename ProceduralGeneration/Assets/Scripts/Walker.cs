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
          //  gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        //    gridManager.CreateRoom((int)position.x, (int)position.y, 6);
            return;
        }
       
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
            else
            {
                
                //Cell neighbour2 = currentCell.GetRandomNeighbour(Random.Range(0,5));
                //x = neighbour2.position.x;
                //y = neighbour2.position.y; // Values to stop walkers moving to the edge of the grid


                //if (neighbour != null && x != 0 && y != 0 && y != gridSizeY - 1 && x != gridSizeX - 1)
                //{
                //    currentCell = neighbour2;
                //    currentCell.cellDebugColour = Color.black;
                //    currentCell.traversed = true;
                //    position = currentCell.position;
                //    currentCell.UpdateNeighbourHeat();
                //}
            }

        }
   
     
        stepsTaken++;

        //if (ShouldCreateRoom(gridSizeX, gridSizeY))
        //{
        //    CreateRoom();
        //}

    }

    private bool ShouldCreateRoom(int gridSizeX, int gridSizeY)
    {

        // Example condition: create a room every 10 steps
        return stepsTaken % 10 == 0;
    }

    public List<Room> createdRooms = new List<Room>();


    public GridManager gridManager;
    private void CreateRoom()
    {
     
        Room room = new Room(position, new Vector2(5, 5)); // Example room size: 5x5
      //  gridManager.CreateRoom();
        createdRooms.Add(room);
    }




}