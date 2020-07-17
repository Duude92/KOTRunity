using UnityEngine;
using System.Collections.Generic;
class Block12 : IBlocktype
{
    public int a, b, paramCount;
    public List<float> Params = new List<float>();
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
BoxCollider boxCollider;
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(new byte[12]);

        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.position.y*-1));
        buffer.AddRange(new byte[12]);

    

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        pos += 16;
        pos += 12;
        float height = System.BitConverter.ToSingle(buffer, pos);
        pos += 4;
       boxCollider = thisObject.AddComponent<BoxCollider>();
       boxCollider.size = new Vector3(10000f, 0f, 10000f);

        thisObject.transform.position = new Vector3(0, height * -1, 0);
        pos += 12;
    }
}
