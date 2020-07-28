using UnityEngine;
using System.Collections.Generic;
class Block40 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    GeneratorInvoker GI;
    private string iName = "Plus";
    private AGenerator generator;
    private Vector3 gPosition;
    public Block40()
    {
        GameManager.RegisterDisableale(this);

    }
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytesRevert(thisObject.transform.position));
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
        switch (Generator)
        {
            case "$$WeldingSparkles":
                {
                    generator = gameObject.AddComponent<WeldingSparkles>();

                    break;
                }
            case "$$TreeGenerator1":
                {
                    generator = gameObject.AddComponent<TreeGenerator1>();
                    break;
                }
            case "$$People":
                {
                    generator = gameObject.AddComponent<People>();
                    break;
                }
            case "$$DynamicGlow":
                {
                    generator = gameObject.AddComponent<Glow>();
                    break;
                }
            default:
                {
                    generator = gameObject.AddComponent<GDefault>();
                    Debug.Log("Not found generator for: " + Generator, gameObject);
                    break;
                }
        }
        generator.InitRead(buffer, ref pos);
        generator.ReadParameters(buffer, ref pos);
        iName = generator.Name;
        gPosition = generator.position;


        if (false)
        {

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

        if (Generator == "$$GeneratorOfTerrain")
        {
            iName = "Terrain";
        }

        else
        {
        }

    }
    void OnDrawGizmos()
    {

        //Gizmos.DrawIcon(gPosition + Vector3.up * 2, iName, true);
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
abstract class AGenerator : MonoBehaviour
{
    [SerializeField] private List<float> Params = new List<float>();
    public abstract Vector3 position { get; set; }
    public abstract string Name { get; set; }
    protected int paramCount;
    [SerializeField] private int gType, xz;
    public virtual void InitRead(byte[] buffer, ref int pos)
    {
        gType = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        xz = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        paramCount = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;

    }
    public virtual void ReadParameters(byte[] buffer, ref int pos)
    {
        for (int i = 0; i < paramCount; i++)
        {
            Params.Add(System.BitConverter.ToSingle(buffer, pos)); pos += 4;
        }
    }
    public abstract void Generate();
    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawIcon(position + Vector3.up * 2, Name, true);

    }
}
class Terrain : AGenerator
{
    public override Vector3 position { get => transform.position; set { } }
    public override string Name { get => "Terrain"; set { } }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }
}
class Glow : AGenerator
{
    public override Vector3 position { get => transform.position; set { } }
    public override string Name { get => "Glow"; set { } }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }
}
class People : AGenerator
{
    public override Vector3 position { get => transform.position; set { } }
    public override string Name { get => "People"; set { } }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }

}
class GDefault : AGenerator
{
    public override Vector3 position { get => transform.position; set => throw new System.NotImplementedException(); }
    public override string Name { get => "defaultName"; set => throw new System.NotImplementedException(); }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }
}
class TreeGenerator1 : AGenerator
{
    public override string Name { get => "Tree"; set => throw new System.NotImplementedException(); }
    public override Vector3 position { get => transform.position; set { } }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }


}
class WeldingSparkles : AGenerator
{
    private Vector3 _position;
    [SerializeField] private Vector2 someVector;
    [SerializeField] private int someVar;
    public override string Name { get => "Sparkles"; set { } }
    public override Vector3 position { get => _position; set => _position = value; }
    [SerializeField] private List<float> Params = new List<float>();
    public override void Generate()
    {
        throw new System.NotImplementedException();
    }

    public override void ReadParameters(byte[] buffer, ref int pos)
    {

        position = Instruments.ReadV3(buffer, pos);
        pos += 12;
        paramCount -= 3;
        someVector = Instruments.ReadV2(buffer, pos);
        pos += 8;
        paramCount -= 2;
        someVar = System.BitConverter.ToInt32(buffer, pos);
        pos += 4;
        paramCount -= 1;
        if (paramCount != 0)
        {
            Debug.LogWarning("This WeldingSparkles have more parameters", gameObject);
        }


    }


}