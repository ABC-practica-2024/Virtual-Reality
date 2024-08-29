using UnityEngine.XR.Interaction.Toolkit;

public class ButtonBase : XRBaseInteractable
{
    protected System.Action action;
    public void AddAction(System.Action action)
    {
        this.action += action;
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        action();
    }
}
