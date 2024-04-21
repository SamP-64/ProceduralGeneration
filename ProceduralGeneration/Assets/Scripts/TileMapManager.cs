using GridSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Inst;
    public CustomTile[] AllTiles;
    public Dictionary<String, CustomTile> TileDict = new Dictionary<string, CustomTile>();
    [SerializeField] private Tilemap m_tileMap;
   
    public void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Inst != null && Inst != this)
        {
            Destroy(this);
        }
        else
        {
            Inst = this;
        }
        for (int i = 0; i < AllTiles.Length; i++)
        {
            TileDict.Add(AllTiles[i].Name, AllTiles[i]);
        }
    }

    public Tile GetTile(String _name)
    {
        CustomTile newTile = null;
        if (TileDict.ContainsKey(_name))
        {
            newTile = TileDict[_name];
        }
        else
        {
            newTile = new CustomTile();
            newTile.Name = "BackUp Tile";
        }

        return newTile.tile;


    }

}

[Serializable]
public class CustomTile
{
    public string Name;
    public Tile tile;
    public Tile.ColliderType ColliderType;
}