
class Block00 : IBlocktype
{        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[40];
        byte[] count = System.BitConverter.GetBytes(thisObject.transform.childCount);
        byte[] buff2 = new byte[buffer.Length+4];
        buffer.CopyTo(buff2,0);
        count.CopyTo(buff2,buffer.Length-1);
        buffer = buff2;
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {
        throw new System.NotImplementedException();
    }
}