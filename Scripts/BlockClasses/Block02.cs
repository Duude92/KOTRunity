
class Block02 : IBlocktype
{        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[32];
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {
        throw new System.NotImplementedException();
    }
}