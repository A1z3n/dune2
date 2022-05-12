using System;
using System.Collections.Generic;
using System.Linq;
using AI.A_Star;
using UnityEngine;
using Vector2Int = UnityEngine.Vector2Int;
//using AStar_2D;

namespace Assets.Scripts {

    public class astar {
        private static readonly astar instance = new astar();
        public int width;
        public int height;

        private int[][] mapWeights;
        private bool[][] units;
        private bool[][] buildings;
        private Path mPath;
        private List<AI.A_Star.Vector2Int> obstacles;

        public static astar GetInstance()
        {
            return instance;
        }
        // private tileMap map;

        public void Init(int w, int h) {
            width = w;
            height = h;
            mPath = new Path(1000,w*h);
            obstacles = new List<AI.A_Star.Vector2Int>(width * height);
            //   map.init(w, h);
        }

        public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY) {
            FillObstacles();
            IReadOnlyCollection<AI.A_Star.Vector2Int> path1 = new List<AI.A_Star.Vector2Int>();
            mPath.Calculate(new AI.A_Star.Vector2Int(startX, startY), new AI.A_Star.Vector2Int(endX, endY), obstacles, out path1);
            List<Vector2Int> result = new List<Vector2Int>();
            foreach (var p in path1)
            {
                result.Add(new Vector2Int(p.X, p.Y));
            }
            return result;
        }
        public bool FindNext(int startX, int startY, int endX, int endY, out Vector2Int next)
        {
            FillObstacles();
            AI.A_Star.Vector2Int n = new AI.A_Star.Vector2Int();
            bool result = mPath.CalculateNext(new AI.A_Star.Vector2Int(startX, startY), new AI.A_Star.Vector2Int(endX, endY), obstacles, out n);
            next = Vector2Int.up;
            next.x = n.X;
            next.y = n.Y;
            return result;
        }

        private void FillObstacles() {
            obstacles.Clear();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (mapWeights[x][y] == 0) {
                        obstacles.Add(new AI.A_Star.Vector2Int(x,y));
                    }
                    else if(buildings[x][y]){
                        obstacles.Add(new AI.A_Star.Vector2Int(x,y));
                    }
                    else if (units[x][y])
                    {
                        obstacles.Add(new AI.A_Star.Vector2Int(x, y));
                    }
                }
            }
        }

        public void FillMap(int[][] weights) {
            mapWeights = weights;
            //foreach (int[] row in weights) {
            //    foreach (int number in row) {

            //    }
            //}

            units = new bool[width][];
            buildings = new bool[width][];
            for (int x = 0; x < width; x++)
            { 
                units[x] = new bool[height];
                buildings[x] = new bool[height];
                for (int y = 0; y < height; y++) { 
                    units[x][y] = false;
                    buildings[x][y] = false;
                }
            }
        }

        public void AddUnit(Vector2Int pos) {
            units[pos.x][pos.y] = true;
        }
        public void AddUnit(int x, int y)
        {
            units[x][y] = true;
        }

        public void ChangeUnitPos(Vector2Int from, Vector2Int to) {
            units[from.x][from.y] = false;
            units[to.x][to.y] = true;
        }

        public void ChangeUnitPos(int fromX, int fromY, int toX, int toY)
        {
            units[fromX][fromY] = false;
            units[toX][toY] = true;
        }

        public void RemoveUnit(Vector2Int pos) {
            units[pos.x][pos.y] = false;
        }
        public void RemoveUnit(int x, int y)
        {
            units[x][y] = false;
        }

        public void AddBuilding(RectInt posRect) {
            for (int i = posRect.x; i < posRect.x+posRect.width; i++) {
                for (int j = posRect.y; j < posRect.y + posRect.height; j++) {
                    buildings[i][j] = true;
                }
            }
        }

        public void RemoveBuilding(RectInt posRect) {
            for (int i = posRect.x; i < posRect.x + posRect.width; i++)
            {
                for (int j = posRect.y; j < posRect.y + posRect.height; j++)
                {
                    buildings[i][j] = false;
                }
            }
        }
    }
}