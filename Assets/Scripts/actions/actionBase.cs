using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperTiled2Unity;
using UnityEngine;


public class actionBase : MonoBehaviour
{
    // Start is called before the first frame update
    private List<action> actions;
    private List<action> actionsToAdd;
    private List<action> actionsDelayed;
    private List<action> delItems;
    protected void Init() {
        actions = new List<action>();
        delItems = new List<action>();
        actionsToAdd = new List<action>();
        actionsDelayed = new List<action>();
    }


    // Update is called once per frame
    protected void Update() {
        float dt = Time.deltaTime;
        foreach (var currentAction in actions) {
            if (!currentAction.Update(this, dt)) {
                currentAction.OnEndCallback?.Invoke();
                delItems.Add(currentAction);
            }
        }

        if (!delItems.IsEmpty()) {
            foreach (var delItem in delItems) {
                actions.Remove(delItem);
            }
            delItems.Clear();
        }

        if (actions.IsEmpty()) {
            if (!actionsToAdd.IsEmpty()) {

                AddAction(actionsToAdd[0]);
                actionsToAdd.RemoveAt(0);
            }
        }

        if (!actionsDelayed.IsEmpty()) {
            foreach (var a in actionsDelayed) {
                AddAction(a);
            }
            actionsDelayed.Clear();
        }
    }

    public void AddAction(action a) {
        if (actions.Equals(null))
        {
            actions = new List<action>();
        }
        actions.Add(a);
       
    }

    public void AddActionLazy(action a) {
        actionsToAdd.Add(a);
    }

    public void AddActionDelayed(action a) {
        actionsDelayed.Add(a);
    }

    public void RemoveAction(action a) {
        actions.Remove(a);
    }

    public void ClearActions() {
        actions.Clear();
        actionsToAdd.Clear();
    }

    public List<action> GetActions() {
        return actions;
    }

    public bool IsActions()
    {
        return !actions.IsEmpty() || !actionsToAdd.IsEmpty() || !actionsDelayed.IsEmpty();
    }

    public void CancelActions() {
        foreach (var currentAction in actions){
            currentAction.Cancel();
        }
        actionsToAdd.Clear();
        actionsDelayed.Clear();
    }

}
