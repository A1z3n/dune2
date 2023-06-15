using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class waitAction : action {
        private float timer;

        public void Init(float t) {
            timer = t;
        }

        public override bool Update(actionBase u, float dt) {
            timer -= dt;
            if (timer > 0) {
                return false;
            }

            return true;
        }

        public override eActionType GetActionType() {
            return eActionType.kWaitAction;
        }
    }
}