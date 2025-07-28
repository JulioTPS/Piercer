using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class PaletteSettertmp : MonoBehaviour
{
    private Renderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    public PaletteEnum paletteEnum = PaletteEnum.Default;

    void Awake()
    {
        meshRenderer = TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer) ? skinnedMeshRenderer : GetComponent<MeshRenderer>();

        materialPropertyBlock ??= new MaterialPropertyBlock();
    }

    void Start()
    {
        SetColor();
    }

    public void SetColor()
    {
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_UV_index", (float)paletteEnum);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}


