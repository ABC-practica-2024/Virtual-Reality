using UnityEngine;

public class ArtefactGen : MonoBehaviour
{
    public void CreateArtefact(GameObject model, Vector3 position, Quaternion rotation)
    {
        Instantiate(model, position, rotation, transform);
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
