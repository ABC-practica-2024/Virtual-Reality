using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityControls : MonoBehaviour
{
    public GameObject InteractableControlMainMenu;
    public GameObject InteractableControlsGame;

    public void SelectButton()
    {   
        //Makes the MainMenu Invisible
        InteractableControlMainMenu.SetActive(false);

        //Makes the game buttons visible
        InteractableControlsGame.SetActive(true);

    }

    public void MainMenuButton()
    {
        //Makes the MainMenu visible
        InteractableControlMainMenu.SetActive(true);

        //Makes the game buttons invisible
        InteractableControlsGame.SetActive(false);

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
