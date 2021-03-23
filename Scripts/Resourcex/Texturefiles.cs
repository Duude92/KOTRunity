using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

public class Texturefiles : MonoBehaviour
{
    public List<string> Texturenames = new List<string>();
    public List<Texture2D> textures = new List<Texture2D>();
    //public Dictionary<string,Texture2D> textures = new Dictionary<string,Texture2D>();
    public void addTex(string name, short width, short height, byte[] rawIm, int format, AssetImportContext rootObject = null)
    {
        TextureFormat tf;
        bool r8 = false;
        byte[] raw2 = null;
        switch (format)
        {
            case 248:
                {
                    tf = TextureFormat.RGB565;
                    break;
                }
            case 15:
                {
                    //rawIm = Array
                    tf = TextureFormat.ARGB4444;
                    break;
                }
            case 0:
                {
                    tf = TextureFormat.RGB24;
                    int pos = 18;
                    //byte [] colors = new byte[768];
                    ColorsRGB[] colors = new ColorsRGB[256];

                    for (int i = 0; i < 256; i++)
                    {
                        colors[i].R = rawIm[pos];
                        colors[i].G = rawIm[pos + 1];
                        colors[i].B = rawIm[pos + 2];
                        pos += 3;
                    }
                    raw2 = new byte[width * height * 3];
                    for (int i = 0; i < width * height; i++)
                    {
                        raw2[i * 3] = colors[rawIm[pos]].B;
                        raw2[i * 3 + 1] = colors[rawIm[pos]].G;
                        raw2[i * 3 + 2] = colors[rawIm[pos]].R;
                        pos++;
                    }

                    r8 = true;
                    break;
                }
            default:
                {
                    Debug.LogError("Unknown texture type: " + format + ",  " + name);
                    tf = TextureFormat.RGB565;
                    break;
                }
        }
        var tex = new Texture2D(width, height, tf, false);
        tex.name = name;



        if (r8)
        {
            byte[] row = new byte[width * 3];
            int pos = 0;
            for (int i = 0; i < height; i++)
            {
                System.Array.Reverse(raw2, pos, width * 3);
                pos += width * 3;
            }
            System.Array.Reverse(raw2);
            tex.LoadRawTextureData(raw2);

        }
        else
        {
            tex.LoadRawTextureData(rawIm);
        }
#if UNITY_EDITOR
        tex.alphaIsTransparency = true;
#endif
        tex.Apply();
        if (rootObject != null)
        {
            rootObject.AddObjectToAsset(name, tex);
        }
        Texturenames.Add(name);
        textures.Add(tex);
    }
    [MenuItem("Tools/Export Textures")]
    public static void TextureFilesExport()
    {
        GameObject target = GameManager.currentObject;
        Texturefiles tf = target.GetComponent<Texturefiles>();
        if (tf)
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("Выберите папку для сохранения", path, path);

            for (int i = 0; i < tf.Texturenames.Count; i++)
            {
                Texture2D texture = tf.textures[i];
                string textureName = tf.Texturenames[i];
                textureName = textureName.Split('\\')[1];
                textureName = textureName.Split('.')[0];
                textureName = "\\" + textureName + ".png";
                BinaryWriter binaryWriter = new BinaryWriter(File.Create(path + textureName));
                byte[] data = texture.EncodeToPNG();
                binaryWriter.Write(data);
                binaryWriter.Close();

            }
        }
        Debug.Log("Txr successfully exported");
    }
    [MenuItem("Tools/Export Masks")]
    public static void MaskFilesExport() //TODO: да, да, DRY, похеру, на скорую руку можно и так
    {
        GameObject target = GameManager.currentObject;
        Maskfiles tf = target.GetComponent<Maskfiles>();
        if (tf)
        {
            string path = "";
            path = EditorUtility.OpenFolderPanel("Выберите папку для сохранения", path, path);

            for (int i = 0; i < tf.masks.Count; i++)
            {
                Texture2D texture = tf.textures[i];
                string textureName = tf.masks[i];
                textureName = textureName.Split('\\')[1];
                textureName = textureName.Split('.')[0];
                textureName = "\\" + textureName + ".png";
                BinaryWriter binaryWriter = new BinaryWriter(File.Create(path + textureName));
                byte[] data = texture.EncodeToPNG();
                binaryWriter.Write(data);
                binaryWriter.Close();

            }
        }
        Debug.Log("Masks successfully exported");
    }
}

