using System.Collections;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;

public class action {

    public virtual bool Update(actionBase u, float dt) {
        return true;
    }

    public delegate void OnEndCallback();
}

public class actionSeq : action {
    
    private List<action> actions;

    public actionSeq()
    {
        actions = new List<action>();
    }
    public override bool Update(actionBase u, float dt) {
        if (actions.IsEmpty()) {
            return false;
        }
        if (!actions[0].Update(u,dt)) {
            actions.RemoveAt(0);
            if (actions.Count == 0) {
                //OnEndCallback();
                return false;
            }
        }
        return true;
    }

    public void AddAction(action a)
    {
        actions.Add(a);
    }
    
}

public class actionSim : action {
    List<action> actions;

    public actionSim() {
        actions = new List<action>();
    }
    public override bool Update(actionBase u, float dt) {

        foreach (var a in actions) {
            if (!a.Update(u,dt)) {
                actions.RemoveAt(0);
                if (actions.Count == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void addAction(action a) {
       
        actions.Add(a);
    }
}
