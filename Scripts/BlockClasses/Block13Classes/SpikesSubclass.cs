using UnityEngine;
public class SpikesSubclass : Block13SubclassBase
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] float triggerWidth;
    private Mesh mesh = null;
    private MeshCollider meshCollider = null;
    protected override void InternalRead(byte[] data, ref int pos)
    {
        startPosition = Instruments.ReadV3(data, pos);
        pos += 12;
        endPosition = Instruments.ReadV3(data, pos);
        pos += 12;
        triggerWidth = System.BitConverter.ToSingle(data, pos);
        pos += 4;

        mesh = new Mesh();

        var mf = gameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        RecalculateBounds();
    }
    public void RecalculateBounds()
    {
        Matrix4x4 mt = Matrix4x4.LookAt(startPosition, endPosition, Vector3.up);

        var startRight = new Vector3(mt.m00, mt.m10, mt.m20);


        Vector3[] vertices = new Vector3[8];
        //near
        //left
        vertices[0] = startPosition - startRight * triggerWidth - Vector3.up * triggerWidth;//bot
        vertices[1] = startPosition - startRight * triggerWidth + Vector3.up * triggerWidth;//top
        //right
        vertices[2] = startPosition + startRight * triggerWidth - Vector3.up * triggerWidth;//bot
        vertices[3] = startPosition + startRight * triggerWidth + Vector3.up * triggerWidth;//top
        //far
        //left
        vertices[4] = endPosition - startRight * triggerWidth - Vector3.up * triggerWidth;//bot
        vertices[5] = endPosition - startRight * triggerWidth + Vector3.up * triggerWidth;//top
        //right
        vertices[6] = endPosition + startRight * triggerWidth - Vector3.up * triggerWidth;//bot
        vertices[7] = endPosition + startRight * triggerWidth + Vector3.up * triggerWidth;//top

        mesh.vertices = vertices;
        int[] quads = new int[] {
         0,1,3,2,   //near
         2,3,7,6,   //right
         1,3,5,7,   //top
         0,2,4,6,   //bot
         0,1,5,4,   //left
         4,5,7,6    //far
        };

        mesh.SetIndices(quads, MeshTopology.Quads, 0);
        mesh.RecalculateBounds();

        meshCollider.convex = false;
        meshCollider.convex = true;
    }

    protected override byte[] InternalGetByte()
    {
        System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytesZ(startPosition));
        buffer.AddRange(Instruments.Vector3ToBytesZ(endPosition));
        buffer.AddRange(System.BitConverter.GetBytes(triggerWidth));
        return buffer.ToArray();
    }
}