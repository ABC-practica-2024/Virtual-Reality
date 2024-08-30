using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    protected GameObject userInterface, siteSection, artefacts;
    TouchScreenKeyboard keyboard;
    private void Start()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        EnterMainMenu();
    }
    void EnterMainMenu()
    {
        userInterface.SetActive(false);
        siteSection.SetActive(false);
        artefacts.SetActive(false);
        keyboard.active = true;
    }
    void ExitMainMenu()
    {
        userInterface.SetActive(true);
        siteSection.SetActive(true);
        artefacts.SetActive(true);
        keyboard.active = false;
    }
}
