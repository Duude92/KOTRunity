using System.Collections.Generic;
using UnityEngine;
public class Block18 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public Transform GetTransform => transform;

    public GameObject GetGameObject => gameObject;

    private InvokeMe me;
    private Vector3 gizmoPosition;
    public Block24 gizmosTransform;
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private float scale;
    public Block18()
    {
        GameManager.RegisterDisableale(this);

    }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector4ToBytes(this.unknownVector));
        byte[] textBuffer = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(me.space).CopyTo(textBuffer, 0);
        buffer.AddRange(textBuffer);
        textBuffer = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(me.blocks).CopyTo(textBuffer, 0);
        buffer.AddRange(textBuffer);
        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        this.unknownVector = Instruments.ReadV4(buffer, pos);
        pos += 12;
        scale = unknownVector.w;
        pos += 4;
        byte[] newbuff = new byte[32];
        System.Array.Copy(buffer, pos, newbuff, 0, 32);
        pos += 32;
        string space = System.Text.Encoding.UTF8.GetString(newbuff).Replace("\x0", string.Empty);
        System.Array.Copy(buffer, pos, newbuff, 0, 32);
        pos += 32;
        string block = System.Text.Encoding.UTF8.GetString(newbuff).Replace("\x0", string.Empty);
        me = thisObject.AddComponent<InvokeMe>();
        me.space = space;
        me.blocks = block;
        me.GO = script.transform;
        script.InvokeBlocks.Add(thisObject);
        if (!string.IsNullOrEmpty(space))
        {
            Transform tr = GameManager.currentObject.transform.Find(space);
            if (!tr)
            {
                tr = GameManager.common.transform.Find(space);
            }
            if (tr)
            {
                gizmosTransform = tr.GetComponent<Block24>();
                position = gizmosTransform.position;
                //transform.position = gizmosTransform.position;
            }
            else
            {
                Debug.Log("No " + space + " position for", gameObject);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (gizmosTransform)
        {
            Gizmos.DrawIcon(gizmosTransform.position + Vector3.up * 2, "Copy18", true);
            //gizmosTransform.position = transform.position;
            //transform.position = gizmosTransform.position;
        }
    }
    public void Disable()
    {
        if(!me)
        {
            Debug.Log(this,this);
            return;
        }
        me.Destroy();
    }

    public void Enable()
    {
        if(!me)
        me = GetComponent<InvokeMe>();
        try
        {
            me.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e, this);
            throw e;
        }
    }
}