using UnityEngine;
using System.Collections.Generic;
//LodManager10
class Block10 : MonoBehaviour, IBlocktype
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    bool rendered = false;
    LODCustom curLodObj = null;
    public GameObject[][] gameObjects = new GameObject[2][];
    public Vector3 Center;
    public float Distance;
    GameObject[][] gameObjects0 = new GameObject[2][];

    // Use this for initialization
    public void SetLods(int index, GameObject[] gameObjects0, Vector3 Center0, float Distance0)
    {
        gameObjects[index] = gameObjects0;
        Center = Center0;
        Distance = Distance0;
    }

    void Start()
    {
        /*if (!gameObject.GetComponent<LODGroup>())
			curLodObj = gameObject.AddComponent<LODGroup>();
		else
			copied = true;*/
        if (!gameObject.GetComponent<LODCustom>())
        {
            curLodObj = gameObject.AddComponent<LODCustom>();
            /*Destroy(gameObject.GetComponent<LODCustom>());
			Destroy(gameObject.GetComponent<SphereCollider>());*/
        }
        else
        {
            curLodObj = gameObject.GetComponent<LODCustom>();
            Destroy(curLodObj.gameObject.GetComponent<SphereCollider>());
        }
        //copied = true;
        //curLodObj = gameObject.AddComponent<LODCustom>();

        //	copied = true;

    }

    void OnRenderObject()
    {
        if ((!rendered))
        {
            List<GameObject> gobs = new List<GameObject>();
            rendered = true;

            int childCount = transform.childCount;
            int state = 0;
            for (int i = 0; i < childCount; i++)
            {
                if (transform.GetChild(i).name != "444")
                {
                    gobs.Add(transform.GetChild(i).gameObject);
                }
                else
                {
                    gameObjects0[state] = gobs.ToArray();
                    gobs = new List<GameObject>();
                    state++;
                }

            }
            gameObjects0[1] = gobs.ToArray();
            gobs = null;
            curLodObj.SetLods(gameObjects0, Center, Distance);
            curLodObj.MakeLod();
            curLodObj.SwitchState(0);

        }


    }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);

        buffer.AddRange(Instruments.Vector3ToBytesRevert(Center));
        buffer.AddRange(System.BitConverter.GetBytes(Distance));
        int childCount = 0;
        foreach (Transform ob in transform)
        {
            if (ob.name != "444")
            {
                childCount += 1;
            }
        }
        buffer.AddRange(System.BitConverter.GetBytes(childCount));


        return buffer.ToArray();

    }

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 16;


        Center = new Vector3(System.BitConverter.ToSingle(buffer, pos), System.BitConverter.ToSingle(buffer, pos + 8), System.BitConverter.ToSingle(buffer, pos + 4));
        Distance = System.BitConverter.ToSingle(buffer, pos + 12);
        pos += 16;
        //System.Array.Copy(resource,pos,buff,0,4); 	
        pos += 4;
    }
}
