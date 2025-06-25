using UnityEngine;

public class Block : MonoBehaviour
{
    private MeshRenderer mr;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mpb ??= new MaterialPropertyBlock();
    }

    public void SetColor(Color color)
    {
        // Debug.Log($"SetColor called on {gameObject.name} with color {color}");
        mr.GetPropertyBlock(mpb);
        mpb.SetColor("_color", color);
        mr.SetPropertyBlock(mpb);
    }
}
