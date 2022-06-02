using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

public class destroyAction : action
{
    public void Init() {

    }

    public override bool Update(actionBase u, float dt) {
        var un = u as unit;
        if(un != null)
            un.DestroyMe();
        return false;
    }

    public override eActionType GetActionType()
    {
        return eActionType.kDestroyAction;
    }
}
