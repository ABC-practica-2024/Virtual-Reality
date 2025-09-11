using UnityEngine;

public class VisibilityControls : MonoBehaviour
{
    [SerializeField] protected GameObject CanvasMainMenu;
    [SerializeField] protected GameObject CanvasSite;
    [SerializeField] protected GameObject siteSection;
    public void OnSectionSelected()
    {
        //Makes the MainMenu Invisible
        CanvasMainMenu.SetActive(false);

        //Makes the site buttons visible
        CanvasSite.SetActive(true);

        siteSection.SetActive(true);
    }

    public void MainMenuButton()
    {
        //Makes the MainMenu visible
        CanvasMainMenu.SetActive(true);

        //Makes the game buttons invisible
        CanvasSite.SetActive(false);

        siteSection.SetActive(false);
    }
}
