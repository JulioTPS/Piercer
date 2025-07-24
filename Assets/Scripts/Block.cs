using UnityEngine;

public class Block : MonoBehaviour
{
    private Renderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    void Awake()
    {
        meshRenderer = TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer) ? skinnedMeshRenderer : GetComponent<MeshRenderer>();

        materialPropertyBlock ??= new MaterialPropertyBlock();
    }

    public void SetColor(Color color)
    {
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_color", color);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
