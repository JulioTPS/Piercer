using UnityEngine;

public class testsetter : MonoBehaviour
{
    [SerializeField] public TestScript testList;
    void Start()
    {
        Debug.Log("Test List Count: " + testList.listColors.Count);
        foreach (var colorPair in testList.listColors)
        {
            Debug.Log($"Name: {colorPair.name}, Color: {colorPair.color}");
        }
    }

    void Update()
    {

    }
}
