using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomGrabInteractable : XRGrabInteractable
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    protected override void OnEnable()
    {
        base.OnEnable();
        // Save the initial position and rotation of the
        // object at the start of the program
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ResetObject();
        Debug.Log(transform.root);
    }
    public void ResetObject()
    {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }
}