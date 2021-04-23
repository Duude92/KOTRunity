using UnityEngine;
using System.Collections.Generic;
using System.Linq; //see OnDrawGizmos()
class Block09 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    [SerializeField] public Vector3 Direction;
    [SerializeField] public float Distance;
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
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        foreach (Vector3 vector in script.triggerBox)
        {
            foreach (Vector3 vector1 in script.triggerBox)
            {

            }
        }

    }
    public override void ClosingEvent()
    {
        //script.triggerBox.RemoveAt(script.triggerBox.Count - 1);
        script.triggerBox.Clear();
    }

    public override void ComaEvent()
    {
        return;
        List<Vector3> rawVertices = new List<Vector3>();
        BlockType bt = this;
        while (bt?.Type == 9)
        {
            var b9 = (Block09)bt;
            rawVertices.Add(b9.Direction * -b9.Distance);
            bt = bt.transform.parent?.GetComponent<BlockType>();
        }

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        Vector3 topVector = new Vector3(0, 5, 0);
        foreach (var item in rawVertices)
        {
            vertices.Add(item);
            vertices.Add(item + topVector);
        }
        int[] quads = new int[vertices.Count * 2];
        for (int i = 0; i < vertices.Count - 2; i += 2)
        {
            quads[i * 2 + 0] = i + 0;
            quads[i * 2 + 1] = i + 1;
            quads[i * 2 + 2] = i + 3;
            quads[i * 2 + 3] = i + 2;
        }
        {
            int i = vertices.Count - 2;
            quads[i * 2 + 0] = i + 0;
            quads[i * 2 + 1] = i + 1;
            quads[i * 2 + 2] = 0;
            quads[i * 2 + 3] = 1;
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(quads, MeshTopology.Quads, 0);
        mesh.RecalculateBounds();
        var mf = gameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = true;

    }

}
