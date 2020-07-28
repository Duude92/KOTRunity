using System.Collections.Generic;
using UnityEngine;

public class Block08 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public List<Vector3> vertices;
    public List<string> material = new List<string>();
    public List<string> loops = new List<string>();
    List<int> matNum;
    int format;
    List<int> formats = new List<int>();
    List<List<int>> facesData1 = new List<List<int>>();


    public byte[] GetBytes()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);

        int subMeshCount = mesh.subMeshCount;
        buffer.AddRange(System.BitConverter.GetBytes(subMeshCount)); //Some value i_null
        int fcount = 0;
        for (int i = 0; i < subMeshCount; i++)
        {
            buffer.AddRange(System.BitConverter.GetBytes(formats[i]));
            buffer.AddRange(System.BitConverter.GetBytes(1f));
            buffer.AddRange(System.BitConverter.GetBytes(32767));
            //buffer.AddRange(new byte[8]); //1f, 32767
            List<byte> face = new List<byte>();
            face.AddRange(System.BitConverter.GetBytes(matNum[i])); //TODO: MATNUM
            int[] faces = facesData1[i].ToArray();//mesh.GetIndices(i);

            int vCount = faces.Length;//mesh.GetIndices(i).Length;


            face.AddRange(System.BitConverter.GetBytes(vCount)); //count vertices in face???

            if ((formats[i] == 178) || (formats[i] == 50))
            {
                //vCount = faces.Length;
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));//TODO: Похоже что везде нужно vCount-1-j, но не факт
                    face.AddRange(Instruments.Vector2ToBytes(mesh.uv[faces[j]]));
                    face.AddRange(new byte[12]);
                }
            }
            else if ((formats[i] == 3) || (formats[i] == 2) || (formats[i] == 131))
            {
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));
                    face.AddRange(Instruments.Vector2ToBytes(mesh.uv[faces[ j]]));
                }
            }
            else if ((formats[i] == 176) || (formats[i] == 48) || (formats[i] == 179) || (formats[i] == 51))
            {
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));
                    face.AddRange(new byte[12]);
                }
            }
            else if ((formats[i] == 177))
            {
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));
                    face.AddRange(new byte[4]);
                }
            }
            else if ((formats[i] == 16) || (formats[i] == 1) || (formats[i] == 0))
            {
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));
                }
            }
            else if (((formats[i] == 144) || (formats[i] == 129)) || true)
            {
                face.RemoveRange(face.Count - 4, 4);
                // int[] indices = mesh.GetIndices(i);
                // System.Array.Reverse(indices);
                // List<int> face_old = new List<int>();
                // foreach (int ind in indices)
                // {
                //     if (!face_old.Contains(ind))
                //     {
                //         face_old.Add(ind);
                //         //face.AddRange(System.BitConverter.GetBytes(ind));
                //     }
                // }
                vCount = faces.Length;
                face.AddRange(System.BitConverter.GetBytes(vCount));
                for (int j = 0; j < vCount; j++)
                {
                    face.AddRange(System.BitConverter.GetBytes(faces[j]));
                }

                // foreach (var ind in face_old)
                // {
                //     face.AddRange(System.BitConverter.GetBytes(facesData[fcount + j]));
                //     j++;
                // }


            }
            fcount += vCount - 1;


            buffer.AddRange(face);

        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        Transform tr = script.GetParentVertices(transform);

        IVerticesBlock bt1 = tr.GetComponent<BlockType>() as IVerticesBlock;
        //UV = new List<Vector2>();
        pos += 16;
        int polygons = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        int j_null;

        List<int> faces_all = new List<int>();

        matNum = new List<int>();
        Mesh curMesh = new Mesh();
        curMesh.Clear();
        curMesh.subMeshCount = polygons;
        List<Material> mats = new List<Material>();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Vector2[] uv_new = new Vector2[bt1.vertices.Count];
        List<int> faces = new List<int>();
        curMesh.vertices = bt1.vertices.ToArray();

        for (int i = 0; i < polygons; i++)
        {
            List<int> facesData = new List<int>();

            //normals = null;
            List<int> faces_old = new List<int>();
            List<List<int>> facesOfFaces = new List<List<int>>();
            format = System.BitConverter.ToInt32(buffer, pos);
            formats.Add(format);
            pos += 4;
            pos += 8;


            matNum.Add(System.BitConverter.ToInt32(buffer, pos));
            pos += 4;

            j_null = System.BitConverter.ToInt32(buffer, pos);
            pos += 4;
            string loop = "f: " + format + " | ";
            MeshTopology mt = MeshTopology.Quads;
            if ((format == 178) || (format == 50))
            {
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    faces_old.Add(num);
                    facesData.Add(num);
                    pos += 4;
                    Vector2 uv = new Vector2(System.BitConverter.ToSingle(buffer, pos + 0), System.BitConverter.ToSingle(buffer, pos + 4));
                    //UV.Add(uv);
                    uv_new[num] = uv;
                    pos += 8;
                    loop += faces_old[faces_old.Count - 1] + ' ' + uv.ToString() + ' ' + System.BitConverter.ToSingle(buffer, pos + 0) + ' ' + System.BitConverter.ToSingle(buffer, pos + 4) + ' ' + System.BitConverter.ToSingle(buffer, pos + 8) + " ";
                    pos += 12;

                }
                if (j_null == 4)
                    mt = MeshTopology.Quads;
                else
                    mt = MeshTopology.Triangles;
            }
            else if ((format == 3) || (format == 2) || (format == 131))
            {
                //List<int> faces3 = new List<int>();
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    // vertices.Add(vertices[num]); //Это правильно, но УВ для каждой вершины каждого полигона
                    // faces_old.Add(vertices.Count - 1);
                    faces_old.Add(num);
                    facesData.Add(num);

                    pos += 4;
                    Vector2 uv = new Vector2(System.BitConverter.ToSingle(buffer, pos + 0), System.BitConverter.ToSingle(buffer, pos + 4)); //FIXME: УВ для каждой вершины каждого полигона
                                                                                                                                            //UV.Add(uv); // FIXME
                    uv_new[num] = uv;

                    pos += 8;
                    loop += faces_old[faces_old.Count - 1] + ' ' + uv.ToString() + " ";

                }
            }
            else if ((format == 176) || (format == 48) || (format == 179) || (format == 51))
            {
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    faces_old.Add(num);
                    facesData.Add(num);

                    pos += 4;
                    loop += faces_old[faces_old.Count - 1] + ' ' + System.BitConverter.ToSingle(buffer, pos + 0) + ' ' + System.BitConverter.ToSingle(buffer, pos + 4) + ' ' + System.BitConverter.ToSingle(buffer, pos + 8) + " ";

                    pos += 12;
                }
            }
            else if ((format == 177))
            {
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    faces_old.Add(num);
                    facesData.Add(num);

                    pos += 4;
                    loop += faces_old[faces_old.Count - 1] + ' ' + System.BitConverter.ToSingle(buffer, pos + 0) + " ";
                    pos += 4;

                }
            }
            else if (((format == 144) || (format == 129)))
            {
                List<int> ax = new List<int>();
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    ax.Add(num);
                    facesData.Add(num);
                    pos += 4;
                    loop += ax[ax.Count - 1] + " ";
                }
                faces = new List<int>();
                if (false)
                    for (int j = 0; j < faces_old.Count / 4; j++)
                    {
                        facesOfFaces.Add(new List<int> { faces_old[j * 4 + 2], faces_old[j * 4 + 3], faces_old[j * 4 + 1], faces_old[j * 4 + 0] });
                    }
                if (true)
                {
                    for (int k = 0; k < ax.Count - 2; k++)
                    {


                        if (k % 2 == 0)
                        {
                            faces.Add(ax[k + 0]);
                            faces.Add(ax[k + 2]);
                            faces.Add(ax[k + 1]);

                        }
                        else
                        {
                            faces.Add(ax[k + 1]);
                            faces.Add(ax[k + 2]);
                            faces.Add(ax[k + 0]);
                        }
                    }
                    faces.Reverse();
                    faces_old.AddRange(faces);

                }
                mt = MeshTopology.Triangles;

            }
            else
            {
                for (int j = 0; j < j_null; j++)
                {
                    int num = System.BitConverter.ToInt32(buffer, pos);
                    faces_old.Add(num);
                    facesData.Add(num);

                    pos += 4;
                    loop += faces_old[faces_old.Count - 1] + " ";
                }
            }
            loops.Add(loop);

            faces_old.Reverse(); //развернем полигоны вовнутрь
            if (((format == 144) || (format == 129)) && false)
            {
                curMesh.subMeshCount = curMesh.subMeshCount + facesOfFaces.Count - 1;
                int j = 0;
                foreach (var a in facesOfFaces)
                {
                    curMesh.SetIndices(a.ToArray(), mt, i + j);
                    j++;
                }

            }
            else
            {

                curMesh.SetIndices(faces_old.ToArray(), mt, i, true);



                mats.Add(script.GetComponent<Materials>().maths[script.TexInts[matNum[i]]]);
                material.Add(script.GetComponent<Materials>().material[script.TexInts[matNum[i]]]);

            }
            facesData1.Add(facesData);
        }



        curMesh.uv = bt1.uv.ToArray();//uv_new;

        //bt.MatNum = b3dObject.TexInts[matNum[matNum.Count - 1]];
        gameObject.GetComponent<Renderer>().materials = mats.ToArray();

        curMesh.normals = bt1.normals.ToArray();
        try
        {
            curMesh.uv2 = bt1.uv1.ToArray();
        }
        catch { }
        curMesh.RecalculateBounds();
        gameObject.GetComponent<MeshFilter>().mesh = curMesh;



    }
}