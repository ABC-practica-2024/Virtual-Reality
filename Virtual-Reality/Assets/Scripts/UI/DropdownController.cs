//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using FuckingEndpoints;
using TMPro;
using UnityEngine;

public class DropdownController : MonoBehaviour
{
    protected Endpoints fuckingEndpoints;
    [SerializeField] protected TMP_Dropdown siteDropdown, sectionDropdown;
    protected void Awake()
    {
        fuckingEndpoints = GetComponent<Endpoints>();
        fuckingEndpoints.FetchSites();
    }
    public void OnSiteChosen(int index)
    {
        fuckingEndpoints.FetchSections(fuckingEndpoints.sites[index].id);
    }
    public void OnSectionChosen(int index)
    {

    }
}
