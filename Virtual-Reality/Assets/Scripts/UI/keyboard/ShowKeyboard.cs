using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x => openKeyboard());
    }

    // Update is called once per frame
    public void openKeyboard()
    {
        NonNativeKeyboard.Instance.InputField= inputField;
        NonNativeKeyboard.Instance.PresentKeyboard();
    }
}
