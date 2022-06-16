using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

public class baseBuilding : building {
    public override void Init(int x, int y, int pPlayer, float pHealthPart)
    {
        rect.width = 2;
        rect.height = 2;
        type = eBuildingType.kBase;
        base.Init(x,y, pPlayer,pHealthPart);
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


    public override void Select()
    {
        base.Select();
        gameManager.GetInstance().GetGui().SetBuildsActive(true);
    }
    public override void Unselect()
    {
        base.Unselect();
        gameManager.GetInstance().GetGui().SetBuildsActive(false);
    }
}
