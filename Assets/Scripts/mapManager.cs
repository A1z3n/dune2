using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using Dune2;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Dune2 {
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
        private Dictionary<int, SpriteRenderer> buildModeRenderers;
        private List<int> buildModeConcretes;
        private Vector2Int mousePos;
        private bool bBuilding = false;
        private float unselectTimer = 0.0f;
        private bool unselectCheck = false;

        private spiceManager mSpiceManager;
        private void Awake() {
            LoadMapTmx("scene/map");
        }

        void Start() {
            buildModeConcretes = new List<int>();
            buildModePos = new RectInt();
            buildModeRenderers = new Dictionary<int, SpriteRenderer>();
           
        }

        void Init() {
            inited = true;
            BuildBases();
            //buildings.Build(4, 2, eBuildingType.kBase, 1);

            //units.CreateUnit(eUnitType.kTrike, 1, 2, 2);
            //units.CreateUnit(eUnitType.kHarvester, 1, 2, 5);
            //buildings.Build(6, 2, eBuildingType.kConcrete, 1);
            // //units.CreateUnit(eUnitType.kTrike, 2,5, 5);
            gameManager.GetInstance().AddCredits(1000);
            mSpiceManager = new spiceManager();
            mSpiceManager.Init(mapSize);
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

            if (Input.GetMouseButtonDown(0)) {

                if (!camera) {
                    camera = gameManager.GetInstance().GetCamera();
                }



                if (selectedUnit != null) {
                    selectedUnit.Unselect();
                }

                selectedUnit = null;
                var targetPosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        cameraDistance));
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
                    if (buildModeType != eBuildingType.kConcrete && count == buildSize) {
                        buildings.Build(buildModePos.x, buildModePos.y, buildModeType, myPlayer);
                        DeactivateBuildMode();
                    }
                    else if (count > 0) {
                        //buildModeConcretes
                        buildings.Build(buildModePos.x, buildModePos.y, buildModeType, myPlayer, count);
                        DeactivateBuildMode();
                    }

                }
            }

            if (Input.GetMouseButtonDown(1)) {
                if (selectedUnit != null) {
                    var targetPosition =
                        camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            cameraDistance));
                    int x = tools.RoundPosX(targetPosition.x);
                    int y = tools.RoundPosY(targetPosition.y);
                    bool attack = false;
                    unit target = units.GetUnitAt(targetPosition);
                    if (target != null) {
                        if (target.GetPlayer() != myPlayer) {
                            if (tools.IsInAttackRange(selectedUnit, target)) {
                                attack = true;
                                selectedUnit.CancelActions();
                                units.Attack(selectedUnit, target, true);
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
                                if (tools.IsInAttackRange(selectedUnit, b)) {
                                    attack = true;
                                    selectedUnit.CancelActions();
                                    units.Attack(selectedUnit, b, true);
                                }
                                else {
                                    selectedUnit.CancelActions();
                                    units.MoveToTargetAndAttack(selectedUnit, b);
                                    attack = true;
                                }
                            }
                        }
                    }

                    if (!attack) {
                        //selectedUnit.CancelActions();
                        selectedUnit.CancelNextActions();
                        selectedUnit.OnMoveCommand();
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

        public void LoadMapTmx(string path) {

            units = new unitManager();
            dataFromTiles = new Dictionary<TileBase, tileData>();
            foreach (var td in tileDatas) {
                foreach (var t in td.tiles) {
                    dataFromTiles.Add(t, td);
                }
            }

            mapTile = GameObject.Find(path);
            mapSize = new Vector2Int();
            var sm = mapTile.GetComponent<SuperMap>();
            cellSize.x = sm.m_TileWidth;
            cellSize.y = sm.m_TileHeight;
            mapSize.x = sm.m_Width;
            mapSize.y = sm.m_Height;
            astar.GetInstance().Init(mapSize.x, mapSize.y);

            //var spices = GameObject.Find("scene/map/Grid/spices");
            //var bases = GameObject.Find("scene/map/Grid/bases").GetComponent<Tilemap>();


            int[][] weights = new int[mapSize.x][];

            for (int x = 0; x < mapSize.x; x++) {

                weights[x] = new int[mapSize.y];
                for (int y = 0; y < mapSize.y; y++) {
                    //TileBase t = tm.GetTile(new Vector3Int(x, y, 0));
                    //t.GetTileData(new Vector3Int(x,y,0));
                    //Debug.Log(t.name);
                    //var obj = bases.GetTile<TileBase>(new Vector3Int(x,y));
                    //if (obj != null) {
                       // Debug.Log(obj.name);
                    //}
                    int w = 1;
                    //var t = bases.GetTile(new Vector3Int(x, y, 0));
                    //if (t != null) {
                       // Debug.Log(t.name);
                    //}

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
            for (int x = 0; x < mapSize.x; x++) {

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

        public Vector2Int GetMapSize() {
            return mapSize;
        }

        public Vector2Int GetTileSize() {
            return tileSize;
        }

        public Vector2 GetCellSize() {
            return cellSize;
        }

        public unitManager GetUnitManager() {
            return units ??= new unitManager();
        }

        public SuperMap getMap() {
            return superMap;
        }

        private void BuildModeUpdate() {
            if (buildSize > 0) {
                if (buildModeObject != null) {
                    var targetPosition =
                        camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            cameraDistance));
                    int x = tools.RoundPosX(targetPosition.x - 0.25f);
                    int y = tools.RoundPosY(targetPosition.y + 0.25f);
                    if (buildModePos.x != x || buildModePos.y != y) {
                        CheckForBuild(x, y, buildModeType == eBuildingType.kConcrete);
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
                    else if (buildSize == 9) {
                        buildModePos.width = 3;
                        buildModePos.height = 3;
                    }

                    buildModeObject.transform.position = tools.iPos2PosB(x, y); // + new Vector3(-0.5f,-0.5f);

                }
            }
        }

        public int CheckForBuild(int x, int y, bool checkconcrete = false) {
            buildModeConcretes.Clear();
            int num = 0;
            int count = 0;
            for (int j = y; j < y + buildModePos.height; j++) {
                for (int i = x; i < x + buildModePos.width; i++) {

                    bool fnd = false;

                    if (!astar.GetInstance().CheckForFree(i, j)) {
                        fnd = true;
                    }
                    else if (checkconcrete && buildings.CheckConcrete(i, j)) {
                        fnd = true;
                    }

                    num++;
                    if (fnd) {
                        buildModeRenderers[num].enabled = false;
                        buildModeRenderers[10 + num].enabled = true;
                    }
                    else {
                        count++;
                        buildModeConcretes.Add(num);
                        if (buildModeRenderers.TryGetValue(num, out var r1)) {
                            r1.enabled = true;
                        }

                        if (buildModeRenderers.TryGetValue(10 + num, out var r2)) {
                            r2.enabled = false;
                        }
                    }
                    }
            }

            return count;
        }

        public void ActivateBuildMode(eBuildingType type) {

            buildModeRenderers.Clear();
            buildSize = buildings.GetBuildSize(type);
            buildModeObject =
                scene.Instantiate(Resources.Load("buildMode" + buildSize, typeof(GameObject))) as GameObject;
            var list = buildModeObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var it in list) {
                for (int i = 1; i < buildSize + 1; i++) {
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

        public void BuildBarracks() {
            buildings.StartBuilding(eBuildingType.kBarracks);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildPalace() {
            buildings.StartBuilding(eBuildingType.kPalace);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildRadar() {
            buildings.StartBuilding(eBuildingType.kRadar);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildRepair() {
            buildings.StartBuilding(eBuildingType.kRepair);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildStarPort() {
            buildings.StartBuilding(eBuildingType.kStarPort);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildTurret() {
            buildings.StartBuilding(eBuildingType.kTurret);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildRocketTurret() {
            buildings.StartBuilding(eBuildingType.kTurretRocket);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildVehicle() {
            buildings.StartBuilding(eBuildingType.kVehicle);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildAir() {
            buildings.StartBuilding(eBuildingType.kAir);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildBase() {
            buildings.StartBuilding(eBuildingType.kBase);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildRefinery() {
            buildings.StartBuilding(eBuildingType.kRefinery);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildSilo() {
            buildings.StartBuilding(eBuildingType.kSilo);
            bBuilding = true;
            unselectCheck = false;
        }

        public void BuildWall() {
            buildings.StartBuilding(eBuildingType.kWall);
            bBuilding = true;
            unselectCheck = false;
        }

        public buildingsManager GetBuildingsManager() {
            return buildings;
        }


        void RefreshSpicesConfig() {
           mSpiceManager.RefreshSpicesConfig();
        }

        public void AddSpiceBomb(Vector3 pos, int spices) {
            mSpiceManager.AddSpiceBomb(pos, spices);
        }

        public void CheckSpiceMines(int x, int y) {
            mSpiceManager.CheckSpiceMines(x,y);
        }

        public void ActivateSpiceBomb(int x, int y, int spices) {
            mSpiceManager.ActivateSpiceBomb(x,y,spices);
        }

        public void AddSpice(spice s) {
            mSpiceManager.AddSpice(s);
        }

        public void AddSpiceAt(int x, int y) {
            mSpiceManager.AddSpiceAt(x,y);
        }

        public Vector2Int SearchNearestSpice(Vector2Int from) {
            return mSpiceManager.SearchNearestSpice(from);
        }

        public Vector2Int SearchNearestBuildingPosition(eBuildingType type, int x, int y) {
            return GetBuildingsManager().GetNearestBuilding(type, x, y);
        }
        public bool IsSpiceAtPoint(Vector2Int pos) {
            return mSpiceManager.IsSpiceAtPoint(pos);
        }

        public int AddSpiceCountAt(int x, int y, int count) {
            return mSpiceManager.AddSpiceCountAt(x, y, count);
        }

        private void BuildBases() {
            var base1 = GameObject.Find("scene/map/Grid/bases/1").GetComponent<SuperObject>();
            var base2 = GameObject.Find("scene/map/Grid/bases/2").GetComponent<SuperObject>(); ;
            var x1 = tools.RoundPosX(base1.transform.position.x);
            var y1 = tools.RoundPosY( base1.transform.position.y)-1;
            var x2 = tools.RoundPosX(base2.transform.position.x);
            var y2 = tools.RoundPosY(base2.transform.position.y)-1;
            buildings.Build(x1, y1, eBuildingType.kBase, 1);
            buildings.Build(x2, y2, eBuildingType.kBase, 2);
            gameManager.GetInstance().GetCamera().GetComponent<CameraController>().LookAt(x1,y1);

            CreateStartUnits(x1,y1,1);
            CreateStartUnits(x2, y2, 2);
        }

        private void CreateStartUnits(int x, int y, int player) {
            var posx = x;
            var posy = y;
            Random rnd = new Random();
            var rx = rnd.Next(1, 3);
            var ry = rnd.Next(1, 3);
            var signx = rnd.Next(0, 2);
            var signy = rnd.Next(0, 2);
            if (signx == 1) {
                posx += 1 + rx;
            }
            else {
                posx -= rx;
            }
            if (signy == 1)
            {
                posy += 1 + ry;
            }
            else
            {
                posy -=  ry;
            }


            units.CreateUnit(eUnitType.kTrike, player, posx, posy);

        }

        public void SetPlayer(int player) {
            myPlayer = player;
        }

        public bool IsSpiceAt(int x, int y) {
            if (mSpiceManager.GetSpiceAt(x, y) != null) {
                return true;
            }

            return false;
        }
    }

}
