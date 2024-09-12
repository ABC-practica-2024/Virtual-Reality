using System.Collections.Generic;
using UnityEngine;

public class LayerGenerator : MonoBehaviour
{
    [SerializeField] protected Transform layerHolder;
    [SerializeField] protected Transform layerPrefab;
    [SerializeField] protected Transform layerPoolObject;
    [SerializeField] protected XRLayerControl layerControl;
    [SerializeField] protected Queue<Transform> layerPool = new();
    public Vector3 size;
    public bool b;
    protected void OnEnable()
    {
        GenerateLayers();
    }
    public void Update()
    {
        if (b)
        {
            b = false;
            DeactivateLayers();
            layerHolder.root.localScale = size;
            GenerateLayers();
        }
    }
    public void GenerateLayers()
    {
        //Debug.Log("Initial nr of children: " + layerHolder.childCount);
        layerControl.ResetCurrent();
        int nr_of_layers = (int)(layerHolder.parent.localScale.y * 50) - 1;
        float layer_height = layerHolder.GetChild(0).position.y - .2f;
        layerHolder.GetChild(0).gameObject.SetActive(true);
        //Debug.Log("Nr of layers: " + nr_of_layers);
        for (int i = 0; i < nr_of_layers; i++)
        {
            if (layerPool.Count > 0)
            {
                //we have an available layer in the pool
                var layer = layerPool.Dequeue();
                layer.gameObject.SetActive(true);
                layer.parent = layerHolder;
                layer.position = new Vector3(layerHolder.position.x, layer_height,
                    layerHolder.position.z);
                //rotation should not change
            }
            else
            {
                Instantiate(layerPrefab, new Vector3(layerHolder.position.x,
                    layer_height, layerHolder.position.z), layerHolder.rotation,
                    layerHolder);
            }
            layer_height -= .2f;
        }
        //Debug.Log("nr of children: " + layerHolder.childCount);
    }
    public void DeactivateLayers()
    {
        //we need to save the child count and go in reverse, otherwise the child
        //count and order of children in the children array will change while we
        //are still in the for loop
        var childCount = layerHolder.childCount;
        //Debug.Log("Child count before deletion: " + childCount);
        for (int i = childCount - 1; i > 0; i--)
        {
            var layer = layerHolder.GetChild(i);
            layer.gameObject.SetActive(false);
            layer.parent = layerPoolObject;
            layerPool.Enqueue(layer);
        }
        //Debug.Log("Child count after deletion: " + layerHolder.childCount);
    }
}
