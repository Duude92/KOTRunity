using System.Collections.Generic;
using UnityEngine;
class Block37 : VerticesBlock, IBlocktype
{
    public B3DScript script;
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[36]);
        buffer.AddRange(System.BitConverter.GetBytes(2)); //Some value i_null
        int loopCount = 0;
        foreach(var _mesh in mesh)
        {
            loopCount+=_mesh.triangles.Length/3;
        }
        buffer.AddRange(System.BitConverter.GetBytes(loopCount)); //Some value j_null
        foreach (var _mesh in mesh)
        {
            for (int i = 0; i < _mesh.triangles.Length/3; i++)
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
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        byte[] buff = new byte[4];
        pos += 16;
        pos += 32;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        int i_null = System.BitConverter.ToInt32(buff, 0);
        //Debug.Log(i_null);
        int j_null;
        System.Array.Copy(buffer, pos, buff, 0, 4);
        pos += 4;
        j_null = System.BitConverter.ToInt32(buff, 0);
        script.vertices = new List<Vector3>();
        script.UV = new List<Vector2>();
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
                script.vertices.Add(Instruments.ReadV3(newBuff,0));
                script.UV.Add(Instruments.ReadV2(newBuff,12));
                script.normals.Add(Instruments.ReadV3(newBuff,20));
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
                script.vertices.Add(Instruments.ReadV3(newBuff,0));
                script.UV.Add(Instruments.ReadV2(newBuff,12));
                //
            }
            script.normals = null;

        }
        else if (i_null == 514)
        {
            for (int i = 0; i < j_null; i++)
            {
                byte[] newBuff = new byte[48];
                //
                System.Array.Copy(buffer, pos, newBuff, 0, 48);
                pos += 48;
                script.vertices.Add(Instruments.ReadV3(newBuff,0));
                script.UV.Add(Instruments.ReadV2(newBuff,12));
                script.normals.Add(Instruments.ReadV3(newBuff,20));
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
                script.vertices.Add(Instruments.ReadV3(newBuff,0));
                script.UV.Add(Instruments.ReadV2(newBuff,12));
                script.normals.Add(Instruments.ReadV3(newBuff,20));
                //
            }
        }
        pos += 4;


    }
}