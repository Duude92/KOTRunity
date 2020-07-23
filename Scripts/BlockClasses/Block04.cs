using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//joinBlock
[ExecuteInEditMode]
public class Block04 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
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
        pos += 32;

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
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(transform.position));
        buffer.AddRange(new byte[4]);

        byte[] bf = new byte[32];
        System.Text.Encoding.UTF8.GetBytes(nameToJoin).CopyTo(bf, 0);

        buffer.AddRange(bf);
        buffer.AddRange(new byte[32]);
        buffer.AddRange(System.BitConverter.GetBytes(thisObject.transform.childCount));



        return buffer.ToArray();


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
