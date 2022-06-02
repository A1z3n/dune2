using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class gui : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject builds;
    public creditsGUI credits;
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

    public void AddCredits(int num) {
        credits.AddCredits(num);
    }
    public void SetCredits(int num)
    {
        credits.SetCredits(num);
    }

    public void checkDependencies() {

    }
}
