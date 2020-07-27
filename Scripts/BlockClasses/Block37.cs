using System.Collections.Generic;
using UnityEngine;
public class Block37 : BlockType, IVerticesBlock, IBlocktype
{
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    //-------- IVertices
    private List<Vector3> _vertices = new List<Vector3>();
    public List<Vector3> vertices { get => _vertices; set => _vertices = value; }
    private List<Vector2> _uv = new List<Vector2>();
    public List<Vector2> uv { get => _uv; set => _uv = value; }
    private List<Vector3> _normals = new List<Vector3>();
    public List<Vector3> normals { get => _normals; set => _normals = value; }
    private List<Vector2> _uv2 = new List<Vector2>();
    public List<Vector2> uv1 { get => _uv2; set => _uv2 = value; }

    //-------- 

    public string collisionName;
    [SerializeField] private int i_null;
    [SerializeField] private bool bumped = false;

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);

        byte[] colName = new byte[32];
        if (string.IsNullOrEmpty(collisionName))
        {
            collisionName = "";
        }
        System.Text.Encoding.ASCII.GetBytes(collisionName).CopyTo(colName, 0);
        buffer.AddRange(colName);
        int i_null = 0;
        if (false)//(mesh.Count == 0)
        {
            Debug.Log("This object have vertices block, but does not have faces", thisObject);

            buffer.AddRange(System.BitConverter.GetBytes(3)); //Some value i_null
            buffer.AddRange(System.BitConverter.GetBytes(0)); //vcount
            buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
            return buffer.ToArray();

        }
        if (bumped == true)
        {
            i_null = 514;
        }
        else if (vertices.Count > normals.Count)
        {
            i_null = 3;
        }
        else if (uv1.Count > 0)
        {
            i_null = 258;
        }
        else
        {
            i_null = 2;
        }
        buffer.AddRange(System.BitConverter.GetBytes(i_null)); //Some value i_null
        int vCount = vertices.Count;
        buffer.AddRange(System.BitConverter.GetBytes(vCount)); //Some value j_null
        if (i_null == 2) //vertex + uv + normal
        {
            for (int i = 0; i < vCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].y));
            }
        }
        else if (i_null == 258)
        {
            for (int i = 0; i < vCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv1[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv1[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].y));
            }

        }
        else if (i_null == 514)
        {
            for (int i = 0; i < vCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv1[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv1[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(0f));
                buffer.AddRange(System.BitConverter.GetBytes(0f));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(normals[i].y));
            }

        }
        else if (i_null == 3) //NO normals
        {
            for (int i = 0; i < vCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(1f)); //TODO: Unknown data
            }

        }

        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        this.unknownVector = Instruments.ReadV4(buffer, pos);
        pos += 16;
        byte[] colName = new byte[32];
        System.Array.Copy(buffer, pos, colName, 0, 32);
        collisionName = System.Text.Encoding.UTF8.GetString(colName);
        pos += 32;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        i_null = System.BitConverter.ToInt32(buff, 0);
        int j_null;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        j_null = System.BitConverter.ToInt32(buff, 0);
        if (i_null == 0)
        {
            ;
        }
        else if (i_null == 2)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[32];
                System.Array.Copy(buffer, pos, newBuff, 0, 32);
                pos += 32;
                var vertex = Instruments.ReadV3(newBuff, 0);
                var normal = Instruments.ReadV3(newBuff, 20);

                vertices.Add(vertex);
                uv.Add(Instruments.ReadV2(newBuff, 12));
                normals.Add(normal);
                //pos+=32; 
            }
        }
        else if (i_null == 3)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[24];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 24);
                pos += 24;
                vertices.Add(Instruments.ReadV3(newBuff, 0));
                uv.Add(Instruments.ReadV2(newBuff, 12));
                //
            }
            normals = new List<Vector3>();

        }
        else if (i_null == 514)
        {
            bumped = true;
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[48];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 48); //TODO: структура 514: вершина 3ф, ув 2ф, ув2 2ф, нормаль ли? 3ф, неизвестно 2ф
                pos += 48;
                var vertex = Instruments.ReadV3(newBuff, 0);
                var normal = Instruments.ReadV3(newBuff, 34);

                vertices.Add(vertex);
                uv.Add(Instruments.ReadV2(newBuff, 12));
                uv1.Add(Instruments.ReadV2(newBuff, 20));

                normals.Add(normal);
                //
            }
        }
        else if ((i_null == 258) || (i_null == 515))
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[40];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 40);
                pos += 40;
                var vertex = Instruments.ReadV3(newBuff, 0);
                var normal = Instruments.ReadV3(newBuff, 28); //TODO: структура 258: вершина (3ф), ув(2ф), ув2(2ф), нормаль(3ф)
                vertices.Add(vertex);
                uv.Add(Instruments.ReadV2(newBuff, 12));
                uv1.Add(Instruments.ReadV2(newBuff, 20));
                normals.Add(normal);
                //
            }
        }
        //int childCount = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;


    }
}