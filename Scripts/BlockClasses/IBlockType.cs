using UnityEngine;

public interface IBlocktype
{
    byte[] GetBytes();
    void Read(byte[] buffer, ref int pos);
    UnityEngine.GameObject thisObject { get; set; }
}
