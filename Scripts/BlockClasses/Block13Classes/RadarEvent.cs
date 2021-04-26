using UnityEngine;
using System.Collections.Generic;
public class RadarEvent : Block13SubclassBase
{
    [SerializeField] float[] floats = new float[4];
    [SerializeField] float speedLimit = 0;
    [SerializeField] Vector3 someDirection = new Vector3();
    protected override void InternalRead(byte[] data, ref int pos)
    {

        speedLimit = System.BitConverter.ToSingle(data, pos);
        pos += 4;

        someDirection = Instruments.ReadV3(data, pos);
        pos += 12;

    }


    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new List<byte>();
        foreach (var item in floats)
        {
            buffer.AddRange(System.BitConverter.GetBytes(item));
        }
        return buffer.ToArray();
    }


}