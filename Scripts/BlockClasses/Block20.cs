using UnityEngine;
using System.Collections.Generic;
class Block20 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    //public B3DScript script;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    MeshCollider col;
    List<Vector3> vertices = new List<Vector3>();
    [SerializeField] int a = 0, b = 0, c = 0;
    [SerializeField] private char[] keyName = new char[12];
    [SerializeField] private float height;
    [SerializeField] private List<int> parameters = new List<int>();
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        int vCount = vertices.Count;

        buffer.AddRange(System.BitConverter.GetBytes(vCount));//TODO: vCount?
        buffer.AddRange(System.BitConverter.GetBytes(a));//TODO: vCount?
        buffer.AddRange(System.BitConverter.GetBytes(b));//TODO: vCount?
        buffer.AddRange(System.BitConverter.GetBytes(c));//TODO: vCount?
        if (c != 4)
        {
            for (int i = 0; i < c; i++)
            {
                if (i == 0)
                {
                    buffer.AddRange(System.BitConverter.GetBytes(height));
                }
                else
                {
                    buffer.AddRange(System.BitConverter.GetBytes(parameters[i - 1]));
                }
            }
        }
        else
        {
            buffer.AddRange(System.BitConverter.GetBytes(height));
            byte[] buff = new byte[12];
            System.Text.Encoding.ASCII.GetBytes(keyName).CopyTo(buff, 0);
            buffer.AddRange(buff);
        }
        // if (c == 4)
        // {
        //     buffer.AddRange(System.BitConverter.GetBytes(d));
        //     byte[] buff = new byte[12];
        //     System.Text.Encoding.ASCII.GetBytes(keyName).CopyTo(buff, 0);
        //     buffer.AddRange(buff);
        // }
        // else if (c == 1)
        // {
        //     buffer.AddRange(System.BitConverter.GetBytes(10));//TODO: height?
        // }
        foreach (var v in vertices)
        {
            buffer.AddRange(Instruments.Vector3ToBytes(v));

        }


        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        this.Type = 20;
        this.unknownVector = Instruments.ReadV4(buffer, pos);
        pos += 16;
        int vertNum = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        List<Vector3> verticesCol = new List<Vector3>();
        List<int> faces = new List<int>();
        a = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        float height = 0;
        b = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        int newParam = System.BitConverter.ToInt32(buffer, pos);
        c = newParam;
        pos += 4;
        col = thisObject.AddComponent<MeshCollider>() as MeshCollider;
        Mesh me = new Mesh();
        //------

        if (c == 4)
        {
            height = System.BitConverter.ToSingle(buffer, pos);
            pos += 4;
            System.Array.Copy(buffer, pos, keyName, 0, 12);
            pos += 12;

        }
        else
        {
            for (int i = 0; i < newParam; i++)
            {
                if (i == 0)
                {
                    height = System.BitConverter.ToSingle(buffer, pos);
                }
                else
                {
                    parameters.Add(System.BitConverter.ToInt32(buffer, pos));
                }
                pos += 4;


            }
        }



        //--------

        int number = 0;
        for (int i = 0; i < vertNum; i++)
        {
            var vert = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 4), System.BitConverter.ToSingle(buffer, pos + 8));
            vertices.Add(vert);
            pos += 12;
            verticesCol.Add(new Vector3(vert.x, vert.z - 100, vert.y));
            if (height > 0)
            {
                verticesCol.Add(new Vector3(vert.x + 0.1f, vert.z + (float)height, vert.y + 0.1f));
            }
            else
            {
                height = 100;
                verticesCol.Add(new Vector3(vert.x + 0.1f, vert.z + (float)height, vert.y + 0.1f));
            }
            if (i == 0)
            {
                faces.AddRange(new int[] { 0, 2, 1 });
                number += 2;
            }
            else
            {
                if (i + 1 == vertNum)
                    faces.AddRange(new int[3] { (number), (number + 1), (number - 1) });
                else
                    faces.AddRange(new int[3] { (number), (number + 1), (number - 1) });
                number += 2;
            }

        }
        me.vertices = verticesCol.ToArray();
        me.triangles = faces.ToArray();

        col.sharedMesh = me;
    }
}