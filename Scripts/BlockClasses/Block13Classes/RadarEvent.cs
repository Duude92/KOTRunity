using UnityEngine;
using System.Collections.Generic;
public class RadarEvent : Block13SubclassBase
{
    float[] floats = new float[4];
    MeshCollider meshCollider;
    Mesh mesh;
    protected override void InternalRead(byte[] data, ref int pos)
    {
        for(int i = 0; i<4;i++)
        {
            floats[i] = System.BitConverter.ToSingle(data,pos);
            pos+=4;
        }

        // mesh = new Mesh();
        // meshCollider.sharedMesh = mesh;
        // RecalculateBounds();
    }
    public void RecalculateBounds()
    {
        Transform tr = transform;
        List<Vector3> rawVertices = new List<Vector3>();
        

        while (tr.parent)
        {
            BlockType bt = tr.GetComponent<BlockType>();
            if (bt?.Type == 9)
            {
                var b9 = (Block09)bt;
                rawVertices.Add(b9.Direction * -b9.Distance);
            }
            tr = tr.parent;
        }

    }

    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new List<byte>();
        foreach(var item in floats)
        {
            buffer.AddRange(System.BitConverter.GetBytes(item));
        }
        return buffer.ToArray();
    }
}