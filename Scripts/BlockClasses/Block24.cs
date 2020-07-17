using UnityEngine;

class Block24 : MonoBehaviour,IBlocktype {

        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

	//public float[][] matrix = new float[3][];
	public Vector3[] matrix = new Vector3[3];
	public Vector3 position = new Vector3();

    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();

		buffer.AddRange(Instruments.Vector3ToBytes(matrix[0]));
		buffer.AddRange(Instruments.Vector3ToBytes(matrix[1]));
		buffer.AddRange(Instruments.Vector3ToBytes(matrix[2]));

		buffer.AddRange(Instruments.Vector3ToBytesRevert(position));


        buffer.AddRange(System.BitConverter.GetBytes(1));
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        throw new System.NotImplementedException();
    }
}