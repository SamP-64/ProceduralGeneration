using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private string m_cellContent;
        public Color cellDebugColour;
        #endregion

        public Cell(Vector2 _pos)
        {
            position = _pos;
            m_cellContent = "Empty";
            cellDebugColour = Color.grey;
        }

        #region Private Functions

        #endregion

        #region Public Functions
        public string GetCellContent()
        {
            return m_cellContent;
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
                        + m_cellContent
                    + "\n "
                    );
        }
        #endregion Private Functions



    }

}