
using GridSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using static UnityEngine.Tilemaps.Tile;

public class GridManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_xSize;
    [SerializeField] private int m_ySize;
    [SerializeField] private int maxStepCount = 20;
    private Grid m_grid;
    [SerializeField] private int m_walkerCount = 5;
    private List<Walker> m_walkers = new List<Walker>();

    private Vector2 origin;
    private Cell m_startCell;


    [SerializeField] Tilemap tilemap;

    public MovePlayer player;
    int level;
    public TextMeshProUGUI levelText;

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

    public TMP_InputField xSizeInputField;
    public TMP_InputField ySizeInputField;
    public TMP_InputField walkersInputField;
    public TMP_InputField maxStepsInputField;
    #endregion

    #region Private Functions

    //OnEnable()

    public GameObject panel;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            
            panel.SetActive(!panel.activeSelf); // Toggle panel on / off
        }
    }
 
    public void Generate()
    {
        level++;
        levelText.text = "Level: " + level;
        string maxSteps = maxStepsInputField.text;
        int.TryParse(maxSteps, out maxStepCount);

        string walkers = walkersInputField.text;
        int.TryParse(walkers, out m_walkerCount);


        string x = xSizeInputField.text;
        int.TryParse(x, out m_xSize);

        string y = ySizeInputField.text;
        int.TryParse(y, out m_ySize); // Get Values From Input Fields

        m_grid = new Grid(m_xSize, m_ySize);
        origin = RandomStartPoint();

        m_walkers = null;
        m_walkers = new List<Walker>();

        for (int i = 0; i < m_walkerCount; i++)
        {
            m_walkers.Add(new Walker(m_grid.cells[(int)origin.x, (int)origin.y], maxStepCount));
        }
        //StartCoroutine(MoveTick());
        MoveTick();
    }
    Vector2  RandomStartPoint()
    {
        int x = Random.Range(1, m_xSize - 1);
        int y = Random.Range(1, m_ySize - 1);
        origin = new Vector2(x, y);
        return origin;
    }
    public Vector2 GetOrigin()
    {
        return origin;
    }

    public void Start()
    {
        player = player.GetComponent<MovePlayer>();
        // StartCoroutine(MoveTick());
        wall.colliderType = Tile.ColliderType.Grid;
    }
    void MoveTick()
    {

      //  yield return new WaitForSeconds(0.001f);
        int deadWalkers = 0;
        for (int i = 0; i < m_walkers.Count; i++)
        {
            m_walkers[i].Move(m_xSize, m_ySize);

            if (m_walkers[i].dead)
            {
                deadWalkers++;
            }
        }

        if (deadWalkers < m_walkerCount)
        {
            //  StartCoroutine(MoveTick());
            MoveTick();
        }
        else
        {
            //  m_drawWalkers = false;


            CreateTiles();
            CheckForWalls();
            player.StartPosition(origin);
            AddStartTile();
            AddEndTile();

        }


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
        tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y), 0), startTile);

        //  var a = Instantiate(m_startCell.cellContent, m_startCell.position + (Vector2.one / 2), Quaternion.identity);
        //  a.transform.parent = m_tileObjectBin.transform;

    }

    public GameObject stairsCollider;
    private void AddEndTile()
    {
     
        List<float> walkerDistances = new List<float>();
        for (int i = 0; i < m_walkers.Count; i++)
        {
            
            walkerDistances.Add(m_walkers[i].GetDistanceFrom(origin));
            Debug.Log(m_walkers[i].position + " is " + walkerDistances[i] + " from " + origin);
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
        tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(c.position.x), Mathf.RoundToInt(c.position.y), 0), endTile);
        stairsCollider.transform.position = new Vector2(c.position.x + 0.5f , c.position.y + 0.5f);

      //  Vector2 vector = new Vector2 (c.position.x, c.position.y );
      //  var a = Instantiate(m_walkers[iLargest].currentCell.cellContent, vector + (Vector2.one / 2), Quaternion.identity);
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
                    wall.colliderType = Tile.ColliderType.Grid;
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

