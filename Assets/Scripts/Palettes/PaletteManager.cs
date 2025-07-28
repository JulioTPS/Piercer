using System.Collections.Generic;
using UnityEngine;

public class PaletteManager : MonoBehaviour
{
    [SerializeField] public Palette palette;
    public static PaletteManager Instance;

    private Dictionary<string, Color> colorsDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            colorsDictionary = palette.GetColorsDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Color GetColor(string colorName)
    {
        if (colorsDictionary.TryGetValue(colorName, out var color))
        {
            return color;
        }
        Debug.LogWarning($"Color '{colorName}' not found in palette.");
        return Color.magenta;
    }
}
