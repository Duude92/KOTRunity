using UnityEngine;
using System.Collections.Generic;
class Block14 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    BlockSwitcher switcher;
    SphereCollider sc;
    [SerializeField] Vector4 newVector;
    [SerializeField] Vector2 doubleVector;
    [SerializeField] float someFloat;


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
        unknownVector = Instruments.ReadV4(buffer, pos);
        pos += 16;
        newVector = Instruments.ReadV4(buffer, pos);

        pos += 16;
        doubleVector = Instruments.ReadV2(buffer, pos);
        pos += 8;
        someFloat = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;

    }
}