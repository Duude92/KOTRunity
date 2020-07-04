using UnityEngine;

class Block24 : MonoBehaviour,IBlocktype {


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


		return buffer;
    }
}