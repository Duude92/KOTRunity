using UnityEngine;
using System.Collections.Generic;
public class Block13SubclassLimited09 : Block13SubclassBase
{
    List<Vector3> rawVertices = new List<Vector3>();
    protected override byte[] InternalGetByte()
    {
        return new byte[0];
    }

    protected override void InternalRead(byte[] data, ref int pos)
    {
        Transform tr = transform;
        rawVertices = new List<Vector3>();

        while (tr.parent)
        {
            BlockType bt = tr.GetComponent<BlockType>();
            if (bt?.Type == 9)
            {
                var b9 = (Block09)bt;
                rawVertices.Add(b9.Direction * -b9.Distance);
            }
            tr = tr.parent;
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
            if (i <= 0)
                return;
            quads[i * 2 + 0] = i + 0;
            quads[i * 2 + 1] = i + 1;
            quads[i * 2 + 2] = 0;
            quads[i * 2 + 3] = 1;
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(quads, MeshTopology.Quads, 0);
        mesh.RecalculateBounds();
        // var mf = gameObject.AddComponent<MeshFilter>();
        // mf.sharedMesh = mesh;
        var mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = true;


        return;
    }


    void OnDrawGizmos()
    {
        if(UnityEditor.Selection.activeGameObject != gameObject)
        return;
        Gizmos.color = Color.red;
        foreach (var item in rawVertices)
        {
            var dir = item.normalized;
            Gizmos.DrawCube(item,(Vector3.one-dir)*10000);
        }
    }
}