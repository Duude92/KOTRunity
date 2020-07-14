using UnityEngine;
using System.Collections.Generic;
class Block07 : VerticesBlock, IBlocktype
{        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public B3DScript script;

    public byte[] GetBytes()
    {
        throw new System.NotImplementedException();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        script.normals = null;
        pos += 16;
        pos += 32;
        int i_null = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        script.vertices = new List<Vector3>();
        script.UV = new List<Vector2>();

        for (int i = 0; i < i_null; i++)
        {
            byte[] newBuff = new byte[20];
            System.Array.Copy(buffer, pos, newBuff, 0, 20);
            pos += 20;
            script.vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
            float u = System.BitConverter.ToSingle(newBuff, 12);
            float v = System.BitConverter.ToSingle(newBuff, 16);


            script.UV.Add(new Vector2(u, v));
        }
        pos += 4;
    }
}