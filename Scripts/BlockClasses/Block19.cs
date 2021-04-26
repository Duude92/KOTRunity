using UnityEngine;
using System.Collections.Generic;
//rooms
class Block19 : BlockType, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    public List<GameObject> hits = new List<GameObject>();

    public byte[] GetBytes()
    {
        //Debug.Log("19 block not implemented for GetBytes()");
        byte[] buffer = new byte[0];
        byte[] count = System.BitConverter.GetBytes(thisObject.transform.childCount);
        byte[] buff2 = new byte[buffer.Length + 4];
        buffer.CopyTo(buff2, 0);
        count.CopyTo(buff2, buffer.Length);
        buffer = buff2;
        return buffer;
    }

    public void Init()
    {
        //room = gameObject;
        string[] newstr = name.Split('_');

        Transform roadT;
        Transform objT;
        if (newstr.Length == 3)
        {
            //Debug.Log(room.name);
            roadT = transform.parent.Find("hit_road_" + newstr[1] + "_" + newstr[2]);
            objT = transform.parent.Find("hit_obj_" + newstr[1] + "_" + newstr[2]);

        }
        else
        {
            roadT = transform.parent.Find("hit_road_" + newstr[1]);
            objT = transform.parent.Find("hit_obj_" + newstr[1]);
        }
        if (objT)
        {

            hits.Add(objT.gameObject);
        }
        if (roadT)
        {
            hits.Add(roadT.gameObject);
        }
    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 4;
    }
}
