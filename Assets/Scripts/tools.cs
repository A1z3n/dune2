using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


namespace Assets.Scripts
{
    static class tools {
        public static Vector3 iPos2PosB(int x, int y) {
            return new Vector3(x, -y, 0.0f);
        }

        public static Vector3 iPos2Pos(int x, int y) {
            return new Vector3(0.5f + x, -0.5f - y,0.0f);
        }

        public static int posX2iPosX(float x) {
             return (int)Math.Round(x-0.5f);
        }

        public static int posY2iPosY(float y) {
            //pos.y = -0.32f - y * 0.64f;
            return (int)Math.Round(-0.5f - y);
        }

        public static int getDirection(Vector2Int pos) {
            
            double ang = 0.0;
            if(pos.x!=0){
                ang = Math.Atan2(pos.y, pos.x) * 180 / Math.PI;
            }
            else if (pos.y > 0 ) {
               ang = 90;
            }
            else {
                ang = 270;
            }
            return (int)(ang/22.5f);
        }
    }
}
