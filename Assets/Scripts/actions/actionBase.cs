using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class actionBase : MonoBehaviour
{
    // Start is called before the first frame update
    private List<action> actions;
    private List<action> delItems;
    protected void Init() {
        actions = new List<action>();
        delItems = new List<action>();
    }


    // Update is called once per frame
    protected void Update() {
        float dt = Time.deltaTime;
        foreach (var currentAction in actions) {
            if ( !currentAction.Update(this,dt)) {
                delItems.Add(currentAction);
            }
        }

        foreach (var delItem in delItems) {
            actions.Remove(delItem);
        }
    }

    public void AddAction(action a) {
        if (actions.Equals(null))
        {
            actions = new List<action>();
        }
        actions.Add(a);
       
    }

    public void RemoveAction(action a) {
        actions.Remove(a);
    }

    public void ClearActions() {
        actions.Clear();
    }
}
