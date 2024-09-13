using UnityEngine;
using TMPro; // Import the TextMeshPro namespace
using System.Collections.Generic;
using UnityEngine.Android;

public class TMPDropdownManager : MonoBehaviour
{
    // Public fields to assign in the Inspector
    public TMP_Dropdown dropdown1;
    public TMP_Dropdown dropdown2;
    public TMP_InputField filterInput; // New TMP_InputField for filtering
    public GameObject parentObject;

    private List<Transform> childTransforms = new List<Transform>();
    private List<string> currentNephewOptions = new List<string>(); // Store the current nephew options
    public string SHOW_ALL_OPTION = "Show all";

    void Start()
    {
        Debug.Log("Implement a load method, that reads the required data from a json");
        // Initialize dropdown1 with the direct children of parentObject
        InitializeDropdown1();

        // Add listener for dropdown1 selection change
        dropdown1.onValueChanged.AddListener(delegate { OnDropdown1ValueChanged(dropdown1); });

        // Add listener for input field changes
        filterInput.onValueChanged.AddListener(delegate { OnFilterInputChanged(filterInput); });

        // Add listener for dropdown2 selection change
        dropdown2.onValueChanged.AddListener(delegate { OnDropdown2ValueChanged(dropdown2); });
    }

    void InitializeDropdown1()
    {
        if (parentObject != null)
        {
            // Clear previous options
            dropdown1.ClearOptions();

            // Get all child objects of parentObject
            Transform[] children = parentObject.GetComponentsInChildren<Transform>(true);

            // List to store names of direct children
            List<string> childOptions = new List<string> { SHOW_ALL_OPTION }; // Add "Show all" option first

            // Populate childTransforms and childOptions lists
            foreach (Transform child in children)
            {
                if (child.parent == parentObject.transform)
                {
                    // Direct child of the parentObject
                    childTransforms.Add(child);
                    childOptions.Add(child.gameObject.name);
                }
            }

            // Populate dropdown1 with the names of direct children and "Show all"
            dropdown1.AddOptions(childOptions);

            // Initialize dropdown2 to show all nephew objects by default
            UpdateDropdown2(null);
        }
        else
        {
            Debug.LogError("Parent Object is not assigned!");
        }
    }

    // Update dropdown2 based on the selected child object
    void UpdateDropdown2(Transform selectedChild)
    {
        currentNephewOptions.Clear(); // Clear current options list

        if (selectedChild == null)
        {
            // "Show all" is selected or dropdown2 needs to show all options
            foreach (Transform child in childTransforms)
            {
                foreach (Transform grandchild in child)
                {
                    currentNephewOptions.Add(grandchild.gameObject.name);
                }
            }
        }
        else
        {
            // Specific child is selected
            foreach (Transform grandchild in selectedChild)
            {
                currentNephewOptions.Add(grandchild.gameObject.name);
            }
        }

        // Apply the current filter to update dropdown2
        ApplyFilter();
    }

    // Listener method called when dropdown1's value changes
    void OnDropdown1ValueChanged(TMP_Dropdown change)
    {
        int selectedIndex = change.value;

        if (selectedIndex == 0)
        {
            // "Show all" option selected
            UpdateDropdown2(null);
        }
        else if (selectedIndex <= childTransforms.Count)
        {
            // Update dropdown2 based on the new selection in dropdown1
            UpdateDropdown2(childTransforms[selectedIndex - 1]);
        }
    }

    // Listener method called when dropdown2's value changes (does nothing)
    void OnDropdown2ValueChanged(TMP_Dropdown change)
    {
        /* Empty Method
         * Activates when dropdown2 value changes
         * TODO
         * Destroy currently generated Section
         * Construct the new selected Section
         */
        Debug.Log("Implement section destruction and construction");
    }

    // Listener method called when filter input field changes
    void OnFilterInputChanged(TMP_InputField input)
    {
        // Apply the filter based on the input text
        ApplyFilter();
    }

    // Method to apply the current filter to dropdown2
    void ApplyFilter()
    {
        string filterText = filterInput.text.ToLower(); // Get the text from the input field and convert to lowercase
        List<string> filteredOptions = new List<string>();

        // Filter options based on input field text
        foreach (string option in currentNephewOptions)
        {
            if (option.ToLower().Contains(filterText))
            {
                filteredOptions.Add(option);
            }
        }

        // Update dropdown2 with the filtered options
        dropdown2.ClearOptions();
        dropdown2.AddOptions(filteredOptions);

        // Refresh dropdown2 if it's currently open
        if (dropdown2.gameObject.activeInHierarchy && dropdown2.transform.childCount > 0)
        {
            StartCoroutine(RefreshDropdown(dropdown2));
        }
    }

    bool IsDropdownExpanded(TMP_Dropdown dropdown)
    {
        // TMP_Dropdown has a dropdownList GameObject when expanded
        if (dropdown.transform.childCount > 0)
        {
            Transform dropdownList = dropdown.transform.Find("Dropdown List");
            if (dropdownList != null)
            {
                return dropdownList.gameObject.activeSelf; // Return true if the dropdown is expanded
            }
        }
        return false; // Dropdown is not expanded
    }

    // Coroutine to refresh TMP_Dropdown (closes and reopens to refresh options)
    System.Collections.IEnumerator RefreshDropdown(TMP_Dropdown dropdown)
    {
        // Wait until the end of the frame to avoid UI update conflicts
        yield return new WaitForEndOfFrame();

        // Only refresh and reopen the dropdown if it's currently expanded
        if (IsDropdownExpanded(dropdown))
        {
            dropdown.Hide();
            yield return new WaitForEndOfFrame(); // Wait for another frame to ensure it's properly hidden
            dropdown.Show();
        }
    }
}
