using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class TMPDropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown1;
    public TMP_InputField filterInput;
    public TMP_Dropdown dropdown2;

    [SerializeField]
    private TextAsset fallbackSiteData; // Assign this in the Unity Inspector

    [SerializeField]
    private TextAsset fallbackSectionData; // Assign this in the Unity Inspector

    private List<Site> sites = new List<Site>();
    private Dictionary<int, Site> siteDictionary = new Dictionary<int, Site>();
    private List<string> siteNames = new List<string>();
    private List<Section> fallbackSections = new List<Section>();

    private string loginUrl = "http://pd-structuri.ro:8081/api/v1/auth/login";
    private string siteUrl = "http://pd-structuri.ro:8081/api/arheo/site";
    private string authToken = "";

    private bool isDefaultOptionActive = true;  // To track if "Choose a site" option is active

    [Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    // Adjusted API response class for `getSites`
    [Serializable]
    private class SiteResponse
    {
        public bool flag;
        public int code;
        public string message;
        public List<Site> data;  // Adjusted to hold a list of Site objects
    }

    [Serializable]
    public class Site
    {
        public int id;
        public string title;
        public string description;
        public Coordinate centralCoordinate;
        public List<PerimeterCoordinate> perimeterCoordinates;
        public string status;
        public int mainArchaeologistID;
        public List<int> sectionsIds;
        public List<int> archaeologistsIds;
    }

    [Serializable]
    private class SectionArrayResponse
    {
        public List<Section> sections; // Adjust to match your JSON structure
    }

    // Adjusted API response class for `getSections{ID}`
    [Serializable]
    private class SectionResponse
    {
        public bool flag;
        public int code;
        public string message;
        public Section data;
    }

    [Serializable]
    private class Section
    {
        public int id;
        public string name;
        public Coordinate southWest;
        public Coordinate northWest;
        public Coordinate northEast;
        public Coordinate southEast;
        public string status;
        public DateTime createdAt;
        public DateTime updatedAt;
        public int siteId;
        public List<int> artifactIds;
    }

    [Serializable]
    public class Coordinate
    {
        public float latitude;
        public float longitude;
    }

    [Serializable]
    public class PerimeterCoordinate : Coordinate
    {
        public int orderIndex;
    }

    void Start()
    {
        StartCoroutine(LoginAndFetchSites());

        // Listen for changes in the dropdowns
        filterInput.onValueChanged.AddListener(delegate { ApplyFilter(); });
        dropdown1.onValueChanged.AddListener(delegate { OnDropdown1ValueChanged(dropdown1.value); });

        // Load fallback section data
        if (fallbackSectionData != null)
        {
            ProcessFallbackSectionData(fallbackSectionData.text);
        }
    }

    IEnumerator LoginAndFetchSites()
    {
        yield return StartCoroutine(AttemptLogin());
        if (!string.IsNullOrEmpty(authToken))
        {
            yield return StartCoroutine(FetchSites()); // Fetch data from the server
        }
    }

    IEnumerator AttemptLogin()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            username = "catalin1",
            password = "catalin1"
        };
        string jsonLogin = JsonUtility.ToJson(loginRequest);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonLogin);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            ProcessLoginResponse(request);
        }
    }

    void ProcessLoginResponse(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            try
            {
                var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);

                if (responseJson != null)
                {
                    if (responseJson.TryGetValue("data", out object dataObj))
                    {
                        if (dataObj is string token)
                        {
                            authToken = token;
                            Debug.Log("Login successful. Token received.");
                        }
                        else if (dataObj is JObject dataJObject)
                        {
                            var tokenProp = dataJObject.Properties().FirstOrDefault(p => p.Name.ToLower().Contains("token"));
                            if (tokenProp != null)
                            {
                                authToken = tokenProp.Value.ToString();
                                Debug.Log("Login successful. Token extracted from data object.");
                            }
                            else
                            {
                                Debug.LogWarning("Login response contains a data object, but no token found.");
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(authToken))
                    {
                        Debug.LogError("Failed to extract auth token from response.");
                        Debug.Log($"Full response: {JsonConvert.SerializeObject(responseJson, Formatting.Indented)}");
                    }
                }
            }
            catch (JsonException e)
            {
                Debug.LogError($"Error parsing login response: {e.Message}");
                Debug.LogError($"Raw response: {request.downloadHandler.text}");
            }
        }
        else
        {
            Debug.LogError($"Login request failed. Status: {request.responseCode}, Error: {request.error}");
        }
    }

    IEnumerator FetchSectionsBySite(List<int> sectionIds)
    {
        List<string> sectionNames = new List<string>();

        foreach (int sectionId in sectionIds)
        {
            string url = $"http://pd-structuri.ro:8081/api/arheo/sections/{sectionId}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
                {
                    try
                    {
                        var sectionResponse = JsonConvert.DeserializeObject<SectionResponse>(request.downloadHandler.text);
                        if (sectionResponse != null && sectionResponse.data != null)
                        {
                            sectionNames.Add(sectionResponse.data.name);
                        }
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError($"Error parsing section response: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to fetch section ID: {sectionId}. Status: {request.responseCode}, Error: {request.error}");
                }
            }
        }

        if (sectionNames.Count > 0)
        {
            dropdown2.ClearOptions();
            dropdown2.AddOptions(sectionNames);
        }
        else
        {
            Debug.LogWarning("No sections found for the selected site.");
        }
    }

    IEnumerator FetchSites()
    {
        dropdown1.ClearOptions();
        dropdown1.AddOptions(new List<string> { "Attempting to access server..." });

        int page = 0;
        int size = 10;
        string url = $"{siteUrl}?page={page}&size={size}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<SiteResponse>(request.downloadHandler.text);
                    if (response.flag && response.data != null)
                    {
                        sites = response.data;
                        siteDictionary = sites.ToDictionary(site => site.id);
                        PopulateDropdown1();
                    }
                    else
                    {
                        Debug.LogWarning("API response was not successful.");
                        UseFallbackData();
                    }
                }
                catch (JsonException e)
                {
                    Debug.LogError($"Error deserializing response: {e.Message}");
                    UseFallbackData();
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch sites. Status code: {request.responseCode}");
                UseFallbackData();
            }
        }
    }

    void PopulateDropdown1()
    {
        dropdown1.ClearOptions();
        siteNames.Clear();

        // Add the default option "Choose a site"
        siteNames.Add("Choose a site");

        foreach (Site site in sites)
        {
            siteNames.Add(site.title);
        }

        dropdown1.AddOptions(siteNames);
        isDefaultOptionActive = true;
    }

    void OnDropdown1ValueChanged(int index)
    {
        Debug.Log($"Dropdown1 selected index: {index}, isDefaultActive: {isDefaultOptionActive}");

        if (index == 0)
        {
            // "Choose a site" is selected, clear dropdown2 options
            dropdown2.ClearOptions();
            isDefaultOptionActive = true; // Keep track that default option is active
        }
        else
        {
            // Retrieve the actual site ID and load sections by site ID
            string selectedSite = siteNames[index];
            var site = sites.FirstOrDefault(s => s.title == selectedSite);
            if (site != null)
            {
                StartCoroutine(FetchSectionsBySite(site.sectionsIds));
            }
        }
    }

    void UseFallbackData()
    {
        if (fallbackSiteData != null)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<SiteResponse>(fallbackSiteData.text);
                if (response.data != null)
                {
                    sites = response.data;
                    siteDictionary = sites.ToDictionary(site => site.id);
                    PopulateDropdown1();
                }
                else
                {
                    Debug.LogWarning("Failed to use fallback data. No sites found.");
                }
            }
            catch (JsonException e)
            {
                Debug.LogError($"Error parsing fallback data: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Fallback data is not provided.");
        }
    }

    void ApplyFilter()
    {
        string filterText = filterInput.text.ToLower();

        if (!string.IsNullOrEmpty(filterText))
        {
            var filteredOptions = siteNames.Where(siteName => siteName.ToLower().Contains(filterText)).ToList();
            dropdown1.ClearOptions();
            dropdown1.AddOptions(filteredOptions);
        }
        else
        {
            dropdown1.ClearOptions();
            dropdown1.AddOptions(siteNames);
        }
    }

    void ProcessFallbackSectionData(string jsonData)
    {
        try
        {
            var sectionArrayResponse = JsonConvert.DeserializeObject<SectionArrayResponse>(jsonData);
            if (sectionArrayResponse != null && sectionArrayResponse.sections != null)
            {
                fallbackSections = sectionArrayResponse.sections;
            }
        }
        catch (JsonException e)
        {
            Debug.LogError($"Error processing fallback section data: {e.Message}");
        }
    }
}
