using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseBuilding : building {
    public void Init(int x, int y, int player) {
        base.Init(x,y,player);
        rect.width = 2;
        rect.height = 2;
        health = 500;
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
