using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(BlockSwitcher),typeof(SphereCollider))]
class Block21 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    BlockSwitcher switcher;
    SphereCollider sc;
    public byte[] GetBytes()
    {
        if(!thisObject)
        {
            thisObject = gameObject;
        }
        if(!sc|!switcher)
        {
            sc = GetComponent<SphereCollider>();
            switcher = GetComponent<BlockSwitcher>();
            if(!sc|!switcher)
            {
                throw new System.Exception("SphereCollider or BlockSwitcher not found");
            }
        }
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(sc.center));
        buffer.AddRange(System.BitConverter.GetBytes(sc.radius));
        buffer.AddRange(System.BitConverter.GetBytes(switcher.groups));
        buffer.AddRange(System.BitConverter.GetBytes(new int())); //TODO: UNKNOWN INT
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount - switcher.groups + 1)); //FIXME:444
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        script.SwitchBlocks.Add(thisObject);
        sc = thisObject.GetComponent<SphereCollider>();
        sc.center = Instruments.ReadV3(buffer,pos);
        sc.radius = System.BitConverter.ToSingle(buffer, pos + 12);
        sc.isTrigger = true;
        pos += 16;
        switcher = thisObject.GetComponent<BlockSwitcher>();
        switcher.groups = System.BitConverter.ToInt32(buffer, pos);
        //sw.Initialize(System.BitConverter.ToInt32(resource,pos));
        pos += 4;
        pos += 4;
        pos += 4;
    }
}