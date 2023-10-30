using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Dune2 {
    public class spice : MonoBehaviour {
        public int type = 0;
        public int count = 1000;
        public Vector2Int pos;

        // Start is called before the first frame update
        void Start() {
            //gameManager.GetInstance().GetMapManager().AddSpice(tools.RoundPosX(transform.position.x),
              //  tools.RoundPosY(transform.position.y), type);
              pos=new Vector2Int(tools.RoundPosX(transform.position.x), 
                  tools.RoundPosY(transform.position.y));
              gameManager.GetInstance().GetMapManager().AddSpice(this );
        }

        // Update is called once per frame
        void Update() {
        }
    }
}
