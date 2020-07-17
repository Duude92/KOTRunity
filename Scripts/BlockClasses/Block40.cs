using UnityEngine;
using System.Collections.Generic;
class Block40 : IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public B3DScript script;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    GeneratorInvoker GI;
    public Block40()
    {
        GameManager.RegisterDisableale(this);

    }
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytes(thisObject.transform.position));
        buffer.AddRange(System.BitConverter.GetBytes(GI.Scale));

        buffer.AddRange(new byte[32]);
        byte[] genName = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(GI.invokeName).CopyTo(genName, 0);
        buffer.AddRange(genName);
        buffer.AddRange(System.BitConverter.GetBytes(GI.Type));
        buffer.AddRange(System.BitConverter.GetBytes(GI.hernja));
        int pCount = GI.Params.Count;
        buffer.AddRange(System.BitConverter.GetBytes(pCount));
        byte[] newBuff = new byte[pCount * 4];
        for (int i = 0; i < pCount; i++)
        {
            System.BitConverter.GetBytes(GI.Params[i]).CopyTo(newBuff, i * 4);
        }
        buffer.AddRange(newBuff);

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {

        List<float> Params = new List<float>();
        byte[] newBuff = new byte[16];
        //
        System.Array.Copy(buffer, pos, newBuff, 0, 16);
        pos += 16;


        Vector3 position = new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4));
        thisObject.transform.position = position;
        float scale = System.BitConverter.ToSingle(newBuff, 12);

        pos += 32;  //null

        byte[] GenBytes = new byte[32];
        //
        System.Array.Copy(buffer, pos, GenBytes, 0, 32);
        pos += 32;

        string Generator = System.Text.Encoding.UTF8.GetString(GenBytes).Trim(new char[] { '\0' });

        int gType = System.BitConverter.ToInt32(buffer, pos); pos += 4;
        int xz = System.BitConverter.ToInt32(buffer, pos); pos += 4;
        int paramCount = System.BitConverter.ToInt32(buffer, pos); pos += 4;
        for (int i = 0; i < paramCount; i++)
        {
            Params.Add(System.BitConverter.ToSingle(buffer, pos)); pos += 4;
        }








        GI = thisObject.AddComponent<GeneratorInvoker>();
        GI.resOb = script.gameObject;

        GI.Scale = scale;
        GI.invokeName = Generator;
        GI.Type = gType;
        GI.hernja = xz;
        GI.Params = Params;
    }

    public void Disable()
    {
        GI.Destroy();
    }

    public void Enable()
    {
        GI.Generate();
    }
}