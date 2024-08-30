using UnityEngine;

public class XRLayerControl : MonoBehaviour
{
    //handles layer control from the user
    [SerializeField] protected ButtonBase hideButton; // Assign the Hide button object in the Inspector
    [SerializeField] protected ButtonBase showButton; // Assign the Show button object in the Inspector
    [SerializeField] protected Transform layers;
    protected int currentLayer = 0; // Current layer index
    protected void Awake()
    {
        hideButton.AddAction(HideLayer);
        showButton.AddAction(ShowLayer);
    }
    protected void HideLayer()
    {
        // Hide the current layer and move to the next layer
        if (currentLayer >= 0 && currentLayer < layers.childCount)
        {
            layers.GetChild(currentLayer).gameObject.SetActive(false); // Hide current layer
            // Move to the next layer
            currentLayer++;
        }
    }
    protected void ShowLayer()
    {
        // Show the previous layer and move to the previous layer
        if (currentLayer > 0 && currentLayer <= layers.childCount)
        {
            layers.GetChild(currentLayer - 1).gameObject.SetActive(true); // Hide current layer
            // Move to the previous layer
            currentLayer--;
        }
    }
    public void ResetCurrent()
    {
        currentLayer = 0;
    }
}
