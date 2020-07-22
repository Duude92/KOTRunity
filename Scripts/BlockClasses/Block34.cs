using UnityEngine;
using System.Collections.Generic;
class Block34 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    [HideInInspector] public B3DScript script;
    [SerializeField] int vCount;
    [SerializeField] int j_null;
    [SerializeField] int materialNum;
    [SerializeField] int type2;
    [SerializeField] List<Vector4> vectors28 = new List<Vector4>();


    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(new byte[4]);

        buffer.AddRange(System.BitConverter.GetBytes(vCount));
        for (int i = 0; i < vCount; i++)
        {

            buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
            buffer.AddRange(System.BitConverter.GetBytes(-256)); //someVal 


        }


        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        this.Type = 34;
        byte[] buff = new byte[4];

        pos += 16;
        pos += 4;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        vCount = System.BitConverter.ToInt32(buff, 0);
        for (int i = 0; i < vCount; i++)
        {
            pos += 16;
        }

    }
}