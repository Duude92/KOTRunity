using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

public class Materials : MonoBehaviour
{
    public List<string> material = new List<string>();
    public List<Material> maths = new List<Material>();

    public void addMat(string abs, AssetImportContext rootObject = null)
    {
        material.Add(abs);
        string[] spl = abs.Split(' ');
        var ma = new Material(SettingManager.DefaultShader);
        List<Color32> colors = GameManager.common?.GetComponent<Palettefiles>()?.colors;

        switch (spl[1])
        {
            case "col":
                {
                    ma.color = (colors != null) ? colors[int.Parse(spl[2]) - 1] : new Color32();
                    break;
                }
            case "tex":
                {
                    var tex = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2]) - 1];
                    ma = new Material(SettingManager.DefaultShader);
                    ma.mainTexture = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2]) - 1];
                    break;
                }
            case "ttx":
                {
                    var tex = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2]) - 1];

                    ma = new Material(SettingManager.DefaultShader);
                    ma.mainTexture = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2]) - 1];
                    //Debug.LogWarning(spl[1]+", "+spl[4]);
                    if (spl.Length > 4)
                        if (spl[4] == "col")
                        {
                            //Debug.LogWarning(spl[5]);
                            ma.SetColor("_TransparentColor", (colors != null) ? colors[int.Parse(spl[2]) - 1] : new Color32());
                            //ma.SetColor("_TransparentColor",new Color32(255,255,255,255));
                        }
                    break;
                }
            default:
                {
                    break;
                }
        }
        ma.name = abs;
        if (rootObject != null)
        {
            rootObject.AddObjectToAsset($"{spl[0]}.mat", ma);
        }

        maths.Add(ma);

        //ma.mainTexture = int.Parse(spl[1])
    }
    public int FindIndexByString(string name)
    {
        int a = 65535;
        int i = 0;
        List<string> material1 = gameObject.GetComponent<Materials>().material;
        foreach (string mat in material1)
        {
            //Debug.Log(name.Replace("\0",string.Empty));
            //Debug.LogWarning(mat.Split(' ')[0]);
            if (name.Replace("\0", string.Empty) == mat.Split(' ')[0])
            {
                a = i;
            }
            i++;
        }
        return a;
    }
}
