using UnityEngine;
using System.Collections.Generic;
//<summary> Used for events </summary>
class Block13 : BlockType, IBlocktype
{
    public enum EventType
    {
        NoRain = 4,
        Restart = 11,
        Room,
        Benzo_event = 14,
        STOS,
        Water,
        Store,
        InfoStore = 19,
        NoRainNoSun = 22,
        SomeEvent_roadType = 29,
        RadarEvent,
        Spikes,
        OtherBaza = 4095,


    }
    public int b, paramCount;
    public EventType eventType;
    public List<float> Params = new List<float>();
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(System.BitConverter.GetBytes((int)eventType));
        buffer.AddRange(System.BitConverter.GetBytes(b));
        buffer.AddRange(System.BitConverter.GetBytes(paramCount));
        foreach (float p in Params)
        {
            buffer.AddRange(System.BitConverter.GetBytes(p));
        }

        return buffer.ToArray();
    }

    public void Read(byte[] buffer, ref int pos)
    {
        this.Type = 13;
        this.unknownVector = Instruments.ReadV4(buffer, pos);

        pos += 16;
        int a = System.BitConverter.ToInt32(buffer, pos);
        if (System.Enum.IsDefined(typeof(EventType), a))
        {
            eventType = (EventType)a;
        }
        else
        {
            Debug.LogWarning("no " + a + " event for ", gameObject);
        }
        b = System.BitConverter.ToInt32(buffer, pos + 4);
        int paramCount2 = System.BitConverter.ToInt32(buffer, pos + 8);
        paramCount = paramCount2;
        pos += 4;
        pos += 8;
        //int i_null = System.BitConverter.ToInt32(buff,0);
        for (int i = 0; i < paramCount; i++)
        {
            Params.Add(System.BitConverter.ToSingle(buffer, pos));
            pos += 4;
        }

    }
    public override void ClosingEvent()
    {
        foreach (Vector3 vector in script.triggerBox)
        {

        }
    }
}
