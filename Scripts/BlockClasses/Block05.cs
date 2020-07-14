using UnityEngine;
//joinBlock
[ExecuteInEditMode]
class Block05 : BlockType, IBlocktype, IDisableable
{        UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public string nameToJoin;
    public GameObject obj;
    bool rendered = false;

    public void Read(byte[] buffer, ref int pos)
    {
        pos += 16;
        byte[] JoinName = new byte[32];
        System.Array.Copy(buffer, pos, JoinName, 0, 32);
        pos += 32;
        nameToJoin = System.Text.Encoding.UTF8.GetString(JoinName).Trim(new char[] { '\0' });
        pos += 4;
    }

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
        byte[] count = System.BitConverter.GetBytes(thisObject.transform.childCount);
        byte[] buff2 = new byte[buffer.Length+4];
        buffer.CopyTo(buff2,0);
        count.CopyTo(buff2,buffer.Length-1);
        buffer = buff2;
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
