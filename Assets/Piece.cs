using UnityEngine;

public class Piece : MonoBehaviour
{
    public Color blockColor;
    public char type;

    void Start()
    {
        foreach (Block block in GetComponentsInChildren<Block>())
        {
            block.SetColor(blockColor);
        }
    }
}
