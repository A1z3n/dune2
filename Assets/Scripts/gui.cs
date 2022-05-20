using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gui : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject builds;
    void Start()
    {
        gameManager.GetInstance().SetGui(this);
        builds = GameObject.Find("builds");
        builds.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetBuildsActive(bool active) {
        builds.SetActive(active);
    }
}
