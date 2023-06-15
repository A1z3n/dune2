using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dune2 {
    [CreateAssetMenu]
    public class tileData : ScriptableObject {
        public TileBase[] tiles;
        public int speed;

    }
}
