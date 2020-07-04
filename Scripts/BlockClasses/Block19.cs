using UnityEngine;
using System.Collections.Generic;
//rooms
class Block19 : MonoBehaviour, IBlocktype
{
    public GameObject room;
    public List<GameObject> hits = new List<GameObject>();

    public byte[] GetBytes()
    {
        //Debug.Log("19 block not implemented for GetBytes()");
        return new byte[0];
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
}
