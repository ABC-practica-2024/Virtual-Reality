using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace FuckingEndpoints
{
    public class Endpoints : MonoBehaviour
    {
        #region Utils
        [SerializeField] protected string baseURL = "http://192.168.8.147:5146";
        protected string authToken;
        public UnityWebRequest CreateGet(string uri)
        {
            var r = UnityWebRequest.Get(baseURL + uri);
            r.SetRequestHeader("Authorization", $"Bearer {authToken}");
            return r;
        }
        #endregion
        #region Sites
        [SerializeField] protected List<Site> sites = new();
        public List<Site> Sites
        {
            get
            {
                return sites;
            }
            protected set
            {
                sites = value;
                SitesFetched?.Invoke(sites);
            }
        }
        public UnityEvent<List<Site>> SitesFetched;
        public void FetchSites()
        {
            StartCoroutine(TryGetSites());
        }
        public IEnumerator TryGetSites()
        {
            using (UnityWebRequest webRequest = CreateGet("/sites"))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest != null && webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        Sites = JsonConvert.DeserializeObject<List<Site>>
                        (webRequest.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error deserializing response: {e.Message}.");
                    }
                }
                else Debug.LogError($"Connection error: conection status {webRequest.result}.");
            }
        }
        #endregion
        #region Sections
        [SerializeField] protected List<Section> sections = new();
        public List<Section> Sections
        {
            get
            {
                return sections;
            }
            protected set
            {
                sections = value;
                SectionsFetched?.Invoke(sections);
            }
        }
        public UnityEvent<List<Section>> SectionsFetched;
        public void FetchSections(int siteID)
        {
            StartCoroutine(TryGetSections(siteID));
        }
        public IEnumerator TryGetSections(int siteID)
        {
            using (UnityWebRequest webRequest = CreateGet($"/sections/{siteID}"))
            {
                Debug.Log(siteID);
                yield return webRequest.SendWebRequest();
                if (webRequest != null && webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        Sections = JsonConvert.DeserializeObject<List<Section>>
                        (webRequest.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error deserializing response: {e.Message}.");
                    }
                }
                else Debug.LogError($"Connection error: conection status {webRequest.result}.");
            }
        }
        #region Artifacts
        public void FetchArtifacts(int sectionID)
        {
            StartCoroutine(TryFetchArtefacts(sectionID));
        }
        [SerializeField] protected List<Artifact> artifacts = new();
        public List<Artifact> Artefacts
        {
            get { return artifacts; }
            protected set
            {
                artifacts = value;
                ArtifactsFetched?.Invoke(artifacts);
            }
        }
        public UnityEvent<List<Artifact>> ArtifactsFetched;
        public IEnumerator TryFetchArtefacts(int sectionID)
        {
            using (UnityWebRequest webRequest = CreateGet($"/artefacts/{sectionID}"))
            {
                Debug.Log(sectionID);
                yield return webRequest.SendWebRequest();
                if (webRequest != null && webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        Artefacts = JsonConvert.DeserializeObject<List<Artifact>>
                        (webRequest.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error deserializing response: {e.Message}.");
                    }
                }
                else Debug.LogError($"Connection error: conection status {webRequest.result}.");
            }
        }
        #endregion
        #endregion
        private void OnEnable()
        {
            FetchSites();
        }
    }
}