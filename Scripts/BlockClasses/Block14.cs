using UnityEngine;
using System.Collections.Generic;
class Block14 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public B3DScript script;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    BlockSwitcher switcher;
    SphereCollider sc;
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        
        buffer.AddRange(new byte[28]);
        
        
                return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 16;
        pos += 16;
        pos += 4;

        pos += 8;
    }
}