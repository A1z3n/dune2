using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2 {
    public class spice : MonoBehaviour {
        public int type = 0;

        // Start is called before the first frame update
        void Start() {
            gameManager.GetInstance().GetMapManager().AddSpice(tools.RoundPosX(transform.position.x),
                tools.RoundPosY(transform.position.y), type);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
