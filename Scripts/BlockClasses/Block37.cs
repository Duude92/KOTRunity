using System.Collections.Generic;
using UnityEngine;
public class Block37 : VerticesBlock, IBlocktype
{
    public B3DScript script;
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public string collisionName;

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);

        byte[] colName = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(collisionName).CopyTo(colName, 0);
        buffer.AddRange(colName);
        int i_null = 0;
        if (mesh.Count == 0)
        {
            Debug.Log("This object have vertices block, but does not have faces", thisObject);

            buffer.AddRange(System.BitConverter.GetBytes(3)); //Some value i_null
            buffer.AddRange(System.BitConverter.GetBytes(0)); //vcount
            buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
            return buffer.ToArray();

        }
        if (mesh[0].vertices.Length > mesh[0].normals.Length)
        {
            i_null = 3;
        }
        else if (mesh[0].uv2.Length > 0)
        {
            i_null = 258;
        }
        else
        {
            i_null = 2;
        }
        buffer.AddRange(System.BitConverter.GetBytes(i_null)); //Some value i_null
        int vCount = mesh[0].vertices.Length;
        buffer.AddRange(System.BitConverter.GetBytes(vCount)); //Some value j_null
        var _mesh = mesh[0];
        if (i_null == 2) //vertex + uv + normal
        {
            for (int i = 0; i < _mesh.vertexCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].y));
            }
        }
        else if (i_null == 258)
        {
            for (int i = 0; i < _mesh.vertexCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv2[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv2[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.normals[i].y));
            }

        }
        else if (i_null == 3) //NO normals
        {
            for (int i = 0; i < _mesh.vertexCount; i++)
            {

                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].z));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].x));
                buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].y));
                buffer.AddRange(System.BitConverter.GetBytes(1f)); //TODO: Unknown data
            }

        }

        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        byte[] colName = new byte[32];
        System.Array.Copy(buffer, pos, colName, 0, 32);
        collisionName = System.Text.Encoding.UTF8.GetString(colName);
        pos += 32;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        int i_null = System.BitConverter.ToInt32(buff, 0);
        int j_null;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        j_null = System.BitConverter.ToInt32(buff, 0);
        script.vertices = new List<Vector3>();
        script.UV = new List<Vector2>();
        script.UV1 = new List<Vector2>();
        script.normals = new List<Vector3>();
        //script.vertices = new List<Vector3>[j_null];
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

                script.vertices.Add(vertex);
                script.UV.Add(Instruments.ReadV2(newBuff, 12));
                script.normals.Add(normal);
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
                script.vertices.Add(Instruments.ReadV3(newBuff, 0));
                script.UV.Add(Instruments.ReadV2(newBuff, 12));
                //
            }
            script.normals = new List<Vector3>();

        }
        else if (i_null == 514)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[48];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 48);
                pos += 48;
                var vertex = Instruments.ReadV3(newBuff, 0);
                var normal = Instruments.ReadV3(newBuff, 20);

                script.vertices.Add(vertex);
                script.UV.Add(Instruments.ReadV2(newBuff, 12));
                script.normals.Add(normal);
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
                var normal = Instruments.ReadV3(newBuff, 28); //TODO: структура 258: вершина (3ф), ув(2ф), неизвестно(2ф), нормаль(3ф)
                script.vertices.Add(vertex);
                script.UV.Add(Instruments.ReadV2(newBuff, 12));
                script.UV1.Add(Instruments.ReadV2(newBuff, 20));
                script.normals.Add(normal);
                //
            }
        }
        int childCount = System.BitConverter.ToInt32(buffer, pos);
        script.UV1Users = childCount;
        pos += 4;


    }
}