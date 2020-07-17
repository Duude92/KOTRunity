using UnityEngine;
using System.Collections.Generic;
class Block13 : IBlocktype
{
    public int a, b, paramCount;
    public List<float> Params = new List<float>();
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(System.BitConverter.GetBytes(a));
        buffer.AddRange(System.BitConverter.GetBytes(b));
        buffer.AddRange(System.BitConverter.GetBytes(paramCount));
        foreach (float p in Params)
        {
            buffer.AddRange(System.BitConverter.GetBytes(p));
        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 16;
        a = System.BitConverter.ToInt32(buffer, pos);
        b = System.BitConverter.ToInt32(buffer, pos + 4);
        int paramCount2 = System.BitConverter.ToInt32(buffer, pos + 8);
        paramCount = paramCount2;
        pos += 4;
        pos += 8;
        //int i_null = System.BitConverter.ToInt32(buff,0);
        for (int i = 0; i < paramCount; i++)
        {
            Params.Add(System.BitConverter.ToSingle(buffer, pos));
            pos += 4;
        }

    }
}
