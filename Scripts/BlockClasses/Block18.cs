using System.Collections.Generic;
using UnityEngine;
class Block18 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    InvokeMe me;
    public Block18()
    {
        GameManager.RegisterDisableale(this);

    }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(new Vector3()));
        buffer.AddRange(new byte[4]);
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
        byte[] tempPosB = new byte[16];
        System.Array.Copy(buffer, pos, tempPosB, 0, 16);
        pos += 16;
        Vector3 tempPos = new Vector3(System.BitConverter.ToSingle(tempPosB, 0), System.BitConverter.ToSingle(tempPosB, 8), System.BitConverter.ToSingle(tempPosB, 4));
        float scale = System.BitConverter.ToSingle(tempPosB, 12);
        byte[] newbuff = new byte[32];
        System.Array.Copy(buffer, pos, newbuff, 0, 32);
        pos += 32;
        string space = System.Text.Encoding.UTF8.GetString(newbuff).Replace("\x0", string.Empty);
        System.Array.Copy(buffer, pos, newbuff, 0, 32);
        pos += 32;
        string block = System.Text.Encoding.UTF8.GetString(newbuff).Replace("\x0", string.Empty);
        me = thisObject.AddComponent<InvokeMe>();
        me.space = space;
        //me.tempSpace = tempPos;
        //me.tempScale = scale;
        me.blocks = block;
        me.GO = script.gameObject;
        script.InvokeBlocks.Add(thisObject);
    }

    public void Disable()
    {
        me.Destroy();
    }

    public void Enable()
    {
        me.Invoke();
    }
}