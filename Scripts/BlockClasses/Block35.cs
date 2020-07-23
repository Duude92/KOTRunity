using UnityEngine;
using System.Collections.Generic;
public class Block35 : MonoBehaviour, IBlocktype
{
    public B3DScript script;
    private Mesh mesh;

    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public int matNum = 0;
    public int i_null;
    public List<int> format = new List<int>();
    public byte[] GetBytes()
    {
        mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(System.BitConverter.GetBytes(i_null)); //Some value i_null
        buffer.AddRange(System.BitConverter.GetBytes(matNum)); //Some value MatNum
        int loopCount = mesh.triangles.Length / 3;
        buffer.AddRange(System.BitConverter.GetBytes(loopCount)); //Some value j_null
        if (i_null == 3)
        {
            for (int i = 0; i < loopCount; i++)
            {
                List<byte> face = new List<byte>();
                face.AddRange(System.BitConverter.GetBytes(16)); //UNKNOWN DATA
                face.AddRange(System.BitConverter.GetBytes(1f));
                face.AddRange(System.BitConverter.GetBytes(32767));
                face.AddRange(System.BitConverter.GetBytes(matNum)); //TODO: MATNUM
                face.AddRange(System.BitConverter.GetBytes(3)); //count vertices in face???
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                buffer.AddRange(face);

            }
        }
        else if (i_null < 3)
        {
            List<byte> face = new List<byte>();

            int i = 0;
            // for fmt in format

            foreach (int fmt in format)
            {
                face = new List<byte>();
                face.AddRange(System.BitConverter.GetBytes(fmt));
                face.AddRange(System.BitConverter.GetBytes(1f));
                face.AddRange(System.BitConverter.GetBytes(32767));
                face.AddRange(System.BitConverter.GetBytes(matNum)); //TODO: MATNUM
                face.AddRange(System.BitConverter.GetBytes(3));

                if (fmt == 50)
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[20]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[20]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[20]);
                }
                else if (fmt == 49)
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[4]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[4]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[4]);
                }
                else if ((fmt == 1) || (fmt == 0))
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                }
                else if ((fmt == 2) || (fmt == 3))
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[8]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[8]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[8]);
                }
                else
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[12]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[12]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[12]);
                }
                buffer.AddRange(face);
                i++;
            }

        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        i_null = System.BitConverter.ToInt32(buff, 0);
        matNum = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;//textureNum?
        int j_null;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        j_null = System.BitConverter.ToInt32(buff, 0);
        //
        List<int> faces = new List<int>();
        //List<int> matNum = new List<int>();
        Mesh curMesh = new Mesh();
        curMesh.Clear();

        if (i_null < 3)
        {
            //faces = new int[j_null*3];
            for (int i = 0; i < j_null; i++)
            {
                int[] face = new int[3];
                System.Array.Copy(buffer, pos, buff, 0, 4); pos += 4;
                format.Add(System.BitConverter.ToInt32(buff, 0));
                if (format[format.Count - 1] == 50)
                {
                    byte[] newBuff = new byte[88];
                    //
                    System.Array.Copy(buffer, pos, newBuff, 0, 88);
                    pos += 88;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 64), System.BitConverter.ToInt32(newBuff, 40) };
                }
                else if (format[format.Count - 1] == 49)
                {
                    byte[] newBuff = new byte[40];
                    System.Array.Copy(buffer, pos, newBuff, 0, 40);
                    pos += 40;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 32), System.BitConverter.ToInt32(newBuff, 24) };
                }
                else if ((format[format.Count - 1] == 1) || (format[format.Count - 1] == 0))
                {
                    byte[] newBuff = new byte[28];
                    System.Array.Copy(buffer, pos, newBuff, 0, 28);
                    pos += 28;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 24), System.BitConverter.ToInt32(newBuff, 20) };
                }
                else if ((format[format.Count - 1] == 2) || (format[format.Count - 1] == 3))
                {
                    byte[] newBuff = new byte[52];
                    System.Array.Copy(buffer, pos, newBuff, 0, 52);
                    pos += 52;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 40), System.BitConverter.ToInt32(newBuff, 28) };
                }
                else
                {
                    byte[] newBuff = new byte[64];
                    System.Array.Copy(buffer, pos, newBuff, 0, 64);
                    pos += 64;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 48), System.BitConverter.ToInt32(newBuff, 32) };
                }
                faces.AddRange(face);
            }

        }
        else if (i_null == 3)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[32];

                System.Array.Copy(buffer, pos, newBuff, 0, 32);
                pos += 32;
                int[] face = new int[3] { System.BitConverter.ToInt32(newBuff, 20), System.BitConverter.ToInt32(newBuff, 28), System.BitConverter.ToInt32(newBuff, 24) };
                faces.AddRange(face);
            }
        }

        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().material = script.GetComponent<Materials>().maths[script.TexInts[matNum]];// resOb.GetComponent<Materials>().maths[TexInts[matNum]];


        curMesh.vertices = script.vertices.ToArray();
        curMesh.triangles = faces.ToArray();
        curMesh.uv = script.UV.ToArray();

        if (script.normals.Count > 0)
        {
            curMesh.normals = script.normals.ToArray();
        }
        if (script.UV1Users > 0)
        {
            curMesh.uv2 = script.UV1.ToArray();
            script.UV1Users--;
            if (script.UV1Users == 0)
            {
                script.UV1 = new List<Vector2>();
                script.UV = new List<Vector2>();
            }
        }


        curMesh.RecalculateBounds();

        gameObject.AddComponent<MeshFilter>().mesh = curMesh;

        Transform tr = script.GetParentVertices(transform);
        BlockType bt1 = tr.GetComponent<BlockType>();


        ((VerticesBlock)bt1.component).mesh.Add(curMesh); //ищет в каждом родительском обьекте компоненту verticesBlock рекурсивно    }


    }
}