using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
public class Colors : MonoBehaviour
{
    public List<string> color = new List<string>();
    int x;
    //public string[] color = new string[num];
    public void addString(string abs)
    {
        color.Add(abs);
    }
}
