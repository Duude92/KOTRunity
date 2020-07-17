using UnityEngine;
using System.Collections.Generic;
class Block30 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    Light point;
    public B3DScript script;
    LoadTrigger go;
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        byte[] rName = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(go.roomName).CopyTo(rName, 0);
        buffer.AddRange(rName);

        buffer.AddRange(Instruments.Vector3ToBytesRevert(go.point0));
        buffer.AddRange(Instruments.Vector3ToBytesRevert(go.point1));

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        thisObject.AddComponent<MeshRenderer>();//OnWillRender()
        pos += 16;
        byte[] name = new byte[32];
        //
        System.Array.Copy(buffer, pos, name, 0, 32);
        pos += 32;

        go = thisObject.AddComponent<LoadTrigger>();
        go.roomName = System.Text.Encoding.UTF8.GetString(name).Replace("\x0", string.Empty);

        go.point0 = Instruments.ReadV3(buffer, pos);
        pos += 12; //position

        go.point1 = Instruments.ReadV3(buffer, pos);
        pos += 12; //position

        go.Trigger();
        script.LoadTriggers.Add(thisObject); //аар1
    }
}