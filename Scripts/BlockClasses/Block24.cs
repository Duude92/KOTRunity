using UnityEngine;

class Block24 : MonoBehaviour,IBlocktype {

        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

	//public float[][] matrix = new float[3][];
	public Vector3[] matrix = new Vector3[3];
	public Vector3 position = new Vector3();

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[52];
		byte[] pos = new byte[12];
		pos = Instruments.Vector3ToBytes(matrix[0]);
		pos.CopyTo(buffer,0);
		pos = Instruments.Vector3ToBytes(matrix[1]);
		pos.CopyTo(buffer,12);
		pos = Instruments.Vector3ToBytes(matrix[2]);
		pos.CopyTo(buffer,24);

		pos = Instruments.Vector3ToBytes(position);
		pos.CopyTo(buffer,40);


        byte[] count = System.BitConverter.GetBytes(thisObject.transform.childCount);
        byte[] buff2 = new byte[buffer.Length+4];
        buffer.CopyTo(buff2,0);
        count.CopyTo(buff2,buffer.Length-1);
        buffer = buff2;
        return buffer;
    }

    public void Read(byte[] buffer, ref int pos)
    {
        throw new System.NotImplementedException();
    }
}