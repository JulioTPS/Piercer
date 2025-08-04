using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public enum PaletteEnum
{
    DefaultColor,
    PieceI,
    PieceJ,
    PieceL,
    PieceO,
    PieceS,
    PieceT,
    PieceZ,
    Arena,
    BackgroundMiddle,
    BackgroundAbove,
    BackgroundBelow,
}

[CreateAssetMenu(fileName = "Palette Template", menuName = "Scriptable Objects/Palette Template")]
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

    private Dictionary<PaletteEnum, Color> colorsDictionary;

    private void OnEnable()
    {
        colorsDictionary = new Dictionary<PaletteEnum, Color>();
        FieldInfo[] colorFields = typeof(Palette).GetFields(
            BindingFlags.Public | BindingFlags.Instance
        );
        PaletteEnum[] paletteEnums = (PaletteEnum[])Enum.GetValues(typeof(PaletteEnum));

        int colorFieldsLength = colorFields.Length;
        for (int i = 0; i < colorFieldsLength; i++)
        {
            if (colorFields[i].Name != paletteEnums[i].ToString())
            {
                Debug.LogError(
                    $"Palette field '{colorFields[i].Name}' does not match PaletteEnum '{paletteEnums[i]}'. Please ensure they are in sync."
                );
            }
            colorsDictionary[paletteEnums[i]] = (Color)colorFields[i].GetValue(this);
        }
    }

    public Dictionary<PaletteEnum, Color> GetColorsDictionary()
    {
        if (colorsDictionary == null)
        {
            OnEnable();
        }
        return colorsDictionary;
    }
}
