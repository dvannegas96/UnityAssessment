using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public struct TwoStrings
{
    public string string1;
    public string string2;
    public string concatenated;
}

public class NativeCppLibraryIntegration : MonoBehaviour
{
    [DllImport("CppPlugin")]
    public static extern TwoStrings concatenate(TwoStrings parameter);
    // Start is called before the first frame update
    void Start()
    {
        TwoStrings parameter;
        parameter.string1 = "Hello";
        parameter.string2 = "World";
        parameter.concatenated = "";

        Debug.Log("Result: " + concatenate(parameter).concatenated);
    }

    // Update is called once per frame
    void Update()
    {

    }
}