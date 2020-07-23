using UnityEngine;
class Block09 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    [SerializeField] private Vector3 Direction;
    [SerializeField] private float Distance;


    public Vector4 position;
    void Awake()
    {
    }
    void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {

            Gizmos.color = Color.red;
            Vector3 position = Direction * -Distance;
            Gizmos.DrawSphere(position, 1f);
        }
    }

    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Instruments.Vector4ToBytes(position));

        buffer.AddRange(Instruments.Vector3ToBytesRevert(Direction));
        buffer.AddRange(System.BitConverter.GetBytes(Distance));
        int childCount = 0;
        foreach (Transform t in transform)
        {
            if (!t.name.Contains("444"))
            {
                childCount++;

            }
        }
        buffer.AddRange(System.BitConverter.GetBytes(childCount));

        return buffer.ToArray();

    }

    public void Read(byte[] buffer, ref int pos)
    {
        this.Type = 9;
        position = Instruments.ReadV4(buffer, pos);

        pos += 16;
        Direction = Instruments.ReadV3(buffer, pos);
        pos += 12;
        Distance = System.BitConverter.ToSingle(buffer, pos);
        pos += 4;


        pos += 4;//childCount
    }


}
