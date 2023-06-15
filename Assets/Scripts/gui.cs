
using System;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class gui : MonoBehaviour {
        // Start is called before the first frame update
        private GameObject builds;
        public creditsGUI credits;
        private Dictionary<eBuildingType, GameObject> buildings;

        void Start() {
            buildings = new Dictionary<eBuildingType, GameObject>();
            gameManager.GetInstance().SetGui(this);
            builds = GameObject.Find("GUI/builds");
            builds.SetActive(false);
            {
                var b = GameObject.Find("GUI/Icons/windtrap");
                b.SetActive(false);
                buildings[eBuildingType.kWindTrap] = b;
            } /*
        {
            var b = GameObject.Find("GUI/Icons/refinery");
            buildings[eBuildingType.kRefinery] = b;
        }
        */

        }

        // Update is called once per frame
        void Update() {

        }

        public void SetBuildsActive(bool active) {
            builds.SetActive(active);
        }

        public void ShowBuildIcon(eBuildingType type) {
            var b = buildings[type];
            b.SetActive(true);
        }

        public void HideBuildIcon() {
            foreach (var t in buildings) {
                t.Value.SetActive(false);
            }
        }

        public void AddCredits(int num) {
            credits.AddCredits(num);
        }

        public void SetCredits(int num) {
            credits.SetCredits(num);
        }

        public void checkDependencies() {

        }
    }

}