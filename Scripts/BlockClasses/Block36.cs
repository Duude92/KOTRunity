using System.Collections.Generic;
using UnityEngine;
public class Block36 : VerticesBlock, IBlocktype
{
    public B3DScript script;
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[36]);
        buffer.AddRange(new byte[32]);
        int i_null = 0;
        Debug.Log("", _thisObject);
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
        int vCount = 0;
        foreach (var _mesh in mesh)
        {
            vCount += _mesh.vertices.Length;
        }
        buffer.AddRange(System.BitConverter.GetBytes(vCount)); //Some value j_null
        foreach (var _mesh in mesh)
        {
            if (i_null == 2) //vertex + uv + normal
            {
                for (int i = 0; i < vCount; i++)
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
                for (int i = 0; i < vCount; i++)
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
                for (int i = 0; i < vCount; i++)
                {

                    buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].x));
                    buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].z));
                    buffer.AddRange(System.BitConverter.GetBytes(_mesh.vertices[i].y));
                    buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].x));
                    buffer.AddRange(System.BitConverter.GetBytes(_mesh.uv[i].y));
                    buffer.AddRange(System.BitConverter.GetBytes(1f)); //TODO: Unknown data
                }

            }
        }
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        pos += 32;
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
        script.normals = new List<Vector3>();
        if (i_null == 0)
        {
            ;
        }
        else if (i_null == 2)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[32];
                System.Array.Copy(buffer, pos, newBuff, 0, 32); pos += 32;
                script.vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                script.UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                script.normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
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
                script.vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                script.UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                //
            }
        }
        else if (i_null == 514)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[48];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 48);
                pos += 48;
                script.vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                script.UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                script.normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
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
                script.vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                script.UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                script.normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
                //
            }
        }
        pos += 4;


    }
}