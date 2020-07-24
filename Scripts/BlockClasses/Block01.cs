
class Block01 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[64];
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {

        pos += 32;
        pos += 32;
    }
}