using System.Collections.Generic;
using UnityEngine;
class Block23 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public B3DScript script;
    Mesh me;
    private collisionType colType;
    enum collisionType
    {
        unknown,
        nonRoad,
        Road
        
    }
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(1));
        buffer.AddRange(System.BitConverter.GetBytes((int)colType)); //unknown data
        buffer.AddRange(System.BitConverter.GetBytes(0));

        int loops = 1;
        loops = me.subMeshCount;
        buffer.AddRange(System.BitConverter.GetBytes(loops));
        for (int i = 0; i < loops; i++)
        {

            //buffer.AddRange(System.BitConverter.GetBytes(me.vertices.Length));
            int[] indices = me.GetIndices(i);
            buffer.AddRange(System.BitConverter.GetBytes(indices.Length));
            List<Vector3> faces = new List<Vector3>();

            for (int j = 0; j < indices.Length; j++)
            {
                buffer.AddRange(Instruments.Vector3ToBytesRevert(me.vertices[indices[j]]));
            }

        }
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        bool truck = false;
        if (script.gameObject.name == "TRUCKS")
        {
            truck = true;
        }
        truck = false;

        pos += 4;
        colType = (collisionType)System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        
        List<Vector3> verticesCol = new List<Vector3>();
        List<int[]> faces = new List<int[]>();



        int i_null = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        for (int i = 0; i < i_null; i++)
        {
            pos += 4;
        }
        int loopCount = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        int number = 0;
        me = new Mesh();
        me.subMeshCount = loopCount;

        for (int i = 0; i < loopCount; i++)
        {
            int vCount = System.BitConverter.ToInt32(buffer, pos);
            pos += 4;

            faces.Add(new int[vCount]);
            for (int j = 0; j < vCount; j++)
            {
                faces[i][j] = number;
                number += 1;
                var vert = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4));
                pos += 12;
                verticesCol.Add(vert);
            }
            if (truck)
            {
                List<int> faces1 = new List<int>();
                int Count = faces1.Count / 3;
                for (int j = 0; j < Count; j++)
                {
                    List<Vector3> newTri = script.Extrude(verticesCol[faces1[j]], verticesCol[faces1[j + 1]], verticesCol[faces1[j + 2]]);
                    verticesCol.AddRange(newTri);
                    faces1.Add(verticesCol.Count - 3);
                    faces1.Add(verticesCol.Count - 2);
                    faces1.Add(verticesCol.Count - 1);

                }



                me = new Mesh();
                MeshCollider col = thisObject.AddComponent<MeshCollider>();
                me.vertices = verticesCol.ToArray();
                me.triangles = faces[0]; // legacy
                me.name = ((DamageKey)i).ToString();


                col.sharedMesh = me;
                col.convex = true;
                //faces_all.AddRange(faces[0]); // legacy
                faces = new List<int[]>();
            }


        }
        {
            MeshCollider col = thisObject.AddComponent<MeshCollider>();
            me.vertices = verticesCol.ToArray();
            if (!truck)
            {
                for (int i = 0; i < faces.Count; i++)
                {
                    //me.triangles = faces.ToArray();
                    me.SetIndices(faces[i], MeshTopology.Quads, i);
                }
                col.sharedMesh = me;
            }
            // thisObject.AddComponent<MeshFilter>().sharedMesh = me;
            // thisObject.AddComponent<MeshRenderer>();
        }

    }
}