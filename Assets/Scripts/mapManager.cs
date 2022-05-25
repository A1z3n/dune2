using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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
    private int buildSize = 0;
    private GameObject buildModeObject;
    private bool inited = false;
    private eBuildingType buildModeType = eBuildingType.kNone;
    private Vector2Int buildModePos;


    private void Awake() {
        LoadMapOld();
    }

    void Start() {

        buildModePos = new Vector2Int();
    }

    void Init() {
        inited = true;
        buildings.Build(4, 2, eBuildingType.kBase, 1);
        units.CreateUnit(eUnitType.kTrike, 1, 1, 1);
        //buildings.Build(6, 2, eBuildingType.kConcrete, 1);
        // //units.CreateUnit(eUnitType.kTrike, 2,5, 5);
        gameManager.GetInstance().AddCredits(1000);
    }

    public void Update() {
        if (!inited) Init();
        checkInput();
        units.Update(Time.deltaTime);
        buildings.Update(Time.deltaTime);
        BuildModeUpdate();
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

            
            selectedUnit = null;
            var targetPosition =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            if (buildSize == 0) {
                selectedUnit = units.GetUnitAt(targetPosition);
                if (selectedUnit != null) {
                    if (selectedUnit.GetPlayer() != myPlayer)
                        selectedUnit = null;
                    else {
                        selectedUnit.Select();
                    }
                }
                else {
                    var select = buildings.GetBuildAt(targetPosition);
                    if (select != null) {
                        if (select.CheckPlayer(myPlayer)) {
                            select.Select();
                        }
                        else {
                            select = null;
                        }
                    }

                    if (select != null) {
                        if(selectedBuilding!=null)
                            selectedBuilding.Unselect();
                        selectedBuilding = select;
                    }
                }
            }
            else {

                if (gameManager.GetInstance().CheckCredits(buildings.GetBuildCost(buildModeType))) {
                    buildings.Build(buildModePos.x, buildModePos.y, buildModeType, myPlayer);
                }
                DeactivateBuildMode();
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
                            selectedUnit.CancelActions();
                            units.Attack(selectedUnit, target,true);
                        }
                        else {
                            selectedUnit.CancelActions();
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
                                selectedUnit.CancelActions();
                                units.Attack(selectedUnit, b,true);
                            }
                            else
                            {
                                selectedUnit.CancelActions();
                                units.MoveToTargetAndAttack(selectedUnit, b);
                                attack = true;
                            }
                        }
                    }
                }

                if (!attack) {
                    selectedUnit.CancelActions();
                    units.MoveTo(selectedUnit, x, y);
                }

                selectedUnit.Unselect();
                selectedUnit = null;
            }
            if (buildSize > 0) {
               DeactivateBuildMode();
            }
        }
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
        buildings = new buildingsManager();
        buildings.Init(mapSize.x, mapSize.y);
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

    private void BuildModeUpdate() {
        if (buildSize > 0) {
            if (buildModeObject != null) {
                var targetPosition =
                    camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        cameraDistance));
                int x = tools.PosX2IPosX(targetPosition.x+0.25f);
                int y = tools.PosY2IPosY(targetPosition.y-0.25f);
                buildModePos.x = x;
                buildModePos.y = y;

                buildModeObject.transform.position = tools.iPos2PosB(x,y);// + new Vector3(-0.5f,-0.5f);
            }
        }
    }

    public void ActivateBuildMode(eBuildingType type) {

        switch (type) {
            case eBuildingType.kTurret:
            case eBuildingType.kTurretRocket:
            case eBuildingType.kWall:
                buildSize = 1;
                break;
            case eBuildingType.kConcrete:
            case eBuildingType.kAir:
            case eBuildingType.kBarracks:
            case eBuildingType.kRadar:
            case eBuildingType.kSilo:
            case eBuildingType.kBase:
                buildModeObject = scene.Instantiate(Resources.Load("buildMode4", typeof(GameObject))) as GameObject;
                buildSize = 4;
                break;
            case eBuildingType.kRefinery:
            case eBuildingType.kRepair:
            case eBuildingType.kVehicle:
                buildSize = 6;
                break;
            case eBuildingType.kPalace:
            case eBuildingType.kStarPort:
                buildSize = 9;
                break;

        }

        buildModeType = type;
    }

    public void DeactivateBuildMode() {
        if (buildSize == 0) return;
        buildModeType = eBuildingType.kNone;
        scene.Destroy(buildModeObject);
        buildModeObject = null;
        buildSize = 0;
    }

    public void BuildConcrete() {
        buildings.StartBuilding(eBuildingType.kConcrete);
        //if (buildSize > 0) return;//TODO: cancel prev build
        //if (!gameManager.GetInstance().CheckCredits(buildings.GetBuildCost(eBuildingType.kConcrete)))
        //    return;
        //ActivateBuildMode(4);
        //buildModeType = eBuildingType.kConcrete;
    }
}
