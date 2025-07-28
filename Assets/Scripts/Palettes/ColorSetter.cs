using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ColorSetter : MonoBehaviour
{
    private Renderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    public string colorName = "DefaultColor";
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
        materialPropertyBlock.SetColor("_Color", PaletteManager.Instance.GetColor(colorName));
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}


