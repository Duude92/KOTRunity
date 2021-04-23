using UnityEngine;
using System.Collections.Generic;
public class DefaultSubclass : Block13SubclassBase
{
    [SerializeField] List<float> Params = new List<float>();

    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new List<byte>();
        foreach(var item in Params)
        {
            buffer.AddRange(System.BitConverter.GetBytes(item));
        }
        return buffer.ToArray();
    }


    protected override void InternalRead(byte[] data, ref int pos)
    {
        for (int i = 0; i < parameterCount; i++)
        {
            Params.Add(System.BitConverter.ToSingle(data, pos));
            pos += 4;
        }
    }
}