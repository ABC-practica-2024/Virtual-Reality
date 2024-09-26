using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityControls : MonoBehaviour
{
    public GameObject CanvasMainMeniu;
    public GameObject CanvasSite;

    public void SelectButton()
    {   
        //Makes the MainMenu Invisible
        CanvasMainMeniu.SetActive(false);

        //Makes the site buttons visible
        CanvasSite.SetActive(true);

    }

    public void MainMenuButton()
    {
        //Makes the MainMenu visible
        CanvasMainMeniu.SetActive(true);

        //Makes the game buttons invisible
        CanvasSite.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
