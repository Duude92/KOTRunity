using UnityEngine;

public interface IBlocktype
{
    byte[] GetBytes();
}
public static class Instruments
{
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