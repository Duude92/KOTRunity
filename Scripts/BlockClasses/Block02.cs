using UnityEngine;
class Block02 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[32];
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 36;
        Debug.LogWarning("Read is not implemented", gameObject);
    }
}