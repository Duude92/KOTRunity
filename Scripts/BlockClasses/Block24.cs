using UnityEngine;

public class Block24 : BlockType, IBlocktype
{

    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public Vector3[] matrix = new Vector3[3];
    public Vector3 position = new Vector3();
    public int flag = 0;

    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();

        buffer.AddRange(Instruments.Vector3ToBytes(matrix[0]));
        buffer.AddRange(Instruments.Vector3ToBytes(matrix[1]));
        buffer.AddRange(Instruments.Vector3ToBytes(matrix[2]));

        buffer.AddRange(Instruments.Vector3ToBytesRevert(position));


        //buffer.AddRange(System.BitConverter.GetBytes(1));
        buffer.AddRange(System.BitConverter.GetBytes(flag));
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        byte[] bts = new byte[12];
        byte[] buff = new byte[4];

        System.Array.Copy(buffer, pos, bts, 0, 12);
        pos += (12);
        //matrix = new [3][];
        matrix[0] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));

        System.Array.Copy(buffer, pos, bts, 0, 12);
        pos += (12);
        matrix[1] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));
        System.Array.Copy(buffer, pos, bts, 0, 12);
        pos += (12);
        matrix[2] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));
        System.Array.Copy(buffer, pos, bts, 0, 12);
        pos += (12);
        position = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 8), System.BitConverter.ToSingle(bts, 4));
        System.Array.Copy(buffer, pos, buff, 0, 4);
        flag = System.BitConverter.ToInt32(buffer, pos);
        pos += (4);
        pos += 4;
    }
}