using UnityEngine;

public class Block : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        materialPropertyBlock ??= new MaterialPropertyBlock();
    }

    public void SetColor(Color color)
    {
        skinnedMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_color", color);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
