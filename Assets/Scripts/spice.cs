using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class spice : MonoBehaviour {
    public int spices = 5000;
    // Start is called before the first frame update
    void Start()
    {
        gameManager.GetInstance().GetMapManager().AddSpice(tools.PosX2IPosX(transform.position.x),tools.PosY2IPosY(transform.position.y), spices);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
