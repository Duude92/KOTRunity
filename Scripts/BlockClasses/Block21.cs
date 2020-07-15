using UnityEngine;
using System.Collections.Generic;
class Block21 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public B3DScript script;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    BlockSwitcher switcher;
    SphereCollider sc;
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(sc.center));
        buffer.AddRange(System.BitConverter.GetBytes(sc.radius));
        buffer.AddRange(System.BitConverter.GetBytes(switcher.groups));
        buffer.AddRange(System.BitConverter.GetBytes(new int())); //TODO: UNKNOWN INT
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount-switcher.groups+1)); //FIXME:444
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        thisObject.AddComponent<MeshRenderer>();//sw -> OnWillRender()
        script.SwitchBlocks.Add(thisObject);
        sc = thisObject.AddComponent<SphereCollider>();
        sc.center = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4));
        sc.radius = System.BitConverter.ToSingle(buffer, pos + 12);
        sc.isTrigger = true;
        pos += 16;
        switcher = thisObject.AddComponent<BlockSwitcher>();
        switcher.groups = System.BitConverter.ToInt32(buffer, pos);
        //sw.Initialize(System.BitConverter.ToInt32(resource,pos));
        pos += 4;
        pos += 4;
        pos += 4;
    }
}