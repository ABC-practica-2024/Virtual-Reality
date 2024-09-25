using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TMPDropdownManager : MonoBehaviour
{
    [Header("Scene objects")]
    [SerializeField] protected ArtefactGen artefactHolder;
    [SerializeField] protected Transform siteSection;

    [Header("UI stuff")]
    [SerializeField] protected TMP_Dropdown dropdown1;
    [SerializeField] protected TMP_InputField filterInput;
    [SerializeField] protected TMP_Dropdown dropdown2;

    private List<Site> sites = new List<Site>();
    private Dictionary<int, Site> siteDictionary = new Dictionary<int, Site>();
    private List<string> siteNames = new List<string>();

    private string loginUrl = "http://pd-structuri.ro:8081/api/v1/auth/login";
    private string siteUrl = "http://pd-structuri.ro:8081/api/arheo/site";
    private string authToken = "";

    private bool isDefaultOptionActive = true;  // To track if "Choose a site" option is active

    void Start()
    {
        StartCoroutine(LoginAndFetchSites());

        // Listen for changes in the dropdowns
        filterInput.onValueChanged.AddListener(delegate { ApplyFilter(); });
        dropdown1.onValueChanged.AddListener(delegate { OnDropdown1ValueChanged(dropdown1.value); });
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
                    }
                }
                catch (JsonException e)
                {
                    Debug.LogError($"Error deserializing response: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch sites. Status code: {request.responseCode}");
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
}
