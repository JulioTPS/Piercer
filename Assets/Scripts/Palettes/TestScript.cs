using System.Collections.Generic;
using UnityEngine;

public struct ColorPair
{
    public string name;
    public Color color;
}

[CreateAssetMenu(fileName = "TestScript", menuName = "ScriptableObjects/TestScript")]
public class TestScript : ScriptableObject
{
    [System.Serializable]
    public struct ColorPair
    {
        public string name;
        public Color color;
    }
    public List<ColorPair> listColors;
}
