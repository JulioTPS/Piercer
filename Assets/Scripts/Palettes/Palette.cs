using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Palette Template", menuName = "ScriptableObjects/Palette Template")]
public class Palette : ScriptableObject
{
    public Color DefaultColor;
    public Color PieceI;
    public Color PieceJ;
    public Color PieceL;
    public Color PieceO;
    public Color PieceS;
    public Color PieceT;
    public Color PieceZ;
    public Color Arena;
    public Color BackgroundMiddle;
    public Color BackgroundAbove;
    public Color BackgroundBelow;

    private static FieldInfo[] _colorFields;

    static Palette()
    {
        _colorFields = typeof(Palette).GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    public Dictionary<string, Color> GetColorsDictionary()
    {
        Dictionary<string, Color> colorsDictionary = new();

        foreach (FieldInfo field in _colorFields)
        {
            colorsDictionary[field.Name] = (Color)field.GetValue(this);
        }

        return colorsDictionary;
    }
}
