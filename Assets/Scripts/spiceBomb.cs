using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2 {
    public class spiceBomb : MonoBehaviour {
        // Start is called before the first frame update
        public int spices = 10000;
        public bool first = true;

        void Start() {
            if (first) {
                gameManager.GetInstance().GetMapManager().ActivateSpiceBomb(tools.RoundPosX(transform.position.x),
                    tools.RoundPosY(transform.position.y), spices);
            }
            else {
                gameManager.GetInstance().GetMapManager().AddSpiceBomb(transform.position, spices);
            }
        }

        // Update is called once per frame
        void Update() {
        }
    }
}
