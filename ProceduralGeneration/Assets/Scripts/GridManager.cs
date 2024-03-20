using GridSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_xSize;
    [SerializeField] private int m_ySize;
    [SerializeField] private int maxStepCount = 20;
    private Grid m_grid;
    private Walker[] m_walkers = new Walker[4];

    [SerializeField] Tilemap tilemap;
    
    
    [SerializeField] Tile Middle;
    [SerializeField] Tile topLeft;
    [SerializeField] Tile topRight;
    [SerializeField] Tile bottomLeft;
    [SerializeField] Tile bottomRight;
    [SerializeField] Tile left;
    [SerializeField] Tile right;
    [SerializeField] Tile top;
    [SerializeField] Tile bottom;
    #endregion

    #region Private Functions
    private void OnEnable()
    {
        m_grid = new Grid(m_xSize, m_ySize);
        for (int i = 0; i < m_walkers.Length; i++)
        {
            m_walkers[i] = new Walker(m_grid.cells[Mathf.FloorToInt(m_xSize / 2), Mathf.FloorToInt(m_ySize / 2)]);
        }
    }

    public void Start()
    {
        StartCoroutine(MoveTick());
    }
    IEnumerator MoveTick()
    {

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < m_walkers.Length; i++)
        {
        
            if(maxStepCount > m_walkers[i].stepsTaken )
            {
                m_walkers[i].Move();
            }
            else
            {
                CreateRooms();
            }
           
        }
        StartCoroutine(MoveTick());
    }

    public void CreateRooms()
    {
     
        
        
        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

               if (m_grid.cells[x, y].traversed == false) { continue ; }

                tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 1, 0), Middle);
                tilemap.SetTile(new Vector3Int((x * 3) , (y * 3) + 2, 0), topLeft);
                tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) + 2, 0), topRight);
                tilemap.SetTile(new Vector3Int((x * 3) , (y * 3) , 0), bottomLeft);
                tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) , 0), bottomRight);



                if (m_grid.cells[x, y].GetNeighbour(Vector2.up) != null)
                {
                 
                    if(m_grid.cells[x, y].GetNeighbour(Vector2.up).traversed == false)
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 2, 0), top);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 2, 0), Middle);
                    }
                  
                }
                else
                {
                    tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 2, 0), top);
                }

                if (m_grid.cells[x, y].GetNeighbour(Vector2.down) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.down).traversed == false)
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) , 0), bottom);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) , 0), Middle);
                    }

                }
                else
                {
                    tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3), 0), bottom);
                }

                if (m_grid.cells[x, y].GetNeighbour(Vector2.right) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.right).traversed == false)
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) + 1, 0), right);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) + 1, 0), Middle);
                    }

                }
                else
                {
                    tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) + 1, 0), right);
                }

                if (m_grid.cells[x, y].GetNeighbour(Vector2.left) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.left).traversed == false)
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) , (y * 3) + 1, 0), left);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3), (y * 3) + 1, 0), Middle);
                    }

                }
                else
                {
                    tilemap.SetTile(new Vector3Int((x * 3), (y * 3) + 1, 0), left);
                }

            }
        }
 
    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (m_grid == null)
        {
            return;
        }
        for (int iX = 0; iX < m_xSize; iX++)
        {
            for (int iY = 0; iY < m_ySize; iY++)
            {
                Gizmos.color = m_grid.cells[iX, iY].cellDebugColour;
                Gizmos.DrawCube(new Vector3(iX, 0, iY), Vector3.one / 2);
            }
        }
        Gizmos.color = Color.blue;
        for (int i = 0; i < m_walkers.Length; i++)
        {
            Gizmos.DrawSphere(new Vector3(m_walkers[i].position.x, 0, m_walkers[i].position.y), 0.5f);
        }


    }

    #endregion
}