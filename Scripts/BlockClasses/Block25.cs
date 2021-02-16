using UnityEngine;
class Block25 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        throw new System.NotImplementedException();
        byte[] buffer = new byte[40];
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 12;
        pos += 32;
        pos += 4;
        pos += 40;
        Debug.LogWarning("Read is not implemented", gameObject);

    }
}