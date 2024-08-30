using UnityEngine;

public class ArtefactGen : MonoBehaviour
{
    [SerializeField] protected Transform artefactHolder;
    public void CreateArtefact(GameObject model, Vector3 position, Quaternion rotation)
    {
        Instantiate(model, position, rotation, artefactHolder);
    }
    public void DestroyArtefacts()
    {
        for (int i = 0; i < artefactHolder.childCount; i++)
        {
            Destroy(artefactHolder.GetChild(i).gameObject);
        }
    }
}
