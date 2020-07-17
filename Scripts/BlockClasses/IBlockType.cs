using UnityEngine;

public interface IBlocktype
{
    byte[] GetBytes();
    void Read(byte[] buffer, ref int pos);
    UnityEngine.GameObject thisObject { get; set; }
}
public static class Instruments
{
    public static Vector3 ReadV3(byte[] buffer, int pos)
    {
        Vector3 vector = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4));
        return vector;
    }
    public static Vector4 ReadV4(byte[] buffer, int pos)
    {
        Vector4 vector = new Vector4(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4), System.BitConverter.ToSingle(buffer, pos + 12));
        return vector;
    }
    public static Vector3 ReadV3NonRevert(byte[] buffer, int pos)
    {
        Vector3 vector = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 4), System.BitConverter.ToSingle(buffer, pos + 8));
        return vector;
    }
    public static Vector2 ReadV2(byte[] buffer, int pos)
    {
        Vector2 vector = new Vector2(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 4));
        return vector;
    }
    public static byte[] Vector2ToBytes(Vector2 vector)
    {
        byte[] buffer = new byte[8];
        byte[] bytes = System.BitConverter.GetBytes(vector.x);
        bytes.CopyTo(buffer, 0);
        bytes = System.BitConverter.GetBytes(vector.y);
        bytes.CopyTo(buffer, 4);
        return buffer;

    }

    public static byte[] Vector3ToBytes(Vector3 vector)
    {
        byte[] buffer = new byte[12];
        byte[] bytes = System.BitConverter.GetBytes(vector.x);
        bytes.CopyTo(buffer, 0);
        bytes = System.BitConverter.GetBytes(vector.y);
        bytes.CopyTo(buffer, 4);
        bytes = System.BitConverter.GetBytes(vector.z);
        bytes.CopyTo(buffer, 8);
        return buffer;

    }
    public static byte[] Vector3ToBytesRevert(Vector3 vector)
    {
        byte[] buffer = new byte[12];
        byte[] bytes = System.BitConverter.GetBytes(vector.x);
        bytes.CopyTo(buffer, 0);
        bytes = System.BitConverter.GetBytes(vector.z);
        bytes.CopyTo(buffer, 4);
        bytes = System.BitConverter.GetBytes(vector.y);
        bytes.CopyTo(buffer, 8);
        return buffer;

    }
    public static byte[] Vector4ToBytes(Vector4 vector)
    {
        byte[] buffer = new byte[16];
        byte[] bytes = System.BitConverter.GetBytes(vector.x);
        bytes.CopyTo(buffer, 0);
        bytes = System.BitConverter.GetBytes(vector.y);
        bytes.CopyTo(buffer, 4);
        bytes = System.BitConverter.GetBytes(vector.z);
        bytes.CopyTo(buffer, 8);
        bytes = System.BitConverter.GetBytes(vector.w);
        bytes.CopyTo(buffer, 12);
        return buffer;

    }

}