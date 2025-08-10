using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SimpleColorSetter : MonoBehaviour
{
    public Color color;
    private Renderer[] meshRenderers;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        mpb ??= new MaterialPropertyBlock();
    }

    void Start()
    {
        meshRenderers = GetValidChildRenderers(transform.GetComponent<Renderer>()).ToArray();
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor", color);
            meshRenderer.SetPropertyBlock(mpb);
        }
    }

    public List<Renderer> GetValidChildRenderers(Renderer currentRenderer)
    {
        List<Renderer> renderers = new List<Renderer>();
        int childCount = currentRenderer.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            SimpleColorSetter childComponent = currentRenderer
                .transform.GetChild(i)
                .GetComponent<SimpleColorSetter>();
            if (childComponent != null)
                continue;

            Renderer childRenderer = childComponent.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                renderers.Add(childRenderer);
            }

            renderers.AddRange(childComponent.GetValidChildRenderers(childRenderer));
        }
        return renderers;
    }
}
