using UnityEngine;
public class OtherBazaSubclass : Block13SubclassBase
{
    [SerializeField] string scene;
    protected override void InternalRead(byte[] data, ref int pos)
    {
        char[] roomName = System.Text.Encoding.ASCII.GetChars(data, pos, 4);
        scene = new string(roomName);
        pos += 4;
    }
    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        byte[] roomByte = System.Text.Encoding.ASCII.GetBytes(scene);
        byte[] roomByte16 = new byte[4];
        roomByte.CopyTo(roomByte16, 0);
        buffer.AddRange(roomByte16);
        return buffer.ToArray();
    }

}