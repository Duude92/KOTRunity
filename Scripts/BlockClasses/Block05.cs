using UnityEngine;
//joinBlock
[ExecuteInEditMode]
class Block05 : MonoBehaviour, IBlocktype, IDisableable
{
    public string nameToJoin;
    public GameObject obj;
    bool rendered = false;

    void Start()
    {
        GameManager.RegisterDisableale(this);
    }

    public void Disable()
    {
        DestroyImmediate(obj);
    }

    byte[] IBlocktype.GetBytes()
    {
        byte[] buffer = new byte[48];
        byte[] buf = new byte[12];
        buf = Instruments.Vector3ToBytes(transform.position);
        buf.CopyTo(buffer, 0);
        buf = new byte[32];
        System.Text.Encoding.UTF8.GetBytes(nameToJoin, 0, nameToJoin.Length, buf, 0);
        buf.CopyTo(buffer, 16);
        return buffer;


    }

    void OnRenderObject()
    {
        if (!rendered)
        {
            if (obj)
            {
                int childCount = transform.childCount;
                bool child = false;

                if (obj.transform.parent == transform)
                {
                    child = true;
                }
                if (!child)
                {

                    GameObject hit_obj_ = Instantiate(obj);
                    hit_obj_.transform.localPosition = new Vector3(0, 0, 0);
                    hit_obj_.transform.SetParent(transform);
                    obj = hit_obj_;

                }
            }
        }
        rendered = true;
    }

    public void Enable()
    {
        GameObject obj2 = GameObject.Find(nameToJoin).gameObject;
        obj2.SetActive(false);
        obj2 = GameObject.Instantiate(obj2);
        this.obj = obj2;
        obj2.transform.SetParent(this.transform);
    }
}
