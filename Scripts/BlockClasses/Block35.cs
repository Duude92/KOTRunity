using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class Block35 : BlockType, IBlocktype, IMeshInfo
{
    private Mesh mesh;

    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    private List<int> _materials = new List<int>();
    public List<int> Materials { get => _materials; set => _materials = value; }

    public int matNum = 0;
    public int i_null;
    private int j_null;
    public List<int> format = new List<int>();
    List<int> faces = new List<int>();

    public byte[] GetBytes()
    {
        mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
        buffer.AddRange(System.BitConverter.GetBytes(i_null)); //Some value i_null
        buffer.AddRange(System.BitConverter.GetBytes(matNum)); //Some value MatNum
        int loopCount = mesh.triangles.Length / 3;
        buffer.AddRange(System.BitConverter.GetBytes(j_null)); //Some value j_null
        if (i_null == 3)
        {
            for (int i = 0; i < loopCount; i++)
            {
                List<byte> face = new List<byte>();
                face.AddRange(System.BitConverter.GetBytes(16)); //UNKNOWN DATA
                face.AddRange(System.BitConverter.GetBytes(1f));
                face.AddRange(System.BitConverter.GetBytes(32767));
                face.AddRange(System.BitConverter.GetBytes(matNum)); //TODO: MATNUM
                face.AddRange(System.BitConverter.GetBytes(3)); //count vertices in face???
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                buffer.AddRange(face);

            }
        }
        else if (i_null < 3)
        {
            List<byte> face = new List<byte>();

            int i = 0;
            // for fmt in format

            foreach (int fmt in format)
            {
                face = new List<byte>();
                face.AddRange(System.BitConverter.GetBytes(fmt));
                face.AddRange(System.BitConverter.GetBytes(1f));
                face.AddRange(System.BitConverter.GetBytes(32767));
                face.AddRange(System.BitConverter.GetBytes(matNum)); //TODO: MATNUM
                face.AddRange(System.BitConverter.GetBytes(3));

                if (fmt == 50)
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[20]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[20]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[20]);
                }
                else if (fmt == 49)
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[4]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[4]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[4]);
                }
                else if ((fmt == 1) || (fmt == 0))
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                }
                else if ((fmt == 2) || (fmt == 3))
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[8]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[8]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[8]);
                }
                else
                {
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[0 + i * 3]));
                    face.AddRange(new byte[12]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[2 + i * 3]));
                    face.AddRange(new byte[12]);
                    face.AddRange(System.BitConverter.GetBytes(mesh.triangles[1 + i * 3]));
                    face.AddRange(new byte[12]);
                }
                buffer.AddRange(face);
                i++;
            }

        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        Transform tr = script.GetParentVertices(transform);
        IVerticesBlock bt1 = tr.GetComponent<BlockType>() as IVerticesBlock;


        List<Vector3> newVector = new List<Vector3>();
        newVector.AddRange(bt1.vertices);
        List<Vector3> newNormals = new List<Vector3>();
        newNormals.AddRange(bt1.normals);
        List<int> newFaces = new List<int>();
        List<Vector2> nextUvs = new List<Vector2>();
        nextUvs.AddRange(bt1.uv);

        byte[] buff = new byte[4];
        pos += 16;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        i_null = System.BitConverter.ToInt32(buff, 0);
        matNum = System.BitConverter.ToInt32(buffer, pos);
        _materials.Add(matNum);
        pos += 4;//textureNum?
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        j_null = System.BitConverter.ToInt32(buff, 0);
        //
        //List<int> matNum = new List<int>();
        Mesh curMesh = new Mesh();
        curMesh.Clear();
        if (i_null < 3)
        {
            //faces = new int[j_null*3];
            for (int i = 0; i < j_null; i++)
            {
                int[] face = new int[3];
                System.Array.Copy(buffer, pos, buff, 0, 4); pos += 4;
                format.Add(System.BitConverter.ToInt32(buff, 0));
                if (format[format.Count - 1] == 50)
                {
                    byte[] newBuff = new byte[88];
                    // FORMAT: format(i),1f,32767,matNum,fCount,face0,uv,f1,f2,unkn_int?,fCount,face0,uv,f1,f2,unkn_int?,fCount,face0,uv,f1,f2,unkn_int?,
                    System.Array.Copy(buffer, pos, newBuff, 0, 88);
                    pos += 88;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 64), System.BitConverter.ToInt32(newBuff, 40) };
                    nextUvs.Add(Instruments.ReadV2(newBuff, 20));
                    nextUvs.Add(Instruments.ReadV2(newBuff, 68));
                    nextUvs.Add(Instruments.ReadV2(newBuff, 44));

                    newVector.Add(newVector[face[0]]);
                    newVector.Add(newVector[face[1]]);
                    newVector.Add(newVector[face[2]]);
                    if (bt1.normals.Count > 0)
                    {
                        newNormals.Add(newNormals[face[0]]);
                        newNormals.Add(newNormals[face[1]]);
                        newNormals.Add(newNormals[face[2]]);
                    }
                    //face = new int[0];
                    newFaces.AddRange(new int[] { newVector.Count - 3, newVector.Count - 2, newVector.Count - 1 });
                }
                else if (format[format.Count - 1] == 49)
                {
                    byte[] newBuff = new byte[40];
                    System.Array.Copy(buffer, pos, newBuff, 0, 40);
                    pos += 40;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 32), System.BitConverter.ToInt32(newBuff, 24) };
                }
                else if ((format[format.Count - 1] == 1) || (format[format.Count - 1] == 0))
                {
                    byte[] newBuff = new byte[28];
                    System.Array.Copy(buffer, pos, newBuff, 0, 28);
                    pos += 28;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 24), System.BitConverter.ToInt32(newBuff, 20) };
                }
                else if ((format[format.Count - 1] == 2) || (format[format.Count - 1] == 3))
                {
                    byte[] newBuff = new byte[52];
                    System.Array.Copy(buffer, pos, newBuff, 0, 52);
                    pos += 52;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 40), System.BitConverter.ToInt32(newBuff, 28) };
                    nextUvs.Add(Instruments.ReadV2(newBuff, 20));
                    nextUvs.Add(Instruments.ReadV2(newBuff, 44));
                    nextUvs.Add(Instruments.ReadV2(newBuff, 32));

                    newVector.Add(newVector[face[0]]);
                    newVector.Add(newVector[face[1]]);
                    newVector.Add(newVector[face[2]]);
                    if (bt1.normals.Count > 0)
                    {
                        newNormals.Add(newNormals[face[0]]);
                        newNormals.Add(newNormals[face[1]]);
                        newNormals.Add(newNormals[face[2]]);
                    }
                    //face = new int[0];
                    newFaces.AddRange(new int[] { newVector.Count - 3, newVector.Count - 2, newVector.Count - 1 });
                }
                else
                {
                    byte[] newBuff = new byte[64];
                    System.Array.Copy(buffer, pos, newBuff, 0, 64);
                    pos += 64;
                    face = new int[3] { System.BitConverter.ToInt32(newBuff, 16), System.BitConverter.ToInt32(newBuff, 48), System.BitConverter.ToInt32(newBuff, 32) };
                }
                faces.AddRange(face);
            }

        }
        else if (i_null == 3)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[32];

                System.Array.Copy(buffer, pos, newBuff, 0, 32);
                pos += 32;
                int[] face = new int[3] { System.BitConverter.ToInt32(newBuff, 20), System.BitConverter.ToInt32(newBuff, 28), System.BitConverter.ToInt32(newBuff, 24) };
                faces.AddRange(face);
            }
        }

        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = script.GetComponent<Materials>().maths[script.TexInts[matNum]];// resOb.GetComponent<Materials>().maths[TexInts[matNum]];



        {
            List<int> nextFaces = new List<int>();
            nextFaces.AddRange(faces);
            nextFaces.AddRange(newFaces);
            if (newFaces.Count > 0)
            {
                int i = 0;
            }
            curMesh.vertices = newVector.ToArray();
            curMesh.triangles = nextFaces.ToArray();
            curMesh.uv = nextUvs.ToArray();
            curMesh.normals = bt1.normals.ToArray();
        }

        // {
        //         curMesh.vertices = bt1.vertices.ToArray();
        //         curMesh.triangles = faces.ToArray();
        //         curMesh.uv = bt1.uv.ToArray();
        //         curMesh.normals = bt1.normals.ToArray();
        // }
        try
        {
            curMesh.uv2 = bt1.uv1.ToArray();
        }
        catch
        {
            ;
        }



        curMesh.RecalculateBounds();

        gameObject.AddComponent<MeshFilter>().mesh = curMesh;





    }
}

[CustomEditor(typeof(Block35))]
public class Block35Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Mesh"))
        {
            Mesh mesh = (target as Block35).GetComponent<MeshFilter>().sharedMesh;
            string objectName = target.name;
            if (string.IsNullOrEmpty(objectName))
            {
                objectName = "Object" + target.GetInstanceID();
            }
            AssetDatabase.CreateAsset(mesh, $"Assets/Meshes/{objectName}.asset");
            GameObject gob = (target as Block35).gameObject;

            PrefabUtility.SaveAsPrefabAsset(gob, $"Assets/Prefabs/{objectName}.prefab");
        }
    }
}