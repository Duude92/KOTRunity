using UnityEngine;
using System.Collections.Generic;
public class Block35 : MonoBehaviour, IBlocktype
{
    public B3DScript script;
    private int nrml0 = 0, nrml1 = 0;
    private Mesh mesh;

    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public int matNum = 0;

    public byte[] GetBytes()
    {
        mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(System.BitConverter.GetBytes(3)); //Some value i_null
        buffer.AddRange(System.BitConverter.GetBytes(matNum)); //Some value MatNum
        int loopCount = mesh.triangles.Length / 3;
        buffer.AddRange(System.BitConverter.GetBytes(loopCount)); //Some value j_null
        for (int i = 0; i < loopCount; i++)
        {
            List<byte> face = new List<byte>();
            face.AddRange(System.BitConverter.GetBytes(16)); //UNKNOWN DATA
            face.AddRange(System.BitConverter.GetBytes(1f));
            face.AddRange(System.BitConverter.GetBytes(32767));
            face.AddRange(System.BitConverter.GetBytes(matNum)); //TODO: MATNUM
            face.AddRange(System.BitConverter.GetBytes(3)); //count vertices in face???
            face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0+i*3]));
            face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2+i*3]));
            face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1+i*3]));
            buffer.AddRange(face);

        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        int i_null = System.BitConverter.ToInt32(buff, 0);
        matNum = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;//textureNum?
        int j_null, i_null2;
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
                i_null2 = System.BitConverter.ToInt32(buff, 0);
                if (i_null2 == 50)
                {
                    byte[] newBuff = new byte[88];
                    //
                    System.Array.Copy(buffer, pos, newBuff, 0, 88);
                    pos += 88;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 64), System.BitConverter.ToInt32(newBuff, 40) };
                }
                else if (i_null2 == 49)
                {
                    byte[] newBuff = new byte[40];
                    System.Array.Copy(buffer, pos, newBuff, 0, 40);
                    pos += 40;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 32), System.BitConverter.ToInt32(newBuff, 24) };
                }
                else if ((i_null2 == 1) || (i_null2 == 0))
                {
                    byte[] newBuff = new byte[28];
                    System.Array.Copy(buffer, pos, newBuff, 0, 28);
                    pos += 28;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 24), System.BitConverter.ToInt32(newBuff, 20) };
                }
                else if ((i_null2 == 2) || (i_null2 == 3))
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
        if (script.normals != null)
        {
            if (script.normals.Count == curMesh.vertices.Length)
            {
                curMesh.normals = script.normals.ToArray();
            }
            else
            {


                nrml0 = nrml0 + nrml1;
                nrml1 = nrml1 + curMesh.vertices.Length - 1;

                curMesh.normals = script.normals.GetRange(nrml0, nrml1).ToArray();
            }
        }
        curMesh.uv = script.UV.ToArray();
        if(script.UV1.Count>0)
        {
            curMesh.uv2 = script.UV1.ToArray();
            script.UV1 = new List<Vector2>();
        }

        curMesh.RecalculateBounds();

        gameObject.AddComponent<MeshFilter>().mesh = curMesh;

        Transform tr = script.GetParentVertices(transform);
        BlockType bt1 = tr.GetComponent<BlockType>();


        ((VerticesBlock)bt1.component).mesh.Add(curMesh); //ищет в каждом родительском обьекте компоненту verticesBlock рекурсивно    }


    }
}