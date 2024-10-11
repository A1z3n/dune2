using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class refinery : building {
        private SpriteRenderer[] lights;

        private bool lightsAnim = false;

        private int oldFrame = 0;
        // Start is called before the first frame update
        public override void Init(int x, int y, int pPlayer, float pHealthPart)
        {
            base.Init(x, y, pPlayer, pHealthPart);
            rect.width = 3;
            rect.height = 2;
            type = eBuildingType.kRefinery;
            lights = new SpriteRenderer[3];
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < 3; i++) {
                foreach (var it in sprites) {
                    if (it.name == "lights" + (i + 1)) {
                        lights[i] = it;
                        it.enabled = false;
                    }
                }
            }

            
        }

        // Start is called before the first frame update
        void Start() {
            base.Start();
            gameManager.GetInstance().AddEnergy(-30);
        }

        // Update is called once per frame
        void Update() {
            base.Update();
            if (lightsAnim) {
                int t = Time.frameCount / 360;
                t = t % 3;
                if (t != oldFrame) {
                    lights[oldFrame].enabled = false;
                    lights[t].enabled = true;
                    oldFrame = t;
                }
            }
        }


        public override void Select() {
            base.Select();
        }

        public override void Unselect() {
            base.Unselect();
        }

        public void TurnOnLights() {
            lightsAnim = true;
        }

        public void TurnOffLights() {
            lightsAnim = false;
            for (int i = 0; i < 3; i++) {
                lights[i].enabled = false;
            }
        }

        protected override void Activated() {
            gameManager.GetInstance().GetUnitManager().CreateUnit(eUnitType.kHarvester,
                gameManager.GetInstance().GetCurrentPlayer(), rect.x + 2, rect.y + 1);
        }
        public override eBuildingType GetBuildingType()
        {
            return type;
        }
    }

}