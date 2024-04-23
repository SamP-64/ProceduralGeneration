
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
    public int m_xSize;
    public int m_ySize;
    public int maxStepCount;
    public int m_walkerCount;

    private Grid m_grid;
    
    private List<Walker> m_walkers = new List<Walker>();

    public  Vector2 origin;
    private Cell m_startCell;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private MovePlayer player;
    [SerializeField] private Player playerCharacter;

    private int level;
    private int coins;

    private int roomSizeX = 3;
    private int roomSizeY = 3;

    [SerializeField] Tile empty;

    public GameObject stairsCollider;
    public GameObject enemyPrefab;
    public EnemyManager enemyManager;

    [Header("UI")]
    [SerializeField] private TMP_InputField xSizeInputField;
    [SerializeField] private TMP_InputField ySizeInputField;
    [SerializeField] private TMP_InputField walkersInputField;
    [SerializeField] private TMP_InputField maxStepsInputField;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Slider xRoomSlider;
    [SerializeField] private Slider yRoomSlider;
    [SerializeField] private TextMeshProUGUI xRoomSizeText;
    [SerializeField] private TextMeshProUGUI yRoomSizeText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameObject panel;

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

    private void ClearTilemap()
    {
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            tilemap.SetTile(position, null);
        }
    }

    public void Generate() // Generates a new grid
    {

        level++;
        levelText.text = "Level: " + level;

        enemyManager.DeleteAllEnemies();
        ClearTilemap();

        if (!GetValues()) { return;}

        m_grid = new Grid(m_xSize, m_ySize);
        origin = RandomStartPoint();

        m_walkers = null;
        m_walkers = new List<Walker>();

        Debug.Log((int)origin.x);
        Debug.Log((int)origin.y);
        for (int i = 0; i < m_walkerCount; i++)
        {
            m_walkers.Add(new Walker(m_grid.cells[(int)origin.x, (int)origin.y], maxStepCount));
        }

        //StartCoroutine(MoveTick());
        MoveTick();

    }

    private bool GetValues()
    {

        string maxSteps = maxStepsInputField.text;
        int.TryParse(maxSteps, out maxStepCount);

        string walkers = walkersInputField.text;
        int.TryParse(walkers, out m_walkerCount);


        string x = xSizeInputField.text;
        int.TryParse(x, out m_xSize);

        string y = ySizeInputField.text;
        int.TryParse(y, out m_ySize); // Get Values From Input Fields


        if ( maxStepCount < 10)
        {
            errorText.text = "Invalid max steps, enter a valid number greater than or equal to " + 10;
            return false;
        }
        else if (maxStepCount  > 5000)
        {
            errorText.text = "Invalid max steps, enter a valid number greater than or equal to " + 5000;
            return false;
        }

        if ( m_walkerCount < 1)
        {
            errorText.text = "Invalid walker count, enter a valid number greater than or equal to " + 1;
            return false;
        }
        else if (m_walkerCount > 500)
        {
            errorText.text = "Invalid walker count, enter a valid number less than or equal to " + 500;
            return false;
        }

        if ( m_xSize < 6 )
        {
            errorText.text = "Invalid x size, enter a valid number greater than or equal to " + 6;
            return false;
        }
        else if (m_xSize > 1000)
        {
            errorText.text = "Invalid x size, enter a valid number less than  " + 1000;
            return false;
        }

        if (m_ySize < 6)
        {
            errorText.text = "Invalid y size, enter a valid number greater than or equal to " + 6;
            return false;
        }
        else if (m_ySize > 1000)
        {
            errorText.text = "Invalid y size, enter a valid number less than  " + 1000;
            return false;
        }


        errorText.text = "";
        return true;
    }

    private Vector2 RandomStartPoint()
    {

        int x = Random.Range((m_xSize / 2) - (m_xSize / 4), (m_xSize / 2) + (m_xSize / 4));
        int y = Random.Range((m_ySize / 2) - (m_ySize / 4), (m_ySize / 2) + (m_ySize / 4)); // 25 to 75 percent of origin

        origin = new Vector2(x, y);
        return origin;
    }

    private void MoveTick()
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
            WalkersFinished();
        }


    }

    private void WalkersFinished()
    {
        CreateRooms();
        CreateTiles();
        CheckForWalls();
        player.StartPosition(origin);
        PlacePickup();
        AddStartTile();
        AddEndTile();
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
        SpawnTile(Mathf.RoundToInt(c.position.x), Mathf.RoundToInt(c.position.y), "EndStairs");

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

    void CheckForWalls() // Checks what type of wall to place
    {
        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {

                if (m_grid.cells[x, y].room) continue;

                SpawnTile(x, y, "Middle");

                if (m_grid.cells[x, y].traversed) continue;


                bool isSurrounded = true, 
                leftEmpty = true, rightEmpty = true, upEmpty = true, downEmpty = true;


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

    private void PlacePickup()
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
        else if (collidedTile != null && collidedTile == TileMapManager.Inst.GetTile("Ammo"))
        {
            playerCharacter.playerShoot.UpdateAmmo(5);
            SpawnTile(position.x, position.y, "Middle");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
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
        Gizmos.color = Color.red;
        for (int i = 0; i < m_walkers.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(m_walkers[i].position.x, 0, m_walkers[i].position.y), 0.5f);
        }

    }

    private bool CanCreate(ref int xStart, ref int yStart) // Checks if a room can be created in that position
    {

        if (xStart >= (m_xSize - roomSizeX))
        {
            xStart = m_xSize - roomSizeX - 1 ;
        }
        if (xStart < 1)
        {
            xStart = 1;
        }
        if (yStart >= (m_ySize - roomSizeY))
        {
            yStart = m_ySize - roomSizeY - 1;
        }
        if (yStart < 1)
        {
            yStart = 1;
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
            GenerateRandomRoom(xStart, yStart);
        }
    }

    void GenerateRandomRoom(int xStart, int yStart)
    {

        int random = Random.Range(1, 4);

        switch (random)
        {
            case 1:
                GenerateCoinRoom(xStart, yStart);
                break;
            case 2:
                GenerateEnemyRoom(xStart, yStart);
                break;
            case 3:
                GenerateAmmoRoom(xStart, yStart);
                break;
            case 4:
            case 5:
                break;
            default:
                break;
        }

    }
    void GenerateCoinRoom(int xStart, int yStart)
    {

        for (int x = xStart; x < xStart + roomSizeX ; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY ; y++)
            {

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
    void GenerateAmmoRoom(int xStart, int yStart)
    {

        for (int x = xStart; x < xStart + roomSizeX; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY; y++)
            {

                int random = Random.Range(1, 4);
                if (random == 1)
                {
                    SpawnTile(x, y, "Ammo");
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
    void GenerateEnemyRoom(int xStart, int yStart)
    {

        int middleX = xStart + roomSizeX / 2;
        int middleY = yStart + roomSizeY / 2;

        for (int x = xStart; x < xStart + roomSizeX; x++)
        {
            for (int y = yStart; y < yStart + roomSizeY ; y++)
            {
                Vector3 spawnPosition =  new Vector2(x, y);


                if (x >= middleX - 1 && x <= middleX + 1 && y >= middleY - 1 && y <= middleY + 1) // spawns 9 in the middle
                {
                     Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }

                SpawnTile(x, y, "Middle");
                m_grid.cells[x, y].traversed = true;
                m_grid.cells[x, y].room = true;


            }
        }
    }

    void SpawnTile(int x, int y, string name)
    {
        
        Tile tile = TileMapManager.Inst.GetTile(name);
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);

    }
}
