using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

public class refinery : building {
    private SpriteRenderer[] lights;
    private bool lightsAnim = false;
    private int prevFrame = 0;
    // Start is called before the first frame update
    public override void Init(int x, int y, int pPlayer, float pHealthPart)
    {
        rect.width = 3;
        rect.height = 2;
        type = eBuildingType.kRefinery;
        base.Init(x, y, pPlayer, pHealthPart);
        lights = new SpriteRenderer[3];
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < 3; i++) {
            foreach (var it in sprites) {
                if (it.name == "lights" + (i+1)) {
                    lights[i] = it;
                    it.enabled = false;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        gameManager.GetInstance().AddEnergy(-30);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (lightsAnim) {
            int t = Time.frameCount / 360;
            t = t % 3;
            if (t != prevFrame) {
                lights[prevFrame].enabled = false;
                lights[t].enabled = true;
                prevFrame = t;
            }
        }
    }


    public override void Select()
    {
        base.Select();
    }
    public override void Unselect()
    {
        base.Unselect();
    }

    private void TurnOnLights() {
        lightsAnim = true;
    }
    private void TurnOffLights()
    {
        lightsAnim = false;
        for (int i = 0; i < 3; i++) {
            lights[i].enabled = false;
        }
    }
}
