using UnityEngine;
class Block09 : MonoBehaviour, IBlocktype
{        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public Vector4 xz4eEto;
    public Vector3 pos;

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[32];
        byte[] bytes = new byte[16];
		bytes = Instruments.Vector4ToBytes(xz4eEto);
		bytes.CopyTo(buffer,0);
		bytes = Instruments.Vector4ToBytes(pos);
		bytes.CopyTo(buffer,16);

		return buffer;

    }

    public void Read(byte[] buffer, ref int pos)
    {
        throw new System.NotImplementedException();
    }

    /*void Start () //аар3
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pos = new Vector3(xz4eEto.x,xz4eEto.y,xz4eEto.z);
		go.transform.localScale = (new Vector3(1,1,1) - new Vector3(Mathf.Abs(pos.x),Mathf.Abs(pos.y),Mathf.Abs(pos.z)))*10000;
		pos = pos * xz4eEto.w;
		pos = pos * -1;
		go.transform.position = pos;
		go.transform.SetParent(gameObject.transform);
		Destroy(go.GetComponent<BoxCollider>());
	}*/


}
