using UnityEngine;
using System.Collections.Generic;
class Block07 : VerticesBlock, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public B3DScript script;

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(new byte[32]);
        int vCount = 0;
        vCount = mesh[0].vertices.Length; //TODO: should this work?
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UV = new List<Vector2>();
        vertices.AddRange(mesh[0].vertices);
        UV.AddRange(mesh[0].uv);

        buffer.AddRange(System.BitConverter.GetBytes(vCount));
        for(int i = 0; i<vCount;i++)
        {
            buffer.AddRange(Instruments.Vector3ToBytesRevert(vertices[i]));
            buffer.AddRange(Instruments.Vector2ToBytes(UV[i]));
            
        }
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));


        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        script.normals = new List<Vector3>();
        pos += 16;
        pos += 32;
        int vCount = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        script.vertices = new List<Vector3>();
        script.UV = new List<Vector2>();

        for (int i = 0; i < vCount; i++)
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