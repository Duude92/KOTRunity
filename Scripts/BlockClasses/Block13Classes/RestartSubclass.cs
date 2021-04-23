using UnityEngine;
public class RestartSubclass : Block13SubclassBase
{
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 maybeDirection;
    [SerializeField] string room;

    protected override void InternalRead(byte[] data, ref int pos)
    {

        position = Instruments.ReadV3(data, pos);
        pos += 12;
        maybeDirection = Instruments.ReadV3(data, pos);
        pos += 12;
        char[] roomName = System.Text.Encoding.ASCII.GetChars(data, pos, 16);
        room = new string(roomName);
        pos += 16;
        transform.position = position;
    }
    public void OnDrawGizmos()
    {
        position = transform.position;
    }

    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(position));
        buffer.AddRange(Instruments.Vector3ToBytes(maybeDirection));
        byte[] roomByte = System.Text.Encoding.ASCII.GetBytes(room);
        byte[] roomByte16 = new byte[16];
        roomByte.CopyTo(roomByte16,0);
        buffer.AddRange(roomByte16);
        return buffer.ToArray();
    }
}