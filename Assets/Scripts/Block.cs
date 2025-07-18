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
        mr.GetPropertyBlock(mpb);
        mpb.SetColor("_color", color);
        mr.SetPropertyBlock(mpb);
    }
}
