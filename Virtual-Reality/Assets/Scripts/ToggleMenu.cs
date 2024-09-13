using UnityEngine;

public class ToggleVisibilityAction : MonoBehaviour
{
    // Public GameObject to be shown or hidden
    public GameObject targetObject; // The object assigned to toggle its visibility
    public ButtonBase toggleSiteMenu; // Assign the toggleSiteMenu button object in the Inspector

    private void Awake()
    {
        toggleSiteMenu.AddAction(ToggleVisibility);
    }

    // Method to toggle the visibility of the target object
    public void ToggleVisibility()
    {
        if (targetObject != null)
        {
            // Toggle the active state of the target object
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
