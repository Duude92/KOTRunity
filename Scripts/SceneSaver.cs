using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SceneSaver : MonoBehaviour
{
    static BinaryWriter binaryWriter;// = new BinaryWriter(File.Create(path));

    public static void SaveScene(GameObject root, string path)
    {
        binaryWriter = new BinaryWriter(File.Create(path));


        try
        {

            byte[] buffer4b = new byte[4];

            buffer4b = System.Text.Encoding.UTF8.GetBytes("b3d\0");


            binaryWriter.Write(buffer4b);
            binaryWriter.Write(0);
            binaryWriter.Write(6);
            B3DScript script = root.GetComponent<B3DScript>();

            binaryWriter.Write(script.Materials.Count * 8 + 1);
            binaryWriter.Write(script.Materials.Count * 8 + 7);

            binaryWriter.Write(0);
            binaryWriter.Write(script.Materials.Count);


            Materials math = root.GetComponent<Materials>();

            foreach (string mat in math.material)
            {
                byte[] matName = new byte[32];
                string mat1 = mat.Split(' ')[0];

                System.Text.Encoding.UTF8.GetBytes(mat1, 0, mat1.Length, matName, 0);

                binaryWriter.Write(matName);

            }

            binaryWriter.Write(111);
            foreach (Transform gob in root.transform)
            {
                WriteObjectRecursive(gob.gameObject);
            }
            binaryWriter.Write(222);


            binaryWriter.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);



            binaryWriter.Close();
        }
    }

    static void WriteObjectRecursive(GameObject gameObject)
    {
        try
        {
            if (gameObject.name == "444")
            {
                binaryWriter.Write(444);
                return;
            }
            else
            {
                BlockType bt = gameObject.GetComponent<BlockType>();
                int type = bt.Type;
                binaryWriter.Write(333);
                byte[] blockName = new byte[32];
                System.Text.Encoding.UTF8.GetBytes(gameObject.name, 0, gameObject.name.Length, blockName, 0);
                binaryWriter.Write(blockName);
                binaryWriter.Write(type);
                IBlocktype component = (IBlocktype)bt;
                if (component == null)
                {
                    switch (bt.Type)
                    {
                        case 23:
                            component = bt.gameObject.GetComponent<Block23>();
                            break;
                        case 9:
                            component = bt.gameObject.GetComponent<Block09>();
                            break;
                    }

                    if (component == null)
                    {
                        Debug.Log(bt.gameObject, bt.gameObject);
                        return;
                    }

                }

                binaryWriter.Write(component.GetBytes());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e, gameObject);
        }
        //binaryWriter.Write(gameObject.transform.childCount);
        foreach (Transform gob in gameObject.transform)
        {
            WriteObjectRecursive(gob.gameObject);
        }
        binaryWriter.Write(555);



    }

}




