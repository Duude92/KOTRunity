using UnityEngine;
using System.Collections.Generic;
public abstract class Block13SubclassBase : MonoBehaviour
{
    [SerializeField] protected int parameterCount;
    List<Vector4> info = new List<Vector4>();

    public void Read(byte[] data, ref int pos)
    {
        Transform tr = transform;

        while (tr.parent)
        {
            BlockType bt = tr.GetComponent<BlockType>();
            if (bt?.Type == 9)
            {
                var b9 = (Block09)bt;

                Vector4 ax = b9.Direction;
                ax.w = b9.Distance;

                info.Add(ax);
            }
            tr = tr.parent;
        }





        parameterCount = System.BitConverter.ToInt32(data, pos);
        pos += 4;

        InternalRead(data, ref pos);
    }
    public byte[] GetByte()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(parameterCount));
        buffer.AddRange(InternalGetByte());
        return buffer.ToArray();
    }
    protected abstract byte[] InternalGetByte();
    protected abstract void InternalRead(byte[] data, ref int pos);


    void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject)
            return;
        var col = Color.magenta;
        col.a = 0.5f;
        Gizmos.color = col;
        foreach (var item in info)
        {

            var dir = item.normalized;
            Vector3 limit = item * -item.w;
            Vector3 scale = (Vector3.one - new Vector3(Mathf.Abs(item.x), Mathf.Abs(item.y), Mathf.Abs(item.z))) * 10000;

            Gizmos.DrawCube(limit, scale);
        }
    }


}