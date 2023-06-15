using System.Collections.Generic;
using System.Linq;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;

namespace Assets.Scripts.Editor {
    [AutoCustomTmxImporter()]
    public class CustomImporter : CustomTmxImporter {
        private TmxAssetImportedArgs m_ImportedArgs;
        private List<Vector3Int> spices;
        private Dictionary<int,Vector2> bases;
        
        public override void TmxAssetImported(TmxAssetImportedArgs args) {
            m_ImportedArgs = args;
            spices = new List<Vector3Int>();
            bases = new Dictionary<int, Vector2>();
            Parse();
        }
        
        private void Parse() {

            var tiles = m_ImportedArgs.ImportedSuperMap.GetComponentsInChildren<SuperTile>()
                .Where(o => o.m_Type == "player");
            /*
            var tiles = m_ImportedArgs.ImportedSuperMap.GetComponentsInChildren<SuperTile>()
                .Where(o => o.m_Type == "spice");
        
            foreach (var t in tiles) {
                spices.Add(new Vector3Int((int)t.m_Position.x,(int)t.m_Position.y, t.m_TileId));
            }

            var blist = m_ImportedArgs.ImportedSuperMap.GetComponentsInChildren<SuperTile>()
                .Where(o => o.m_Type == "player");
            foreach (var b in blist) {
                switch(b.m_TileId) {
                    case 23:
                        bases[0] = b.m_Position;
                        break;
                    case 24:
                        bases[1] = b.m_Position;
                        break;
                    case 25:
                        bases[2] = b.m_Position;
                        break;
                    case 26:
                        bases[3] = b.m_Position;
                        break;
                }
            }*/
            /*
        // Find all Tiled Objects in our map that are the moving platform type
        var platforms = m_ImportedArgs.ImportedSuperMap.GetComponentsInChildren<SuperObject>().Where(o => o.m_Type == "MovingPlatform");

        // Find all tracks in our maps. These are edge colliders under the "Track" layer
        var tracks = m_ImportedArgs.ImportedSuperMap.GetComponentsInChildren<EdgeCollider2D>().Where(c => c.gameObject.layer == LayerMask.NameToLayer("Track"));

        foreach (var marker in platforms)
        {
            // Instantiate the sprite for our moving platform
            var compPlatform = InstantiateMovingPlatform("MovingPlatform", marker);
            if (compPlatform != null)
            {
                // Find a track for our platform to be attached to
                foreach (var track in tracks)
                {
                    if (compPlatform.AssignTrackIfClose(track))
                    {
                        break;
                    }
                }
            }
            
        }*/
        }

        /*
  private spice InstantiateMovingPlatform(string name, SuperObject marker)
  {

      var go = Resources.Load("MovingPlatform");
      if (go == null)
      {
          m_ImportedArgs.AssetImporter.ReportError("MovingPlatform resource not found.");
          return null;
      }

      // Add our moving platform to the map
      var goPlatform = UnityEngine.Object.Instantiate(go, marker.transform) as GameObject;

      // Custom properties can control platform speed and inital direction
      var compPlatform = goPlatform.GetComponent<MovingPlatformOnTrack>();
      compPlatform.m_Speed = marker.gameObject.GetSuperPropertyValueFloat("Speed", 64.0f);
      compPlatform.m_InitialDirection.x = marker.gameObject.GetSuperPropertyValueFloat("Direction_x", 1.0f);
      compPlatform.m_InitialDirection.y = marker.gameObject.GetSuperPropertyValueFloat("Direction_y", 1.0f);

      return compPlatform;
    
}
      */
        public Vector2 GetBasePos(int id) {
            bases.TryGetValue(id,out var result);
            return result;
        }

    }
}


