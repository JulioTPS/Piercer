using UnityEngine;

public class SimpleColorSetter : MonoBehaviour
{
    public Color color;
    private Renderer meshRenderer;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mpb ??= new MaterialPropertyBlock();
    }

    void Start()
    {
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(mpb);
    }
}
