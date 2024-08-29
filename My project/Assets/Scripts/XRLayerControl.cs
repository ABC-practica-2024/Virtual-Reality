using UnityEngine;

public class XRLayerControl : MonoBehaviour
{
    public ButtonBase hideButton; // Assign the Hide button object in the Inspector
    public ButtonBase showButton; // Assign the Show button object in the Inspector
    [SerializeField] ///asa poti vedea in inspector 
    private Transform layers;
    private int currentLayer = 0; // Current layer index

    private void Awake()
    {
        hideButton.AddAction(HideLayer);
        showButton.AddAction(ShowLayer);
    }

    private void HideLayer()
    {
        // Hide the current layer and move to the next layer
        if (currentLayer >= 0 && currentLayer < layers.childCount)
        {
            layers.GetChild(currentLayer).gameObject.SetActive(false); // Hide current layer
            // Move to the next layer
            currentLayer++;
        }
    }

    private void ShowLayer()
    {
        // Show the previous layer and move to the previous layer
        if (currentLayer > 0 && currentLayer <= layers.childCount)
        {

            layers.GetChild(currentLayer - 1).gameObject.SetActive(true); // Hide current layer
            // Move to the previous layer
            currentLayer--;
        }
    }
}
