using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Dune2;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

public class mapManager : MonoBehaviour {

    private Vector2Int mapSize;
    private Vector2Int tileSize;
    private Vector2 cellSize;
    private SuperMap superMap;
    private unitManager units;
    private buildingsManager buildings;
    [NonSerialized] public GameObject mapTile;
    private Dictionary<TileBase, tileData> dataFromTiles;

    [SerializeField] private Tilemap map;

    [SerializeField] private List<tileData> tileDatas;

    private void Awake() {
        LoadMapOld();
    }

    public void Update() {
        units.Update(Time.deltaTime);
        buildings.Update(Time.deltaTime);
    }
    public void LoadMapOld() {

        units = new unitManager();
        dataFromTiles = new Dictionary<TileBase, tileData>();
        foreach (var td in tileDatas)
        {
            foreach (var t in td.tiles)
            {
                dataFromTiles.Add(t, td);
            }
        }
        mapTile = GameObject.Find("map");
        mapSize = new Vector2Int();
        var grid = mapTile.GetComponent<Grid>();
        cellSize.x = grid.cellSize.x * 32;
        cellSize.y = grid.cellSize.y * 32;
        var tm = mapTile.GetComponentInChildren<Tilemap>();
        mapSize.x = tm.size.x;
        mapSize.y = tm.size.y;
        astar.GetInstance().Init(mapSize.x,mapSize.y);
        int[][] weights = new int[mapSize.x][];
        for (int x = 0; x < mapSize.x; x++)
        {

            weights[x] = new int[mapSize.y];
            for (int y = 0; y < mapSize.y; y++)
            {
                //TileBase t = tm.GetTile(new Vector3Int(x, y, 0));
                //t.GetTileData(new Vector3Int(x,y,0));
                //Debug.Log(t.name);
                int w = 1;
                //int w = dataFromTiles[t].speed;
                weights[x][y] = w;
            }
        }
        astar.GetInstance().FillMap(weights);
        units.createUnit(eUnitType.kTrike, 1,1, 1);
        units.createUnit(eUnitType.kTrike, 2,5, 5);
        buildings = new buildingsManager();
        buildings.Init();
        buildings.Build(4, 2, eBuildingType.kBase, 2);
    }
    public void LoadMap(String name) {
        mapTile = GameObject.Find(name);
        //Date = System.DateTime.Now.TimeOfDay.ToString();
        units = new unitManager();
        superMap = mapTile.GetComponent<SuperMap>();
        mapSize = new Vector2Int();
        tileSize = new Vector2Int();
        mapSize.x = superMap.m_Width;
        mapSize.y = superMap.m_Height;
        tileSize.x = superMap.m_TileWidth * 2;
        tileSize.y = superMap.m_TileHeight * 2;
        Grid gr = superMap.GetComponentInChildren<Grid>();
        cellSize.x = gr.cellSize.x * 2;
        cellSize.y = gr.cellSize.y * 2;
        var layer = mapTile.GetComponentInChildren<SuperTileLayer>();
        var tm = layer.GetComponent<Tilemap>();
        
        
        int[][] weights = new int[mapSize.x][];
        for (int x = 0; x < mapSize.x; x++)
        {

            weights[x] = new int[mapSize.y];
            for (int y = 0; y < mapSize.y; y++) {
                TileBase t = tm.GetTile(new Vector3Int(x, y, 0));
                //t.GetTileData(new Vector3Int(x,y,0));
                Debug.Log(t.name);
                int w = 1;
                //int w = dataFromTiles[t].speed;
                weights[x][y] = w;
            }
        }
        //a.FillMap(weights);
        //SuperTile t = superMap.m_BackgroundColor

    }

    public Vector2Int GetMapSize()
    {
        return mapSize;
    }

    public Vector2Int GetTileSize()
    {
        return tileSize;
    }
    public Vector2 GetCellSize()
    {
        return cellSize;
    }
    
    public unitManager GetUnitManager()
    {
        if (units == null) {
            units = new unitManager();
        }
        return units;
    }

    public SuperMap getMap()
    {
        return superMap;
    }
}
