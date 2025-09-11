//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using FuckingEndpoints;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownController : MonoBehaviour
{
    [SerializeField] protected Endpoints fuckingEndpoints;
    [SerializeField] protected TMP_Dropdown siteDropdown, sectionDropdown;
    [SerializeField] protected Transform section;
    [SerializeField] protected ArtefactGen artefactGen;
    [SerializeField] protected VisibilityControls visibilityControls;
    private void OnEnable()
    {
        siteDropdown.options = new()
        {
            new("Sites")
        };
        sectionDropdown.options = new()
        {
            new("Sections")
        };
        fuckingEndpoints.FetchSites();
    }
    public void OnSiteChosen()
    {
        if (siteDropdown.options.Count > 0)
        {
            try
            {
                fuckingEndpoints.FetchSections(fuckingEndpoints.Sites[siteDropdown.value].Id);
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load sections: {e.Message}.");
            }
        }
    }
    public void SetSiteOptions(List<Site> sites)
    {
        var options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < sites.Count; i++)
        {
            options.Add(new(sites[i].Name));
        }
        siteDropdown.options = options;
    }
    public void OnSectionChosen()
    {
        if (sectionDropdown.options.Count > 0)
        {
            var index = sectionDropdown.value;
            try
            {
                fuckingEndpoints.FetchArtifacts(fuckingEndpoints.Sections[index].Id);
                //also load the section thingy
                var chosenSection = fuckingEndpoints.Sections[index];
                section.localScale = new Vector3(chosenSection.EWWidth, chosenSection.NSLength, chosenSection.Depth);
                visibilityControls.OnSectionSelected();
                //also generate artifacts
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load section: {e.Message}.");
            }
        }
    }
    public void SetSectionOptions(List<Section> sections)
    {
        var options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < sections.Count; i++)
        {
            options.Add(new(sections[i].Name));
        }
        sectionDropdown.options = options;
    }
    public void OnArtifactsFetched(List<Artifact> artifacts)
    {
        artefactGen.CreateArtefacts(artifacts);
    }
}
