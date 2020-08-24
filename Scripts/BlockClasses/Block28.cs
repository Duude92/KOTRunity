using UnityEngine;
using System.Collections.Generic;
class Block28 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    [SerializeField] int loopCount;
    [SerializeField] int j_null;
    [SerializeField] int materialNum;
    [SerializeField] int type2;
    [SerializeField] int vertexCount;
    [SerializeField] List<Vector4> vectors28 = new List<Vector4>();


    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(System.BitConverter.GetBytes(loopCount)); //someVal type
        for (int i = 0; i < loopCount; i++)
        {
            buffer.AddRange(System.BitConverter.GetBytes(j_null)); //someVal type
            buffer.AddRange(System.BitConverter.GetBytes(1f)); //someVal type
            buffer.AddRange(System.BitConverter.GetBytes(32767)); //someVal type
            buffer.AddRange(System.BitConverter.GetBytes(materialNum)); //someVal type
            buffer.AddRange(System.BitConverter.GetBytes(vertexCount)); //someVal type
            for (int j = 0; j < vertexCount; j++)
            {
                buffer.AddRange(Instruments.Vector4ToBytes(new Vector4()));
            }

        }

        /*if (loopCount == 1) //LEGACY
        {
            buffer.AddRange(System.BitConverter.GetBytes(j_null)); //someVal j_null
            buffer.AddRange(new byte[8]); //
            buffer.AddRange(System.BitConverter.GetBytes(materialNum)); //materialNum
            buffer.AddRange(System.BitConverter.GetBytes(type2)); //materialNum

            if (j_null > 1)
            {
                for (int i = 0; i < type2; i++)
                {
                    buffer.AddRange(new byte[16]); //
                }
            }
            else
            {
                buffer.AddRange(new byte[32]); //
            }
        }
        else if (loopCount == 2)
        {
            buffer.AddRange(new byte[20]); //
            for (int i = 0; i < 4; i++)
            {
                buffer.AddRange(new byte[28]); //
            }

            buffer.AddRange(new byte[4]); //
        }
        else if ((loopCount == 10) || (loopCount == 6))
        {
            for (int i = 0; i < loopCount; i++)
            {
                buffer.AddRange(new byte[16]); //
                buffer.AddRange(System.BitConverter.GetBytes(type2)); //materialNum

                for (int j = 0; j < type2; j++)
                {
                    buffer.AddRange(new byte[8]); //
                }
            }
        }*/

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos) //FIXME: Не определился как его генерировать, работает не правильно
    {
        Transform tr = script.GetParentVertices(transform);
        IVerticesBlock bt1 = tr.GetComponent<BlockType>() as IVerticesBlock;


        this.Type = 28;

        byte[] buff = new byte[4];
        List<int> faces = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        faces.AddRange(new int[] { 0, 1, 2, 3, 0, 2 });

        Mesh me = new Mesh();
        me.Clear();
        me.vertices = bt1.vertices.ToArray();
        me.triangles = faces.ToArray();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        for (int i = 4; i<bt1.vertices.Count;i++)
        {
            uvs.Add(new Vector2(0,0));
        }
        me.uv = uvs.ToArray();

        me.normals = bt1.normals.ToArray();

        me.RecalculateBounds();
        int tex = 0;


        gameObject.AddComponent<MeshRenderer>();
        pos += 16;
        pos += 12;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        loopCount = System.BitConverter.ToInt32(buff, 0);
        for (int i = 0; i < loopCount; i++)
        {
            System.Array.Copy(buffer, pos, buff, 0, 4);
            pos += 4;
            j_null = System.BitConverter.ToInt32(buff, 0);
            pos += 8;
            materialNum = System.BitConverter.ToInt32(buffer, pos);
            pos += 4;
            System.Array.Copy(buffer, pos, buff, 0, 4);
            pos += 4;
            vertexCount = System.BitConverter.ToInt32(buff, 0);
            for (int j = 0; j < vertexCount; j++)
            {
                vectors28.Add(Instruments.ReadV4(buffer, pos));
                pos += 16;
            }
        }

        // if (type == 1) // LEGACY
        // {
        //     System.Array.Copy(buffer, pos, buff, 0, 4);
        //     pos += 4;
        //     j_null = System.BitConverter.ToInt32(buff, 0);
        //     pos += 8;
        //     tex = System.BitConverter.ToInt32(buffer, pos);
        //     pos += 4;
        //     System.Array.Copy(buffer, pos, buff, 0, 4);
        //     pos += 4;
        //     type2 = System.BitConverter.ToInt32(buff, 0);

        //     if (j_null > 1)
        //     {
        //         for (int i = 0; i < type2; i++)
        //         {
        //             pos += 16;
        //         }
        //     }
        //     else
        //     {
        //         pos += 32;
        //     }
        // }
        // else if (type == 2)
        // {
        //     pos += 20;
        //     for (int i = 0; i < 4; i++)
        //     {
        //         pos += 28;
        //     }

        //     pos += 4;
        // }
        // else if ((type == 10) || (type == 6))
        // {
        //     for (int i = 0; i < type; i++)
        //     {
        //         pos += 16;
        //         System.Array.Copy(buffer, pos, buff, 0, 4); pos += 4;
        //         type2 = System.BitConverter.ToInt32(buff, 0);
        //         for (int j = 0; j < type2; j++)
        //         {
        //             pos += 8;
        //         }
        //     }
        // }

        gameObject.AddComponent<MeshFilter>().mesh = me;
        gameObject.GetComponent<MeshRenderer>().material = script.gameObject.GetComponent<Materials>().maths[script.TexInts[tex]];

    }
}