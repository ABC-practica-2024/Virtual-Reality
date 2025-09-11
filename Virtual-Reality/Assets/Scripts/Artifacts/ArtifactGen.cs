using FuckingEndpoints;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactGen : MonoBehaviour
{
    [SerializeField] protected GameObject defaultModel;
    public void CreateArtefacts(List<Artifact> artifacts)
    {
        for (int i = 0; i < artifacts.Count; i++)
        {
            CreateArtefact(defaultModel, new Vector3(artifacts[i].EWOffset, artifacts[i].DepthOffset, artifacts[i].NSOffset),
                Quaternion.Euler(artifacts[i].RotX, artifacts[i].RotY, artifacts[i].RotZ));
        }
    }
    public void CreateArtefact(GameObject model, Vector3 position, Quaternion rotation)
    {
        Instantiate(model, position, rotation, transform);
    }
    private void OnDisable()
    {
        DestroyArtefacts();
    }
    public void DestroyArtefacts()
    {
        int i = 0;
        while (i < transform.childCount)
        {
            Destroy(transform.GetChild(i).gameObject);
            i++;
        }
    }
}
