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
        public Tile cellContent;
        public Color cellDebugColour;
        public bool traversed = false;
        public Room room;
        public float heat = 0;
        #endregion

        public Cell(Vector2 _pos)
        {
            position = _pos;
           // m_cellContent = "Empty";
            cellDebugColour = Color.grey;
        }


        #region Public Functions
        public Tile GetCellContent()
        {
            return cellContent;
        }
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

            if (_up == null || _right == null || _down == null || _left == null)
            {
                heat = 1;
            }
        }

        public void DebugCell()
        {
            Debug.Log("Neighbours:\n"
                        + "-Up " + m_neighbours[Vector2.up] + "\n"
                        + "-Right " + m_neighbours[Vector2.right] + "\n"
                        + "-Down " + m_neighbours[Vector2.down] + "\n"
                        + "-Left " + m_neighbours[Vector2.left] + "\n"
                    + "Position:"
                        + position + "\n"
                    + "Content:"
                        + cellContent
                    + "\n "
                    );
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
                Random.Range(0,weights[0]),
                Random.Range(0,weights[1]),
                Random.Range(0,weights[2]),
                Random.Range(0,weights[3])
            };

            int iLargest = 0;
            float largest = 0;

            for (int i = 0; i < randoms.Length; i++)
            {
                if (randoms[i] > largest)
                {
                    largest = randoms[i];
                    iLargest = i;
                }
            }

            Cell c = GetRandomNeighbour(iLargest);

            return c;
        }

        public Cell GetRandomNeighbour(int _r)
        {
            Vector2 direction = Vector2.down;
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

        public void UpdateNeighbourHeat()
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
                else if (i == 1 && down != null && down.traversed == false)
                {
                    neighboursTraversed++;
                }
                else if (i == 2 && left != null && left.traversed == false)
                {
                    neighboursTraversed++;
                }
                else if (i == 3 && right != null && right.traversed == false)
                {
                    neighboursTraversed++;
                }
            }

            heat = neighboursTraversed;
        }
    }

}