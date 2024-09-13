using UnityEngine;

public class LayerGenerator : MonoBehaviour
{
    [SerializeField] protected Transform LayerHolder;
    [SerializeField] protected Transform Layer;
    private void OnEnable()
    {
        GenerateLayers();
    }
    public void GenerateLayers()
    {
        int nr_of_layers = (int)LayerHolder.parent.localScale.y * 50 - 1;
        float layer_height = LayerHolder.GetChild(0).position.y - .2f;
        for (int i = 0; i < nr_of_layers; i++)
        {
            Instantiate(Layer, new Vector3(LayerHolder.position.x, layer_height,
                LayerHolder.position.z), LayerHolder.rotation, LayerHolder);
            layer_height -= .2f;
        }
    }
    public void DestroyLayers()
    {
        for (int i = 1; i < LayerHolder.childCount; i++)
        {
            Destroy(LayerHolder.GetChild(i));
        }
    }
}
