
class Block29 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        throw new System.NotImplementedException();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        int i_null = System.BitConverter.ToInt32(buff, 0);
        pos += 4;
        pos += 28;
        if (i_null == 4)
        {
            pos += 4;
        }
        else
        {
            ;
        }
        pos += 4;

    }
}