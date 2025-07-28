using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ColorSetter : MonoBehaviour
{
    private Renderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    public PaletteEnum colorName = PaletteEnum.DefaultColor;
    private bool hasSetColor = false;
    void Awake()
    {
        meshRenderer = TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer) ? skinnedMeshRenderer : GetComponent<MeshRenderer>();
        materialPropertyBlock ??= new MaterialPropertyBlock();
    }

    void Start()
    {
        TryInitializeColor();
    }

    void OnEnable()
    {
        if (hasSetColor)
            return;
        PaletteManager.OnInstanceReady += OnPaletteManagerReady;
    }

    void OnDisable()
    {
        PaletteManager.OnInstanceReady -= OnPaletteManagerReady;
    }

    private void OnPaletteManagerReady()
    {
        TryInitializeColor();
    }

    private void TryInitializeColor()
    {
        if (PaletteManager.Instance != null)
        {
            Color color = PaletteManager.Instance.GetColor(colorName);
            SetColor(color);
            hasSetColor = true;
        }
    }

    public void SetColor(Color color)
    {
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_Color", PaletteManager.Instance.GetColor(colorName));
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}


