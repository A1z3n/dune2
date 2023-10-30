using System.Collections;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;

// The AutoCustomTmxImporterAttribute will force this importer to always be applied.
// Leave this attribute off if you want to choose a custom importer from a drop-down?list instead.[AutoCustomTmxImporter()]

namespace Dune2 {

    public class gameManager {
        public Camera mainCamera;
        private static readonly gameManager instance = new gameManager();
        private int youColor;
        private mapManager mMap;
        public gui Gui;
        private int Credits = 0;
        private int Energy = 0;
        private int WindtrapsNum = 0;
        public string Date { get; private set; }

        private bool blackout = false;
        private int mPlayer = 1;

        private gameManager() {
            youColor = 1;
            mainCamera = Camera.main;
            var mm = GameObject.Find("mapManager");
            mMap = mm.GetComponent<mapManager>();
            //TODO: Set Player for Multiplayer
            mMap.SetPlayer(mPlayer);
        }

        public static gameManager GetInstance() {
            return instance;
        }

        public Vector2Int GetMapSize() {
            if (mMap != null)
                return mMap.GetMapSize();
            return new Vector2Int(0, 0);
        }

        public Vector2Int GetTileSize() {
            if (mMap != null)
                return mMap.GetTileSize();
            return new Vector2Int(0, 0);
        }

        public Vector2 GetCellSize() {
            if (mMap != null)
                return mMap.GetCellSize();
            return new Vector2(0, 0);
        }

        public Camera GetCamera() {
            if(mainCamera!=null)
                return mainCamera;
            return Camera.main;
        }

        public unitManager GetUnitManager() {
            if (mMap != null)
                return mMap.GetUnitManager();
            return null;
        }

        public SuperMap getMap() {
            if (mMap != null)
                return mMap.getMap();
            return null;
        }

        public int getYourColor() {
            return youColor;
        }

        public void SetGui(gui pGui) {
            Gui = pGui;
        }

        public gui GetGui() {
            return Gui;
        }

        public void AddCredits(int num) {
            Credits += num;
            Gui.AddCredits(num);
        }

        public void SetCredits(int num) {
            Credits += num;
            Gui.SetCredits(Credits);
        }

        public bool CheckCredits(int num) {
            if (num <= Credits)
                return true;
            return false;
        }

        public mapManager GetMapManager() {
            return mMap;
        }

        public void AddEnergy(int value) {
            Energy += value;
            if (Energy < 0) {
                if (!blackout) {
                    BlackoutStart();
                }
            }
            else if (blackout) {
                BlackoutOver();
            }
        }

        private void BlackoutStart() {

        }

        private void BlackoutOver() {

        }

        public int GetEnergy() {
            return Energy;
        }

        public void AddWindtrap() {
            WindtrapsNum++;
        }

        public void RemoveWindtrap() {
            WindtrapsNum--;
        }

        public int GetWindtrapCount() {
            return WindtrapsNum;
        }

        public int GetCurrentPlayer() {
            return mPlayer;
        }
    }
}