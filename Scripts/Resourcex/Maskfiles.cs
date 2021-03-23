using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

class Maskfiles : MonoBehaviour
{
    public List<string> masks = new List<string>();
    public List<Texture2D> textures = new List<Texture2D>();
    public List<Material> maths = new List<Material>();
    public void AddMsk(string name, byte[] rawIm, AssetImportContext rootObject = null)
    {
        masks.Add(name);
        int pos = 4;

        short width = System.BitConverter.ToInt16(rawIm, pos);
        pos += 2;
        short height = System.BitConverter.ToInt16(rawIm, pos);
        pos += 2;

        BinaryWriter bw = new BinaryWriter(File.OpenWrite(@"d:\file"));
        bw.Write(rawIm);
        bw.Close();

        ColorsRGB[] colors = new ColorsRGB[256];

        for (int i = 0; i < 256; i++)
        {
            colors[i].R = rawIm[pos];
            colors[i].G = rawIm[pos + 1];
            colors[i].B = rawIm[pos + 2];
            pos += 3;
        }

        byte[] raw2 = new byte[width * height];
        int pos2 = 0;

        for (; pos < rawIm.Length;)
        //while(pos2<rawIm.Length-776)
        {
            byte number = rawIm[pos];
            //Debug.Log(pos+" "+i);

            if (number <= 127)
            {
                System.Array.Copy(rawIm, pos + 1, raw2, pos2, number);
                pos2 += number;
                pos += number + 1;
            }
            else
            {
                pos2 += number - 128;
                pos++;
            }


            /*raw2[i*3] = colors[rawIm[pos]].R;
			raw2[i*3+1] = colors[rawIm[pos]].G;
			raw2[i*3+2] = colors[rawIm[pos]].B;*/
        }
        byte[] raw3 = new byte[raw2.Length * 3];
        for (int i = 0; i < raw2.Length; i++)
        {
            raw3[i * 3] = colors[raw2[i]].R;
            raw3[i * 3 + 1] = colors[raw2[i]].G;
            raw3[i * 3 + 2] = colors[raw2[i]].B;
            pos++;
        }

        /*BinaryWriter bw2 = new BinaryWriter(File.OpenWrite(@"d:\file2"));
		bw2.Write(raw2);
		bw2.Close();*/
        pos = 0;
        for (int i = 0; i < height; i++)
        {
            System.Array.Reverse(raw3, pos, width * 3);
            pos += width * 3;
        }
        System.Array.Reverse(raw3);
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.name = name;

        tex.LoadRawTextureData(raw3);
        tex.Apply();

        if (rootObject != null)
        {
            rootObject.AddObjectToAsset(name, tex);
        }


        textures.Add(tex);

    }

}
