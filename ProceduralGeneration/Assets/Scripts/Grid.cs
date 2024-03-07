using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    #region Variables
    private int m_xSize, m_ySize;
    public Cell[,] cells;

    #endregion

    public Grid(int _xsize, int _ySize)
    {
        m_xSize = _xsize;
        m_ySize = _ySize;
        cells = CreateCells();
        SetupNeighbours();

    }


    #region Private Functions

    private Cell[,] CreateCells()
    {
        Cell[,] cells = new Cell[m_xSize, m_ySize];
        for (int iX = 0; iX < m_xSize; iX++)
        {
            for (int iY = 0; iY < m_ySize; iY++)
            {
                cells[iX, iY] = new Cell(new Vector2(iX, iY));
            }
        }
        return cells;
    }

    private void SetupNeighbours()
    {
        for (int iX = 0; iX < m_xSize; iX++)
        {
            for (int iY = 0; iY < m_ySize; iY++)
            {
                Cell up = (iY + 1 < m_ySize) ? cells[iX, iY + 1] : null;
                Cell right = (iX + 1 < m_xSize) ? cells[iX + 1, iY] : null;
                Cell down = (iY - 1 >= 0) ? cells[iX, iY - 1] : null;
                Cell left = (iX - 1 >= 0) ? cells[iX - 1, iY] : null;

                cells[iX, iY].SetNeighbours(up, right, down, left);
            }
        }
    }

    #endregion

    #region Public Functions

    public void DebugGrid()
    {
        for (int iX = 0; iX < m_xSize; iX++)
        {
            for (int iY = 0; iY < m_ySize; iY++)
            {
                cells[iX, iY].DebugCell();
            }
        }

    }

    #endregion


}