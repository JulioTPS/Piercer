using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class PaletteManager : MonoBehaviour
{
    [SerializeField]
    public Palette palette;
    public static PaletteManager Instance;

    public static System.Action OnInstanceReady;

    private Dictionary<PaletteEnum, Color> colorsDictionary;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeColors();
                OnInstanceReady?.Invoke();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Instance = this;
            InitializeColors();
        }
    }

    private void InitializeColors()
    {
        if (palette != null)
        {
            colorsDictionary = palette.GetColorsDictionary();
        }
    }

    public Color GetColor(PaletteEnum colorName)
    {
        if (colorsDictionary.TryGetValue(colorName, out var color))
        {
            return color;
        }
        Debug.LogWarning($"Color '{colorName}' not found in palette.");
        return Color.magenta;
    }

    public void ResetSingleton()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }
}
