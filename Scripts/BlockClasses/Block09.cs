using UnityEngine;
class Block09 : MonoBehaviour, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public Vector4 xz4eEto;
    public Vector4 position;

    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Instruments.Vector4ToBytes(position));

        buffer.AddRange(Instruments.Vector4ToBytes(xz4eEto));
		int childCount = 0;
		foreach(Transform t in transform)
		{
			if(!t.name.Contains("444"))
			{
				childCount++;

			}
		}
        buffer.AddRange(System.BitConverter.GetBytes(childCount));

        return buffer.ToArray();

    }

    public void Read(byte[] buffer, ref int pos)
    {
        position = Instruments.ReadV4(buffer, pos);

        pos += 16;
        xz4eEto = Instruments.ReadV4(buffer, pos);


        pos += 16;

        pos += 4;//childCount
    }


}
