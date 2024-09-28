using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

public class TMPDropdownManager : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown siteDropdown; // Assign in the Inspector
    public TMP_Dropdown sectionDropdown; // Assign in the Inspector

    [Header("Game Objects")]
    public Transform Planes; // Change to Transform to directly manipulate its scale
    public GameObject sampleArtifact; // Prefab reference for artifacts

    private List<Site> sites = new List<Site>();
    private List<Section> sections = new List<Section>(); // New list to hold sections
    private List<GameObject> loadedArtifacts = new List<GameObject>(); // List to keep track of instantiated artifacts

    private string loginUrl = "http://pd-structuri.ro:8081/api/v1/auth/login";
    private string siteUrl = "http://pd-structuri.ro:8081/api/arheo/site";
    private string authToken = "";

    // Classes for API responses
    [System.Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    private class SiteResponse
    {
        public bool flag;
        public int code;
        public string message;
        public SiteData data;
    }

    [System.Serializable]
    private class SiteData
    {
        public List<Site> sites;
    }

    [System.Serializable]
    public class Site
    {
        public int id;          // ID for requesting sections
        public string title;    // Name for displaying in the dropdown
    }

    [System.Serializable]
    private class SectionResponse
    {
        public bool flag;
        public int code;
        public string message;
        public SectionData data;
    }

    [System.Serializable]
    private class SectionData
    {
        public List<Section> sites; // Must remain 'sites' as per requirements
    }

    [System.Serializable]
    public class Section
    {
        public int id;
        public string name;
    }

    [System.Serializable]
    public class SectionDetails
    {
        public bool flag;
        public int code;
        public string message;
        public SectionDataDetails data;
    }

    [System.Serializable]
    public class SectionDataDetails
    {
        public Dimensions dimensions;
        public string status;
        public int siteId;
    }

    [System.Serializable]
    public class Dimensions
    {
        public float length; // X
        public float width;  // Z
        public float depth;  // Y
    }

    [System.Serializable]
    private class ArtifactsResponse
    {
        public bool flag;
        public int code;
        public string message;
        public List<Artifact> data; // Changed to hold a list of artifacts
    }

    [System.Serializable]
    public class Artifact
    {
        public int id;
        public Dimensions artifactDimension;
        public Position artifactPosition;
        public Rotation artifactRotation;
        public string label;
        public string category;
    }

    [System.Serializable]
    public class Position
    {
        public float latitude;
        public float longitude;
        public float depth;
    }

    [System.Serializable]
    public class Rotation
    {
        public float pitch;
        public float yaw;
        public float roll;
    }

    void Start()
    {
        StartCoroutine(LoginAndFetchSites());
        siteDropdown.onValueChanged.AddListener(OnSiteDropdownValueChanged);
        sectionDropdown.onValueChanged.AddListener(OnSectionSelected);
    }

    IEnumerator LoginAndFetchSites()
    {
        yield return StartCoroutine(AttemptLogin());
        if (!string.IsNullOrEmpty(authToken))
        {
            yield return StartCoroutine(FetchSites());
        }
    }

    IEnumerator AttemptLogin()
    {
        LoginRequest loginRequest = new LoginRequest { username = "catalin1", password = "catalin1" };
        string jsonLogin = JsonUtility.ToJson(loginRequest);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonLogin);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
            {
                var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                if (responseJson.TryGetValue("data", out object dataObj))
                {
                    authToken = dataObj is string token ? token : ((JObject)dataObj).Value<string>("token");
                }
            }
            else
            {
                Debug.LogError("Login failed. Error: " + request.error);
            }
        }
    }

    IEnumerator FetchSites()
    {
        siteDropdown.ClearOptions();
        siteDropdown.AddOptions(new List<string> { "Attempting to access server..." });

        string url = $"{siteUrl}?page=0&size=10";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
            {
                var response = JsonConvert.DeserializeObject<SiteResponse>(request.downloadHandler.text);
                if (response.flag && response.data.sites != null)
                {
                    sites = response.data.sites;
                    PopulateSiteDropdown();
                }
                else
                {
                    Debug.LogError("Failed to fetch sites.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch sites. Status code: " + request.responseCode);
            }
        }
    }

    void PopulateSiteDropdown()
    {
        siteDropdown.ClearOptions();

        List<string> siteNames = new List<string> { "Choose a site" }; // Add a default option
        siteNames.AddRange(sites.Select(site => site.title));
        siteDropdown.AddOptions(siteNames);
    }

    void OnSiteDropdownValueChanged(int index)
    {
        if (index > 0) // Skip the default option
        {
            var selectedSite = sites[index - 1]; // Adjust for the default option
            StartCoroutine(FetchSectionsBySite(selectedSite.id));
        }
        else
        {
            sectionDropdown.ClearOptions();
        }
    }

    IEnumerator FetchSectionsBySite(int siteId)
    {
        string url = $"http://pd-structuri.ro:8081/api/arheo/site/sections?siteId={siteId}&page=0&pageSize=10";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
            {
                var sectionResponse = JsonConvert.DeserializeObject<SectionResponse>(request.downloadHandler.text);
                if (sectionResponse.data.sites != null && sectionResponse.data.sites.Count > 0)
                {
                    sections = sectionResponse.data.sites; // Store sections here
                    PopulateSectionDropdown(sections);
                }
                else
                {
                    Debug.LogError("No sections found for this site.");
                    sectionDropdown.ClearOptions();
                    sectionDropdown.AddOptions(new List<string> { "No sections found" });
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch sections. Error: {request.error}");
                sectionDropdown.ClearOptions();
                sectionDropdown.AddOptions(new List<string> { "Error fetching sections" });
            }
        }
    }

    void PopulateSectionDropdown(List<Section> sections)
    {
        sectionDropdown.ClearOptions();
        List<string> sectionNames = sections.Select(section => section.name).ToList();
        sectionDropdown.AddOptions(new List<string> { "Select a section" }); // Add a default option
        sectionDropdown.AddOptions(sectionNames);
    }

    public void OnSectionSelected(int index)
    {
        if (index > 0) // Skip the default option
        {
            int selectedSectionId = sections[index - 1].id; // Adjust for the default option
            StartCoroutine(LoadArtifactsForSection(selectedSectionId));
        }
    }

    IEnumerator LoadArtifactsForSection(int sectionId)
    {
        // Destroy old artifacts
        DestroyOldArtifacts();

        // Fetch new artifacts
        string url = $"http://pd-structuri.ro:8081/api/arheo/sections/{sectionId}/artifacts";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
            {
                var artifactResponse = JsonConvert.DeserializeObject<ArtifactsResponse>(request.downloadHandler.text);
                if (artifactResponse.data != null && artifactResponse.data.Count > 0)
                {
                    foreach (var artifact in artifactResponse.data)
                    {
                        InstantiateArtifact(artifact);
                    }
                }
                else
                {
                    Debug.LogError("No artifacts found for this section.");
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch artifacts. Error: {request.error}");
            }
        }
    }

    void DestroyOldArtifacts()
    {
        foreach (var artifact in loadedArtifacts)
        {
            Destroy(artifact);
        }
        loadedArtifacts.Clear(); // Clear the list after destroying old artifacts
    }

    void InstantiateArtifact(Artifact artifact)
    {
        // Create a new artifact instance
        GameObject newArtifact = Instantiate(sampleArtifact);
        newArtifact.transform.position = new Vector3(artifact.artifactPosition.longitude, artifact.artifactPosition.depth, artifact.artifactPosition.latitude); // Set position
        newArtifact.transform.rotation = Quaternion.Euler(artifact.artifactRotation.pitch, artifact.artifactRotation.yaw, artifact.artifactRotation.roll); // Set rotation

        // Optionally, you can set additional properties for the new artifact
        // For example, set the name or any other component data
        newArtifact.name = artifact.label;

        loadedArtifacts.Add(newArtifact); // Keep track of loaded artifacts
    }
}
