using UnityEngine;

public class Piece : MonoBehaviour
{
    public Color blockColor;
    public char type;
    public bool isActive = true;

    void Start()
    {
        foreach (Block block in GetComponentsInChildren<Block>())
        {
            block.SetColor(blockColor);
        }
    }
}
