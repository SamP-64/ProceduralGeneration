
using GridSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class GridManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_xSize;
    [SerializeField] private int m_ySize;
    [SerializeField] private int maxStepCount = 20;
    private Grid m_grid;
    [SerializeField] private int m_walkerCount;
    private List<Walker> m_walkers = new List<Walker>();

    private Vector2 origin;
    private Cell m_startCell;

    [SerializeField] private Tilemap tilemap;

    [SerializeField] private MovePlayer player;
    private int level;
    private int coins;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] Tile empty;

    [SerializeField] private  TMP_InputField xSizeInputField;
    [SerializeField] private TMP_InputField ySizeInputField;
    [SerializeField] private TMP_InputField walkersInputField;
    [SerializeField] private TMP_InputField maxStepsInputField;
  
    [SerializeField] private Slider xRoomSlider;
    [SerializeField] private Slider yRoomSlider;

    [SerializeField] private TextMeshProUGUI xRoomSizeText;
    [SerializeField] private TextMeshProUGUI yRoomSizeText;

    private int roomSizeX = 3;
    private int roomSizeY = 3;
    #endregion

    #region Private Functions


    public GameObject panel;

    public void Start()
    {
        player = player.GetComponent<MovePlayer>();

        SetSliders();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            panel.SetActive(!panel.activeSelf); // Toggle panel on / off
        }
    }

    private void SetSliders()
    {
        xRoomSlider.onValueChanged.AddListener((v) =>
        {
            xRoomSizeText.text = v.ToString("0");
            int.TryParse(xRoomSizeText.text, out roomSizeX);
        });
        yRoomSlider.onValueChanged.AddListener((v) =>
        {
            yRoomSizeText.text = v.ToString("0");
            int.TryParse(yRoomSizeText.text, out roomSizeY);
        });
    }
    public void PlayerDies()
    {
        level = 1;
        levelText.text = "Level: " + level;
        coins = 0;
        coinText.text = "Coins: " + coins;
        PlayerShoot.Instance.ResetAmmo();
    }

    public void Generate()
    {

        level++;
        levelText.text = "Level: " + level;

        enemyManager.DeleteAllEnemies();
        StoreInputValues();

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
    private void StoreInputValues()
    {
        string maxSteps = maxStepsInputField.text;
        int.TryParse(maxSteps, out maxStepCount);

        string walkers = walkersInputField.text;
        int.TryParse(walkers, out m_walkerCount);


        string x = xSizeInputField.text;
        int.TryParse(x, out m_xSize);

        string y = ySizeInputField.text;
        int.TryParse(y, out m_ySize); // Get Values From Input Fields
    }
    private Vector2 RandomStartPoint()
    {
        int x = Random.Range(1 + (m_xSize / 2), m_xSize - (m_xSize / 2));
        int y = Random.Range(1 + (m_ySize / 2), m_ySize - (m_ySize / 2));
        origin = new Vector2(x, y);
        return origin;
    }

    void MoveTick()
    {

        //   yield return new WaitForSeconds(0.1f);
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

          
            CreateRooms();
            CreateTiles();
            CheckForWalls();

            player.StartPosition(origin);

            // CreateRoom(14,14);
            PlacePickup();

            AddStartTile();
            AddEndTile();

           // GenerateCoinRoom(10, 10, 6);
        }


    }

    private void CreateRooms()
    {
        foreach (Walker walker in m_walkers)
        {
            CreateRoom((int)walker.position.x, (int)walker.position.y);
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
        SpawnTile(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y), "StartStairs");

    }

    public GameObject stairsCollider;

    private void AddEndTile()
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
        Cell c = m_walkers[iLargest].currentCell;
        SpawnTile(Mathf.RoundToInt(c.position.x), Mathf.RoundToInt(c.position.y), "StartStairs");

        stairsCollider.transform.position = new Vector2(c.position.x + 0.5f, c.position.y + 0.5f);
        Instantiate(enemyPrefab, stairsCollider.transform.position, Quaternion.identity);

    }


    void CreateTiles()
    {


        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {


                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    continue;
                }

                if (m_grid.cells[x, y].traversed == false)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), empty);

                }
                else
                {
                    SpawnTile(x, y, "Middle");
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

                if (m_grid.cells[x, y].room) continue;

                SpawnTile(x, y, "Middle");

                if (m_grid.cells[x, y].traversed) continue;


                bool isSurrounded = true;
                bool leftEmpty = true;
                bool rightEmpty = true;
                bool upEmpty = true;
                bool downEmpty = true;

                // Check horizontal and vertical neighbors
                if (m_grid.cells[x, y].GetNeighbour(Vector2.up) != null)
                {
                    if (!m_grid.cells[x, y].GetNeighbour(Vector2.up).traversed)
                    {
                        isSurrounded = false;
                        upEmpty = false;
                    }
                }
                else
                {
                    isSurrounded = false;
                    upEmpty = false;
                }

                if (m_grid.cells[x, y].GetNeighbour(Vector2.down) != null)
                {
                    if (!m_grid.cells[x, y].GetNeighbour(Vector2.down).traversed)
                    {
                        isSurrounded = false;
                        downEmpty = false;
                    }
                }
                else
                {
                    isSurrounded = false;
                    downEmpty = false;
                }
                if (m_grid.cells[x, y].GetNeighbour(Vector2.left) != null)
                {
                    if (!m_grid.cells[x, y].GetNeighbour(Vector2.left).traversed)
                    {
                        isSurrounded = false;
                        leftEmpty = false;

                    }
                }
                else
                {
                    isSurrounded = false;
                    leftEmpty = false;
                }

                if (m_grid.cells[x, y].GetNeighbour(Vector2.right) != null)
                {
                    if (!m_grid.cells[x, y].GetNeighbour(Vector2.right).traversed)
                    {
                        isSurrounded = false;
                        rightEmpty = false;
                    }
                }
                else
                {
                    isSurrounded = false;
                    rightEmpty = false;
                }

                Matrix4x4 defaultMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
                tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), defaultMatrix);



                if (!isSurrounded)
                {
                    SpawnTile(x, y, "Wall");
                    m_grid.cells[x, y].wall = true;
                }

                if (upEmpty && !downEmpty && !leftEmpty && !rightEmpty)
                {
                    SpawnTile(x, y, "Up");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && downEmpty && !leftEmpty && !rightEmpty)
                {
                    SpawnTile(x, y, "Down");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && !downEmpty && leftEmpty && !rightEmpty)
                {
                    SpawnTile(x, y, "Left");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && !downEmpty && !leftEmpty && rightEmpty)
                {
                    SpawnTile(x, y, "Right");
                    m_grid.cells[x, y].wall = true;
                }
                //// Conditions for diagonal tiles
                else if (upEmpty && !downEmpty && rightEmpty && !leftEmpty)
                {
                    SpawnTile(x, y, "UpRight");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && downEmpty && !rightEmpty && leftEmpty)
                {
                    SpawnTile(x, y, "DownLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (upEmpty && !downEmpty && !rightEmpty && leftEmpty)
                {
                    SpawnTile(x, y, "UpLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && downEmpty && rightEmpty && !leftEmpty)
                {
                    SpawnTile(x, y, "DownRight");
                    m_grid.cells[x, y].wall = true;
                }

                else if (upEmpty && downEmpty && !rightEmpty && leftEmpty)
                {
                    SpawnTile(x, y, "UpDownLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && downEmpty && rightEmpty && leftEmpty)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one);
                    tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
                    SpawnTile(x, y, "UpDownLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (upEmpty && downEmpty && rightEmpty && !leftEmpty)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
                    tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
                    SpawnTile(x, y, "UpDownLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (upEmpty && !downEmpty && rightEmpty && leftEmpty)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 270), Vector3.one);
                    tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
                    SpawnTile(x, y, "UpDownLeft");
                    m_grid.cells[x, y].wall = true;
                }
                else if (upEmpty && downEmpty && !rightEmpty && !leftEmpty)
                {
                    SpawnTile(x, y, "UpDown");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!upEmpty && !downEmpty && rightEmpty && leftEmpty)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one);
                    tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
                    SpawnTile(x, y, "UpDown");
                    m_grid.cells[x, y].wall = true;
                }
                else if (!rightEmpty && !upEmpty && !leftEmpty && !downEmpty)
                {
                    SpawnTile(x, y, "Middle"); // Out of bounds
                }
                else
                {
                    SpawnTile(x, y, "Wall");
                    m_grid.cells[x, y].wall = true;
                }
            }
        }

    }

    void PlacePickup()
    {

        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

                if (m_grid.cells[x, y].traversed == true)
                {

                    if (Random.Range(0, (m_xSize * m_ySize) / 7) == 0)
                    {

                        SpawnTile(x, y, "Coin");
                    }

                }

            }
        }
    }
    public void CheckPickup(Vector3Int position)
    {

        TileBase collidedTile = tilemap.GetTile(position);    // Get the tile at the collided position

        if (collidedTile != null && collidedTile == TileMapManager.Inst.GetTile("Coin"))
        {
            SpawnTile(position.x, position.y, "Middle");   // Replace the coin tile with the floor tile
            coins++;
            coinText.text = "Coins: " + coins;
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

    bool CanCreate(ref int xStart, ref int yStart)
    {

        Debug.Log(m_xSize);

        if (xStart >= (m_xSize - roomSizeX))
        {
            xStart = m_xSize - roomSizeX - 5;
        }

        if (xStart < 2)
        {
            xStart = 2;
        }
        if (yStart >= (m_ySize - roomSizeY))
        {
            yStart = m_ySize - roomSizeY - 5;
        }
        if (yStart < 2)
        {
            yStart = 2;
        }

        for (int x = xStart; x < xStart + roomSizeX; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY; y++)
            {

                if ((m_grid.cells[x, y] == null || m_grid.cells[x, y].room == true))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void CreateRoom(int xStart, int yStart)
    {

        bool canCreate = CanCreate(ref xStart, ref yStart);


        if (canCreate)
        {
            Debug.Log("Creating Room at " + xStart + " " + yStart);
            GenerateRandomRoom(xStart, yStart);

        }
        else
        {
            Debug.Log("Cannot create at " + xStart + " " + yStart);
        }
    }

    void GenerateRandomRoom(int xStart, int yStart)
    {

        int random = Random.Range(1, 3);

        switch (random)
        {
            case 1:
                GenerateCoinRoom(xStart, yStart);
                break;
            case 2:
                GenerateEnemyRoom(xStart, yStart);
                break;
            case 3:
            case 4:
            case 5:
                break;
            default:
                break;
        }

    }
    void GenerateCoinRoom(int xStart, int yStart)
    {

        Debug.Log("Generating Coin Room");
        for (int x = xStart; x < xStart + roomSizeX ; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY ; y++)
            {
                //  Debug.Log(x + " " + y);

                int random = Random.Range(1, 3);
                if (random == 1)
                {
                    SpawnTile(x, y, "Coin");
                }
                else
                {
                    SpawnTile(x, y, "Middle");
                }
                m_grid.cells[x, y].traversed = true;
                m_grid.cells[x, y].room = true;

            }
        }
    }


    public GameObject enemyPrefab;
    public EnemyManager enemyManager;

    void GenerateEnemyRoom(int xStart, int yStart)
    {

        int middleX = xStart + roomSizeX / 2;
        int middleY = yStart + roomSizeY / 2;

        Debug.Log("Generating Enemy Room");
        for (int x = xStart; x < xStart + roomSizeX; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY ; y++)
            {
                Vector3 spawnPosition =  new Vector2(x, y);


                if (x >= middleX - 1 && x <= middleX + 1 && y >= middleY - 1 && y <= middleY + 1) // spawns 9 in the middle
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }

                SpawnTile(x, y, "Middle");
                m_grid.cells[x, y].traversed = true;
                m_grid.cells[x, y].room = true;


            }
        }
        #endregion
    }

    void SpawnTile(int x, int y, string name)
    {
        
        Tile tile = TileMapManager.Inst.GetTile(name);
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);

    }
}
