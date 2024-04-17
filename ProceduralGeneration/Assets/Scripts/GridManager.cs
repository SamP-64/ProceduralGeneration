
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
    [SerializeField] private int m_walkerCount = 5;
    private List<Walker> m_walkers = new List<Walker>();

    private Vector3Int origin;
    private Cell m_startCell;

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

    [SerializeField] Tile startTile;
    [SerializeField] Tile endTile;

    [SerializeField] Tile wall;
    [SerializeField] Tile empty;
    #endregion

    #region Private Functions
    private void OnEnable()
    {
        m_grid = new Grid(m_xSize, m_ySize);
        origin = new Vector3Int(Mathf.FloorToInt(m_xSize / 2), Mathf.FloorToInt(m_ySize / 2));

        for (int i = 0; i < m_walkerCount; i++)
        {
            m_walkers.Add(new Walker(m_grid.cells[(int)origin.x, (int)origin.y], maxStepCount));
        }

    }

    public void Start()
    {
        StartCoroutine(MoveTick());
    }
    IEnumerator MoveTick()
    {

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < m_walkers.Count; i++)
        {

            if (maxStepCount > m_walkers[i].stepsTaken)
            {
                m_walkers[i].Move();
            }
            else
            {
                CreateTiles();
                CheckForWalls();
                AddStartTile();
                AddEndTile();
                break;
            }

        }
        StartCoroutine(MoveTick());
    }

    private void AddStartTile()
    {
        List<float> walkerDistances = new List<float>();
        for (int i = 0; i < m_walkers.Count; i++)
        {
            walkerDistances.Add(m_walkers[i].GetDistanceFrom(origin));
        }

        int iLargest = 0;
        float largest = 0;
        for (int i = 0; i < walkerDistances.Count; i++)
        {
            if (walkerDistances[i] > largest)
            {
                largest = walkerDistances[i];
                iLargest = i;
            }
        }

        m_startCell = m_walkers[iLargest].currentCell;
        m_startCell.cellContent = startTile;
        tilemap.SetTile(origin, startTile);

      //  var a = Instantiate(m_startCell.cellContent, m_startCell.position + (Vector2.one / 2), Quaternion.identity);
      //  a.transform.parent = m_tileObjectBin.transform;

    }
    private void AddEndTile()
    {
        List<float> walkerDistances = new List<float>();
        for (int i = 0; i < m_walkers.Count; i++)
        {
            walkerDistances.Add(m_walkers[i].GetDistanceFrom(m_startCell.position));
        }

        int iLargest = 0;
        float largest = 0;
        for (int i = 0; i < walkerDistances.Count; i++)
        {
            if (walkerDistances[i] > largest)
            {
                largest = walkerDistances[i];
                iLargest = i;
            }
        }
        Cell c = m_walkers[iLargest].currentCell;
        c.cellContent = endTile;
        tilemap.SetTile(new Vector3Int(c.position.x, c.position.y, c.position.z), endTile);


        //  var a = Instantiate(m_walkers[iLargest].currentCell.cellContent, c.position + (Vector2.one / 2), Quaternion.identity);
        //  a.transform.parent = m_tileObjectBin.transform;

    }

    public void CreateRooms()
    {



        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

                if (m_grid.cells[x, y].traversed == false) { continue; }

                tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 1, 0), Middle);


                tilemap.SetTile(new Vector3Int((x * 3), (y * 3) + 2, 0), topLeft);
                tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3) + 2, 0), topRight);
                tilemap.SetTile(new Vector3Int((x * 3), (y * 3), 0), bottomLeft);
                tilemap.SetTile(new Vector3Int((x * 3) + 2, (y * 3), 0), bottomRight);


                if (m_grid.cells[x, y].GetNeighbour(Vector2.up) != null && m_grid.cells[x, y].GetNeighbour(Vector2.up) == null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.up).traversed == false)
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 2, 0), top);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3) + 2, 0), Middle);
                    }

                }


                if (m_grid.cells[x, y].GetNeighbour(Vector2.up) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.up).traversed == false)
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
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3), 0), bottom);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int((x * 3) + 1, (y * 3), 0), Middle);
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
                        tilemap.SetTile(new Vector3Int((x * 3), (y * 3) + 1, 0), left);
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

    void CreateTiles()
    {



        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

                if (m_grid.cells[x, y].traversed == false)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), empty);

                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), Middle);
                }

            }
        }


    }

    void CheckForWalls()
    {


        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

                if (m_grid.cells[x, y].traversed == true) { continue; }


                bool isSurrounded = true;
                bool leftEmpty = true;
                bool rightEmpty = true;
                bool upEmpty = true;
                bool downEmpty = true;
                bool topLeftEmpty = true;
                bool topRightEmpty = true;
                bool downLeftEmpty = true;
                bool downRightEmpty = true;


                if (m_grid.cells[x, y].GetNeighbour(Vector2.up) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.up).traversed == true)
                    {
                        isSurrounded = false;
                        upEmpty = false;
                    }
                }
                if (m_grid.cells[x, y].GetNeighbour(Vector2.down) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.down).traversed == true)
                    {
                        isSurrounded = false;
                        downEmpty = false;
                    }
                }
                if (m_grid.cells[x, y].GetNeighbour(Vector2.left) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.left).traversed == true)
                    {
                        isSurrounded = false;
                        leftEmpty = false;
                    }
                }
                if (m_grid.cells[x, y].GetNeighbour(Vector2.right) != null)
                {

                    if (m_grid.cells[x, y].GetNeighbour(Vector2.right).traversed == true)
                    {
                        isSurrounded = false;
                        rightEmpty = false;
                    }
                }



                if (isSurrounded == false)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), wall);
                }

                //if(rightEmpty && !upEmpty && !leftEmpty && !downEmpty)
                //{
                //    tilemap.SetTile(new Vector3Int(x, y, 0), right);
                //    Debug.Log("Right");
                //}

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
        for (int i = 0; i < m_walkers.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(m_walkers[i].position.x, 0, m_walkers[i].position.y), 0.5f);
        }


    }

    #endregion
}

