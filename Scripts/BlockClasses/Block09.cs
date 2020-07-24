using UnityEngine;
using System.Collections.Generic;
using System.Linq; //see OnDrawGizmos()
class Block09 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    [SerializeField] private Vector3 Direction;
    [SerializeField] private float Distance;
    void Awake()
    {
    }
    void OnDrawGizmos()
    {

        if (UnityEditor.Selection.gameObjects.Contains(gameObject))
        {

            Gizmos.color = Color.red;
            Vector3 position = Direction * -Distance;
            Gizmos.DrawSphere(position, 1f);
        }
    }

    public byte[] GetBytes()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Instruments.Vector4ToBytes(this.unknownVector));

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
        this.unknownVector = Instruments.ReadV4(buffer, pos);

        pos += 16;
        Direction = Instruments.ReadV3(buffer, pos);
        pos += 12;
        Distance = System.BitConverter.ToSingle(buffer, pos);
        pos += 4;


        script.triggerBox.Add(Direction * -Distance);
        if (false)//(script.triggerBox.Count==4)
        {
            TriggerBox();
        }

        pos += 4;//childCount
    }

    private void TriggerBox()
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (Vector3 vector in script.triggerBox)
        {
            foreach (Vector3 vector1 in script.triggerBox)
            {
                //if (vect)
            }
        }

    }
    public override void ClosingEvent()
    {
        script.triggerBox.RemoveAt(script.triggerBox.Count - 1);
    }


}
