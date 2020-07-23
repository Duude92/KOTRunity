using UnityEngine;
using System.Collections;
//joinBlock
[ExecuteInEditMode]
public class Block05 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public string nameToJoin;
    public GameObject obj;
    bool rendered = false;

    public void Read(byte[] buffer, ref int pos)
    {
        this.unknownVector = Instruments.ReadV3(buffer,pos);
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
        if(string.IsNullOrEmpty(nameToJoin))
        {
            nameToJoin = "";
        }
        System.Text.Encoding.UTF8.GetBytes(nameToJoin, 0, nameToJoin.Length, buf, 0);
        buf.CopyTo(buffer, 16);

        byte[] count = System.BitConverter.GetBytes(thisObject.transform.childCount);
        byte[] buff2 = new byte[buffer.Length + 4];
        buffer.CopyTo(buff2, 0);
        count.CopyTo(buff2, buffer.Length);
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
        if (nameToJoin == "hitAnmObj0241")
        {
            Debug.Log("hitAnmObj0241");
        }
        if (!string.IsNullOrEmpty(nameToJoin))
        {
            Debug.Log("", gameObject);
            StartCoroutine(Enable(nameToJoin));
        }
    }
    IEnumerator Enable(string Name, bool common = false)
    {
        if (gameObject.activeSelf)
        {


            GameObject obj2;
            Transform transform;
            if (!common)
            {
                transform = GameManager.currentObject.transform.Find(Name);

            }
            else
            {
                transform = GameManager.common.transform.Find(Name);
            }
            if (!common && !transform)
            {
                yield return StartCoroutine(Enable(nameToJoin, true));

            }
            else
            {
                if (common && !transform)
                {
                    throw new System.Exception("ObjectName not found for object " + name);

                }
                obj2 = transform.gameObject;
                obj2.SetActive(false);
                obj2 = GameObject.Instantiate(obj2);
                this.obj = obj2;
                obj2.SetActive(true);
                obj2.transform.SetParent(this.transform);
            }
        }
        yield return null;
    }
}
