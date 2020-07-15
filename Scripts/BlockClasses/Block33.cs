using UnityEngine;
using System.Collections.Generic;
class Block33 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    Light point;

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(new byte[12]); //TODO: UNKNOWN DATA 1i 1i 131072i
        buffer.AddRange(Instruments.Vector3ToBytes(point.transform.position));
        buffer.AddRange(new byte[36]); //TODO: UNKNOWN DATA
        buffer.AddRange(System.BitConverter.GetBytes(point.color.r));
        buffer.AddRange(System.BitConverter.GetBytes(point.color.g));
        buffer.AddRange(System.BitConverter.GetBytes(point.color.b));
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        point = thisObject.AddComponent<Light>();
        pos += 16;

        pos += 12;//
        byte[] newbuff = new byte[12];
        System.Array.Copy(buffer, pos, newbuff, 0, 12); 
        pos += 12; //position
        point.transform.position = new Vector3(System.BitConverter.ToSingle(newbuff, 0), System.BitConverter.ToSingle(newbuff, 8), System.BitConverter.ToSingle(newbuff, 4));

        pos += 36; //
        System.Array.Copy(buffer, pos, newbuff, 0, 12); pos += 12; //color
        point.color = new Color(System.BitConverter.ToSingle(newbuff, 0), System.BitConverter.ToSingle(newbuff, 4), System.BitConverter.ToSingle(newbuff, 8));

        pos += 4; //child
    }
}