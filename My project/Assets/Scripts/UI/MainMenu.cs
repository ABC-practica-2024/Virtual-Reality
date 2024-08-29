using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    protected GameObject UI, SiteSection, Artifacts;
    TouchScreenKeyboard keyboard;
    private void Start()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        EnterMainMenu();
    }
    void EnterMainMenu()
    {
        UI.SetActive(false);
        SiteSection.SetActive(false);
        Artifacts.SetActive(false);
        keyboard.active = true;
    }
    void ExitMainMenu()
    {
        UI.SetActive(true);
        SiteSection.SetActive(true);
        Artifacts.SetActive(true);
        keyboard.active = false;
    }
}
