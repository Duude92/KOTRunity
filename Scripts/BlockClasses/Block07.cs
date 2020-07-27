using UnityEngine;
using System.Collections.Generic;
class Block07 : BlockType, IVerticesBlock, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }


    //-------- IVertices
    private List<Vector3> _vertices = new List<Vector3>();
    public List<Vector3> vertices { get => _vertices; set => _vertices = value; }
    private List<Vector2> _uv = new List<Vector2>();
    public List<Vector2> uv { get => _uv; set => _uv = value; }
    private List<Vector3> _normals = new List<Vector3>();
    public List<Vector3> normals { get => _normals; set => _normals = value; }
    public List<Vector2> uv1 { get => null; set => throw new System.NotImplementedException(); }

    //-------- 
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(new byte[32]);
        int vCount = 0;
        vCount = vertices.Count; //TODO: should this work?

        buffer.AddRange(System.BitConverter.GetBytes(vCount));
        for (int i = 0; i < vCount; i++)
        {
            buffer.AddRange(Instruments.Vector3ToBytesRevert(vertices[i]));
            buffer.AddRange(Instruments.Vector2ToBytes(uv[i]));

        }
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));


        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        this.unknownVector = Instruments.ReadV4(buffer, pos);
        pos += 16;
        pos += 32;
        int vCount = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;

        for (int i = 0; i < vCount; i++)
        {
            byte[] newBuff = new byte[20];
            System.Array.Copy(buffer, pos, newBuff, 0, 20);
            vertices.Add(Instruments.ReadV3(newBuff, 0));
            uv.Add(Instruments.ReadV2(newBuff, 12));
            pos += 20;
        }

        pos += 4;
    }
}