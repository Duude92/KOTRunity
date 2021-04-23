using UnityEngine;
using System.Collections.Generic;
public abstract class Block13SubclassBase : MonoBehaviour
{
    [SerializeField] protected int parameterCount;
    public void Read(byte[] data, ref int pos)
    {
        parameterCount = System.BitConverter.ToInt32(data, pos);
        pos += 4;

        InternalRead(data, ref pos);
    }
    public byte[] GetByte()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(parameterCount));
        buffer.AddRange(InternalGetByte());
        return buffer.ToArray();
    }
    protected abstract byte[] InternalGetByte();
    protected abstract void InternalRead(byte[] data, ref int pos);

}