using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Dune2 {
    public class spice : MonoBehaviour {
        public enum eSpiceType {
            kNone,
            kFull,
            kRightDown,
            kLeftDown,
            kRightUp,   
            kLeftUp
        }
        public eSpiceType type = eSpiceType.kFull;
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

        public void Init(int x, int y) {
            pos.x = x;
            pos.y = y;
            transform.position = tools.iPos2PosB(x,y);
        }

        public void ChangeTexture(Sprite spr) {
            var sp = GetComponentInChildren<SpriteRenderer>();
            sp.sprite = spr;
        }
    }
}
