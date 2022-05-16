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
    private Camera camera;
    private unit selectedUnit;
    private building selectedBuilding;
    private const float cameraDistance = 0.64f;
    private int myPlayer = 1;

    private void Awake() {
        LoadMapOld();
    }

    public void Update() {
        checkInput();
        units.Update(Time.deltaTime);
        buildings.Update(Time.deltaTime);
    }

    void checkInput() {

        if (Input.GetMouseButtonDown(0))
        {
            if (!camera)
            {
                camera = gameManager.GetInstance().GetCamera();
            }

            if (selectedUnit != null) {
                selectedUnit.Unselect();
            }

            if (selectedBuilding != null) {
                selectedBuilding.Unselect();
            }
            selectedUnit = null;
            selectedBuilding = null;
            var targetPosition =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));

            selectedUnit = units.GetUnitAt(targetPosition);
            if (selectedUnit!=null) {
                if(selectedUnit.GetPlayer() != myPlayer)
                    selectedUnit = null;
                else {
                    selectedUnit.Select();
                }
            }
            else {
                selectedBuilding = buildings.GetBuildAt(targetPosition);
                if (selectedBuilding != null) {
                    if (selectedBuilding.CheckPlayer(myPlayer)) {
                        selectedBuilding.Select();
                    }
                    else {
                        selectedBuilding = null;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (selectedUnit != null) {
                var targetPosition =
                    camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        cameraDistance));
                int x = tools.PosX2IPosX(targetPosition.x);
                int y = tools.PosY2IPosY(targetPosition.y);
                bool attack = false;
                unit target = units.GetUnitAt(targetPosition);
                if (target != null) {
                    if (target.GetPlayer() != myPlayer) {
                        if (tools.IsInAttackRange(selectedUnit, target)) {
                            attack = true;
                            units.Attack(selectedUnit, target);
                        }
                        else {
                            units.MoveToTargetAndAttack(selectedUnit, target);
                            attack = true;
                        }
                    }
                    else {
                        //TODO: PARAVOZIK
                    }
                }

                if (!attack) {
                    var b = buildings.GetBuildAt(targetPosition);
                    if (b) {
                        if (b.GetPlayer() != myPlayer) {
                            if (tools.IsInAttackRange(selectedUnit, b))
                            {
                                attack = true;
                                units.Attack(selectedUnit, b);
                            }
                            else
                            {
                                units.MoveToTargetAndAttack(selectedUnit, b);
                                attack = true;
                            }
                        }
                    }
                }

                if (!attack) {

                    units.MoveTo(selectedUnit, x, y);
                }

            selectedUnit.Unselect();
                selectedUnit = null;
            }
        }
    }
    public void LoadMapOld() {

        units = new unitManager();
        units.SetPlayer(1);
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
        units.CreateUnit(eUnitType.kTrike, 1,1, 1);
        units.CreateUnit(eUnitType.kTrike, 2,5, 5);
        buildings = new buildingsManager();
        buildings.Init();
        buildings.Build(4, 2, eBuildingType.kBase, 1);
        buildings.Build(7, 2, eBuildingType.kBase, 2);
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
