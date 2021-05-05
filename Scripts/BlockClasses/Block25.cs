using UnityEngine;
class Block25 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    [SerializeField] private int field32767;
    [SerializeField] private int someParameter1;
    [SerializeField] private int someParameter2;
    [SerializeField] private string soundFile;
    [SerializeField] private Vector3 soundPosition;
    [SerializeField] private Vector3 someVector0;
    [SerializeField] private Vector3 someVector1;
    [SerializeField] private Vector2 someVector2;
    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> byteBuffer = new System.Collections.Generic.List<byte>();
        byteBuffer.AddRange(System.BitConverter.GetBytes(field32767));
        byteBuffer.AddRange(System.BitConverter.GetBytes(someParameter1));
        byteBuffer.AddRange(System.BitConverter.GetBytes(someParameter2));

        byte[] file = System.Text.Encoding.ASCII.GetBytes(soundFile);
        byte[] soundFile1 = new byte[32];
        file.CopyTo(soundFile1,0);
        byteBuffer.AddRange(soundFile1);

        byteBuffer.AddRange(Instruments.Vector3ToBytesRevert(soundPosition));
        byteBuffer.AddRange(Instruments.Vector3ToBytesRevert(someVector0));
        byteBuffer.AddRange(Instruments.Vector3ToBytesRevert(someVector1));
        byteBuffer.AddRange(Instruments.Vector2ToBytes(someVector2));
        
        
        Debug.LogWarning("Write is not tested for block 25", gameObject);
        return byteBuffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos) //TODO:CH.B3D //Sound?
    {
        field32767 = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        someParameter1 = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        someParameter2 = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        byte[] soundName = new byte[32];
        System.Array.Copy((System.Array)buffer, pos, soundName, 0, 32);
        soundFile = System.Text.Encoding.UTF8.GetString(soundName).Trim(new char[] { '\0' });
        pos += 32;
        soundPosition = Instruments.ReadV3(buffer, pos);
        pos += 12;
        someVector0 = Instruments.ReadV3(buffer, pos);
        pos += 12;
        someVector1 = Instruments.ReadV3(buffer, pos);
        pos += 12;
        someVector2 = Instruments.ReadV2(buffer, pos);
        pos += 8;
    }
}