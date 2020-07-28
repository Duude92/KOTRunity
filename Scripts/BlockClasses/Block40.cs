using UnityEngine;
using System.Collections.Generic;

public class Block40 : BlockType, IBlocktype, IDisableable
{
    UnityEngine.GameObject _thisObject;
    public UnityEngine.GameObject thisObject { get => _thisObject; set => _thisObject = value; }
    GeneratorInvoker GI;
    private string iName = "Plus";
    private AGenerator generator;
    private Vector3 gPosition;
    public static Dictionary<string, System.Type> generators = new Dictionary<string, System.Type>();
    //private string KeyByValue;
    private string KeyByValue(Dictionary<string, System.Type> dict, System.Type val)
    {
        string key = default;
        foreach (KeyValuePair<string, System.Type> pair in dict)
        {
            if (EqualityComparer<System.Type>.Default.Equals(pair.Value, val))
            {
                key = pair.Key;
                break;
            }
        }
        return key;
    }
    public Block40()
    {
        GameManager.RegisterDisableale(this);

    }
    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(Instruments.Vector3ToBytesRevert(thisObject.transform.position));
        buffer.AddRange(System.BitConverter.GetBytes(generator.scale));

        buffer.AddRange(new byte[32]);
        byte[] genName = new byte[32];
        System.Text.Encoding.ASCII.GetBytes(KeyByValue(generators, generator.GetType())).CopyTo(genName, 0);
        buffer.AddRange(genName);
        buffer.AddRange(generator.GetBytes());

        /*
        buffer.AddRange(System.BitConverter.GetBytes(generator.Type));
        buffer.AddRange(System.BitConverter.GetBytes(generator.hernja));
        int pCount = generator.Params.Count;
        buffer.AddRange(System.BitConverter.GetBytes(pCount));
        byte[] newBuff = new byte[pCount * 4];
        for (int i = 0; i < pCount; i++)
        {
            System.BitConverter.GetBytes(GI.Params[i]).CopyTo(newBuff, i * 4);
        }
        buffer.AddRange(newBuff);*/

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
        System.Type type;
        if (!generators.TryGetValue(Generator, out type))
        {
            type = typeof(GDefault);
            Debug.Log("Not found generator for: " + Generator + ". Scene couldnt be exported.", gameObject);
        }

        generator = (AGenerator)gameObject.AddComponent(type);
        generator.InitRead(buffer, ref pos);
        generator.ReadParameters(buffer, ref pos);
        generator.scale = scale;
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

    }

    public static void Register()
    {
        generators.Add("$$WeldingSparkles", typeof(WeldingSparkles));
        generators.Add("$$TreeGenerator1", typeof(TreeGenerator1));
        generators.Add("$$People", typeof(People));
        generators.Add("$$DynamicGlow", typeof(Glow));
        generators.Add("$$GeneratorOfTerrain", typeof(Terrain));

    }
    public void Disable()
    {
        GI.Destroy();
    }

    public void Enable()
    {
        GI.Generate();
    }
    public override void ClosingEvent()
    {
        base.ClosingEvent();
    }







}




abstract class AGenerator : MonoBehaviour, IDisableable
{
    [SerializeField] private List<float> Params = new List<float>();
    public abstract Vector3 position { get; set; }
    public abstract string Name { get; set; }
    protected int paramCount;
    [SerializeField] protected int gType, xz;
    public float scale;
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
    public virtual byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(gType));
        buffer.AddRange(System.BitConverter.GetBytes(xz));
        buffer.AddRange(System.BitConverter.GetBytes(Params.Count));
        for (int i = 0; i < paramCount; i++)
        {
            buffer.AddRange(System.BitConverter.GetBytes(Params[i]));
        }


        return buffer.ToArray();
    }
    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawIcon(position + Vector3.up * 2, Name, true);

    }

    public void Disable()
    {

    }

    public void Enable()
    {
        //TODO: генерировать всю чепуху в гизмо
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
    public override byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(System.BitConverter.GetBytes(gType));
        buffer.AddRange(System.BitConverter.GetBytes(xz));
        buffer.AddRange(Instruments.Vector3ToBytesRevert(position));
        buffer.AddRange(Instruments.Vector2ToBytes(someVector));
        buffer.AddRange(System.BitConverter.GetBytes(someVar));


        return buffer.ToArray();
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