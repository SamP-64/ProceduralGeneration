using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSystem
{
    public class Cell
    {
        #region Variables
        public Vector2 position;
        private Dictionary<Vector2, Cell> m_neighbours = new Dictionary<Vector2, Cell>()
        {
            {Vector2.up, null},
            {Vector2.right, null},
            {Vector2.down, null},
            {Vector2.left, null}
        };

        public Color cellDebugColour;
        public bool traversed = false;
        public bool room = false;
        public bool wall = false;
        public float heat = 1;
        #endregion

        public Cell(Vector2 _pos)
        {
            position = _pos;
           // m_cellContent = "Empty";
            cellDebugColour = Color.grey;
        }


        #region Public Functions
      
        public Cell GetNeighbour(Vector2 Direction)
        {
            return m_neighbours[Direction];
        }

        public void SetNeighbours(Cell _up, Cell _right, Cell _down, Cell _left)
        {
            m_neighbours[Vector2.up] = _up;
            m_neighbours[Vector2.right] = _right;
            m_neighbours[Vector2.down] = _down;
            m_neighbours[Vector2.left] = _left;
        }

        #endregion Private Functions

        public Cell GetRandomWeightedNeighbour()
        {
            float[] weights = new float[4] { 0, 0, 0, 0 };
            int n = 0;
            foreach (KeyValuePair<Vector2, Cell> entry in m_neighbours)
            {
                if (entry.Value != null)
                {
                    weights[n] = 1 - entry.Value.heat;
                }
                n++;
            }
            float[] randoms = new float[4]
            {
                Random.Range(0,weights[0] ),
                Random.Range(0,weights[1] ),
                Random.Range(0,weights[2] ),
                Random.Range(0,weights[3] )
            };

            int iLargest = 0;

            List<int> highestNumbers = new List<int>();
            float maxRandomValue = float.MinValue;

            for (int i = 0; i < randoms.Length; i++)
            {
                if (randoms[i] > maxRandomValue)
                {
                    maxRandomValue = randoms[i];
                    highestNumbers.Clear(); 
                    highestNumbers.Add(i); 
                }
                else if (randoms[i] == maxRandomValue)
                {
                    highestNumbers.Add(i); 
                }
            }

            iLargest = highestNumbers[Random.Range(0, highestNumbers.Count)]; // List to stop ties causing problems

            Cell c = GetRandomNeighbour(iLargest);

            return c;
        }

        public Cell GetRandomNeighbour(int _r)
        {
            Vector2 direction = Vector2.zero;
            switch (_r)
            {
                case 0:
                    direction = Vector2.up;
                    break;
                case 1:
                    direction = Vector2.right;
                    break;
                case 2:
                    direction = Vector2.down;
                    break;
                case 3:
                    direction = Vector2.left;
                    break;

            }
            return m_neighbours[direction];
        }
     
        public void UpdateHeat()
        {

            Cell up = GetNeighbour(Vector2.up);
            Cell down = GetNeighbour(Vector2.down);
            Cell left = GetNeighbour(Vector2.left);
            Cell right = GetNeighbour(Vector2.right);


            int neighboursTraversed = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == 0 && up != null && up.traversed == false)
                {
                    neighboursTraversed++;
                }
                if (i == 1 && down != null && down.traversed == false)
                {
                    neighboursTraversed++;
                }
                if (i == 2 && left != null && left.traversed == false)
                {
                    neighboursTraversed++;
                }
                 if (i == 3 && right != null && right.traversed == false)
                {
                    neighboursTraversed++;
                }
            }

            heat = 4 - neighboursTraversed;
        }
    }

}