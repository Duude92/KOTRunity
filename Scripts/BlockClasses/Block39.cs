using UnityEngine;
using System.Collections.Generic;
class Block39 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    GeneratorInvoker GI;

    public byte[] GetBytes()
    {
        throw new System.NotImplementedException();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        pos += 16;
        pos += 16;
        pos += 4;
        pos += 4;
    }


}