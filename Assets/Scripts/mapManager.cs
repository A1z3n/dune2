using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    private RectInt buildModePos;
    private Dictionary<int,SpriteRenderer>  buildModeRenderers;
    private List<int> buildModeConcretes;
    private Vector2Int mousePos;
    private bool bBuilding = false;
    private float unselectTimer = 0.0f;
    private bool unselectCheck = false;
    private Dictionary<Vector2Int, int> spicesList;
    private List<Vector3Int> spiceBombsList;
    private const int SPICE_MAX = 1000;
    private List<Vector2Int> searchList;

    private void Awake() {
        LoadMapOld();
    }

    void Start() {
        buildModeConcretes = new List<int>();
        buildModePos = new RectInt();
        buildModeRenderers = new Dictionary<int, SpriteRenderer>();
        spicesList = new Dictionary<Vector2Int, int>();
        spiceBombsList = new List<Vector3Int>();
        searchList = new List<Vector2Int>();
    }

    void Init() {
        inited = true;
        buildings.Build(4, 2, eBuildingType.kBase, 1);
        units.CreateUnit(eUnitType.kTrike, 1, 2, 2);
        units.CreateUnit(eUnitType.kHarvester, 1, 2, 5);
        //buildings.Build(6, 2, eBuildingType.kConcrete, 1);
        // //units.CreateUnit(eUnitType.kTrike, 2,5, 5);
        gameManager.GetInstance().AddCredits(1000);
        InitSpices();
    }

    public void Update() {
        if (!inited) Init();
        if (unselectCheck) {
            unselectTimer += Time.deltaTime;
            if (unselectTimer > 0.3f) {
                unselectTimer = 0.0f;
                unselectCheck = false;
                if (selectedBuilding != null) {
                    selectedBuilding.Unselect();
                }
            }
        }

        CheckInput();
        units.Update(Time.deltaTime);
        buildings.Update(Time.deltaTime);
        BuildModeUpdate();
    }

    void CheckInput() {

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
                    if (selectedBuilding != null) {
                        selectedBuilding.Unselect();
                        selectedBuilding = null;
                    }
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
                            if (selectedBuilding != null) {
                                selectedBuilding.Unselect();
                            }
                            selectedBuilding = select;
                            selectedBuilding.Select();
                        }
                    }
                    else {
                        unselectCheck = true;
                        unselectTimer = 0.0f;
                    }
                }
            }
            else {
                int count = CheckForBuild(buildModePos.x, buildModePos.y, buildModeType == eBuildingType.kConcrete);
                if (buildModeType!=eBuildingType.kConcrete && count==buildSize) {
                    buildings.Build(buildModePos.x, buildModePos.y, buildModeType, myPlayer);
                    DeactivateBuildMode();
                }
                else if(count>0){
                    //buildModeConcretes
                    buildings.Build(buildModePos.x, buildModePos.y, buildModeType, myPlayer,count);
                    DeactivateBuildMode();
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
        mapTile = GameObject.Find("scene/map");
        mapSize = new Vector2Int();
        var sm = mapTile.GetComponent<SuperMap>();
        cellSize.x = sm.m_TileWidth;
        cellSize.y = sm.m_TileHeight;
        mapSize.x = sm.m_Width;
        mapSize.y = sm.m_Height;
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
    
    public unitManager GetUnitManager() {
        return units ??= new unitManager();
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
                int x = tools.PosX2IPosX(targetPosition.x - 0.25f);
                int y = tools.PosY2IPosY(targetPosition.y + 0.25f);
                if (buildModePos.x != x || buildModePos.y != y) { 
                    CheckForBuild(x,y, buildModeType == eBuildingType.kConcrete);
                }
                buildModePos.x = x;
                buildModePos.y = y;
                if (buildSize == 4) {
                    buildModePos.width = 2;
                    buildModePos.height = 2;
                }
                else if (buildSize == 6) {
                    buildModePos.width = 3;
                    buildModePos.height = 2;
                }
                else if (buildSize == 9)
                {
                    buildModePos.width = 3;
                    buildModePos.height = 3;
                }

                buildModeObject.transform.position = tools.iPos2PosB(x,y);// + new Vector3(-0.5f,-0.5f);
                
            }
        }
    }

    public int CheckForBuild(int x, int y, bool checkconcrete = false) {
        buildModeConcretes.Clear();
        int num = 0;
        int count = 0;
        for (int j = y; j < y + buildModePos.height; j++)
        {
            for (int i = x ; i < x + buildModePos.width; i++)
            {

                bool fnd = false;
                
                if (!astar.GetInstance().CheckForFree(i, j))
                {
                    fnd = true;
                }
                else if (checkconcrete && buildings.CheckConcrete(i, j))
                {
                    fnd = true;
                }

                num++;
                if (fnd) {
                    buildModeRenderers[num].enabled = false;
                    buildModeRenderers[10+num].enabled = true;
                }
                else {
                    count++;
                    buildModeConcretes.Add(num);
                    buildModeRenderers[num].enabled = true;
                    buildModeRenderers[10 + num].enabled = false;
                }
            }
        }

        return count;
    }

    public void ActivateBuildMode(eBuildingType type) {

        buildModeRenderers.Clear();
        buildSize = buildings.GetBuildSize(type);
        buildModeObject = scene.Instantiate(Resources.Load("buildMode"+buildSize, typeof(GameObject))) as GameObject;
        var list = buildModeObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in list)
        {
            for (int i = 1; i < buildSize+1; i++) {
                if (it.name == "clear" + i) {
                    buildModeRenderers[i] = it;
                    break;
                }
                if (it.name == "busy" + i) {
                    buildModeRenderers[i + 10] = it;
                    break;
                }
            }
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
        bBuilding = true;
        buildings.StartBuilding(eBuildingType.kConcrete);
        unselectCheck = false;
    }

    public void BuildWindtrap() {
        bBuilding = true;
        buildings.StartBuilding(eBuildingType.kWindTrap);
        unselectCheck = false;
    }

    public void BuildBarracks()
    {
        buildings.StartBuilding(eBuildingType.kBarracks);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildPalace()
    {
        buildings.StartBuilding(eBuildingType.kPalace);
        bBuilding = true;
        unselectCheck = false;
    }

    public void BuildRadar()
    {
        buildings.StartBuilding(eBuildingType.kRadar);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildRepair()
    {
        buildings.StartBuilding(eBuildingType.kRepair);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildStarPort()
    {
        buildings.StartBuilding(eBuildingType.kStarPort);
        bBuilding = true;
        unselectCheck = false;
    }

    public void BuildTurret()
    {
        buildings.StartBuilding(eBuildingType.kTurret);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildRocketTurret()
    {
        buildings.StartBuilding(eBuildingType.kTurretRocket);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildVehicle()
    {
        buildings.StartBuilding(eBuildingType.kVehicle);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildAir()
    {
        buildings.StartBuilding(eBuildingType.kAir);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildBase()
    {
        buildings.StartBuilding(eBuildingType.kBase);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildRefinery()
    {
        buildings.StartBuilding(eBuildingType.kRefinery);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildSilo()
    {
        buildings.StartBuilding(eBuildingType.kSilo);
        bBuilding = true;
        unselectCheck = false;
    }
    public void BuildWall()
    {
        buildings.StartBuilding(eBuildingType.kWall);
        bBuilding = true;
        unselectCheck = false;
    }

    public buildingsManager GetBuildingsManager() {
        return buildings;
    }

    public void InitSpices() {

        
    }

    public void AddSpiceBomb(Vector3 pos, int spices) {
        Vector3Int result = new Vector3Int(tools.PosX2IPosX(pos.x),tools.PosY2IPosY(pos.y), spices);
        spiceBombsList.Add(result);
    }

    public void CheckSpiceMines(int x, int y) {
        foreach(var it in spiceBombsList)
        {
            if (it.x == x && it.y == y) {
                ActivateSpiceBomb(x, y, it.z);
                spiceBombsList.Remove(it);
                return;
            }
        }
    }

    public void ActivateSpiceBomb(int x, int y, int spices) {
        int sum = SPICE_MAX;
        spicesList.Add(new Vector2Int(x,y), SPICE_MAX);
        
        searchList.Clear();
        searchList.Add(new Vector2Int(x+1,y));
        searchList.Add(new Vector2Int(x, y-1));
        searchList.Add(new Vector2Int(x + 1, y-1));
        searchList.Add(new Vector2Int(x , y+1));
        searchList.Add(new Vector2Int(x + 1, y+ 1));
        searchList.Add(new Vector2Int(x - 1, y));
        searchList.Add(new Vector2Int(x - 1, y-1));
        searchList.Add(new Vector2Int(x - 1, y+1));
        searchList.Add(new Vector2Int(x + 2, y ));
        searchList.Add(new Vector2Int(x - 2, y ));
        searchList.Add(new Vector2Int(x , y - 2 ));
        searchList.Add(new Vector2Int(x , y + 2 ));
        //while (sum<spices) {
        //    sum += SPICE_MAX;
            
        //}
    }

    public void AddSpice(int x, int y, int spices) {
        spicesList.Add(new Vector2Int(x,y),spices);
    }
}
