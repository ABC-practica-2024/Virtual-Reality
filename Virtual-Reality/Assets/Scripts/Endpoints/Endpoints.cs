using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FuckingEndpoints
{
    public class Endpoints : MonoBehaviour
    {
        [field: SerializeField] public List<Site> sites { get; protected set; } = new();
        [field: SerializeField] public List<Section> sections { get; protected set; } = new();
        #region Utils
        protected string baseURL = "http://10.219.177.122:7112";
        protected string authToken;
        public UnityWebRequest CreateGet(string uri)
        {
            var r = UnityWebRequest.Get(baseURL + uri);
            r.SetRequestHeader("Authorization", $"Bearer {authToken}");
            return r;
        }
        #endregion
        #region Sites
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
                        sites = JsonConvert.DeserializeObject<List<Site>>
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
        public void FetchSections(int siteID)
        {
            StartCoroutine(TryGetSections(siteID));
        }
        public IEnumerator TryGetSections(int siteID)
        {
            yield return null;
        }
        #endregion
        private void OnEnable()
        {
            FetchSites();
        }
    }
}