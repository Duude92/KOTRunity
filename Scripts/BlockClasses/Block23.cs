using System.Collections.Generic;
using UnityEngine;
class Block23 : IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public B3DScript script;
    Mesh me;
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(1));
        buffer.AddRange(System.BitConverter.GetBytes(1));
        buffer.AddRange(System.BitConverter.GetBytes(0));

        int loops = 1;
        loops = me.vertexCount/3; 
        buffer.AddRange(System.BitConverter.GetBytes(loops));
        for (int i = 0; i < loops; i++)
        {
            
            //buffer.AddRange(System.BitConverter.GetBytes(me.vertices.Length));
            buffer.AddRange(System.BitConverter.GetBytes(3));

            List<byte> buff = new List<byte>();
            buff.AddRange(Instruments.Vector3ToBytesRevert(me.vertices[0+i*3]));
            buff.AddRange(Instruments.Vector3ToBytesRevert(me.vertices[2+i*3]));
            buff.AddRange(Instruments.Vector3ToBytesRevert(me.vertices[1+i*3]));
            

            // foreach (var v in me.vertices)
            // {
            //     buffer.AddRange(System.BitConverter.GetBytes(v.x));
            //     buffer.AddRange(System.BitConverter.GetBytes(v.z));
            //     buffer.AddRange(System.BitConverter.GetBytes(v.y));
            // }
            buffer.AddRange(buff);

        }
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        bool truck = false;
        //List<Mesh> meshe = new List<Mesh>();
        if (script.gameObject.name == "TRUCKS")
        {
            truck = true;
        }


        List<int> faces_all = new List<int>();

        pos += 8;
        List<Vector3> verticesCol = new List<Vector3>();
        List<int> faces = new List<int>();



        int i_null = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        for (int i = 0; i < i_null; i++)
        {
            pos += 4;
        }
        int triNum = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        int number = 0;
        for (int i = 0; i < triNum; i++)
        {
            int vertNum = System.BitConverter.ToInt32(buffer, pos);
            pos += 4;
            if (vertNum == 3)
            {
                faces.AddRange(new int[] { number, number + 2, number + 1 });
                number += 3;
            }
            else if (vertNum == 4)
            {
                faces.AddRange(new int[] { (number), (number + 2), (number + 1), (number + 3), (number + 2), (number) });
                number += 4;
            }
            else
            {
                for (int j = 0; j < vertNum - 2; j++)
                {
                    faces.AddRange(new int[3] { number + j + 2, number + j + 1, number });

                }
                number += vertNum;

            }
            for (int j = 0; j < vertNum; j++)
            {
                var vert = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4));
                /*var a = newObject.AddComponent<SphereCollider>();
                a.center = vert;
                a.radius = 0.1f;*/
                pos += 12;
                verticesCol.Add(vert);
                /*var a = new GameObject();
                    a.transform.SetParent(newObject.transform);
                    a.transform.position = new Vector3(vert.x,vert.z,vert.y);	*/
            }
            if (truck)
            {
                int Count = faces.Count / 3;
                for (int j = 0; j < Count; j++)
                {
                    List<Vector3> newTri = script.Extrude(verticesCol[faces[j]], verticesCol[faces[j + 1]], verticesCol[faces[j + 2]]);
                    verticesCol.AddRange(newTri);
                    faces.Add(verticesCol.Count - 3);
                    faces.Add(verticesCol.Count - 2);
                    faces.Add(verticesCol.Count - 1);

                }



                me = new Mesh();
                MeshCollider col = thisObject.AddComponent<MeshCollider>();
                me.vertices = verticesCol.ToArray();
                me.triangles = faces.ToArray();
                me.name = ((DamageKey)i).ToString();


                col.sharedMesh = me;
                col.convex = true;
                faces_all.AddRange(faces);
                faces = new List<int>();
            }


        }
        {//Mesh me conflict
            me = new Mesh();
            MeshCollider col = thisObject.AddComponent<MeshCollider>();
            me.vertices = verticesCol.ToArray();
            if (!truck)
            {
                me.triangles = faces.ToArray();
                col.sharedMesh = me;
            }
        }
        /*else
        {
            me.triangles = faces_all.ToArray();
            col.convex = true;
        } */
    }
}