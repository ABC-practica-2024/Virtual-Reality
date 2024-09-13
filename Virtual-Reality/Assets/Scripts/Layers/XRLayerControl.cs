using UnityEngine;

public class XRLayerControl : MonoBehaviour
{
    [SerializeField]
    private Transform layers;
    private int currentLayer = 0; // Current layer index
    public void HideLayer()
    {
        // Hide the current layer and move to the next layer
        if (currentLayer >= 0 && currentLayer < layers.childCount)
        {
            layers.GetChild(currentLayer).gameObject.SetActive(false); // Hide current layer
            // Move to the next layer
            currentLayer++;
        }
    }

    public void ShowLayer()
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
