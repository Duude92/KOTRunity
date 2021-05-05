using UnityEngine;
using System.Collections.Generic;
//<summary> Used for events </summary>
class Block13 : BlockType, IBlocktype
{
    public enum EventType
    {
        unknown = 0,
        NoRain = 4,                 //limit09
        NoSunLins = 5,
        FonMain = 6,
        CheckPoint = 7,
        Restart_010 = 10,
        Restart = 11,               //Restart
        Room,
        BenzoEvent = 14,           //limit09
        STOS,                       //default
        Water,
        Store,                      //default
        InfoStore = 19,             //default
        NoRainNoSun = 22,           //possible limited???? //TODO
        WeatherChange,
        Restart_024,                //Restart
        StartSvetofor = 26,
        NoEntry = 27,               //unknown
        Unknown = 28,               //Unknown, BD.B3D
        SomeEventRoadType = 29,
        RadarEvent,                 //default -> RadarEvent
        Spikes,                     //spikes
        OtherBaza = 4095,           //OtherBaza
    }
    public int b;
    private int paramCount;
    public EventType eventType;
    private Block13SubclassBase eventTrigger;
    private List<float> Params = new List<float>();
    GameObject _thisObject;
    public GameObject thisObject { get => _thisObject; set => _thisObject = value; }

    public byte[] GetBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(new byte[16]);
        buffer.AddRange(System.BitConverter.GetBytes((int)eventType));
        buffer.AddRange(System.BitConverter.GetBytes(b));
        // buffer.AddRange(System.BitConverter.GetBytes(paramCount));
        // foreach (float p in Params)
        // {
        //     buffer.AddRange(System.BitConverter.GetBytes(p));
        // }

        buffer.AddRange(eventTrigger.GetByte());

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
            Debug.LogWarning("no " + a + " event for " + gameObject, gameObject);
        }
        b = System.BitConverter.ToInt32(buffer, pos + 4);
        pos += 8;//
        System.Type componentType = null;
        switch (eventType)
        {
            case EventType.Restart:
                componentType = typeof(RestartSubclass);
                break;
            case EventType.Restart_024:
                componentType = typeof(RestartSubclass);
                break;
            case EventType.OtherBaza:
                componentType = typeof(OtherBazaSubclass);
                break;
            case EventType.Spikes:
                componentType = typeof(SpikesSubclass);
                break;
            case EventType.NoRain:
                componentType = typeof(Block13SubclassLimited09);
                break;
            case EventType.NoRainNoSun:
                componentType = typeof(Block13SubclassLimited09);
                break;
            case EventType.BenzoEvent:
                componentType = typeof(Block13SubclassLimited09);
                break;
            case EventType.RadarEvent:
                componentType = typeof(RadarEvent);
                break;
            default:
                componentType = typeof(DefaultSubclass);
                break;

        }
        if (componentType != null)
        {
            eventTrigger = gameObject.AddComponent(componentType) as Block13SubclassBase;
            eventTrigger?.Read(buffer, ref pos);
        }
    }
    public override void ClosingEvent()
    {
        foreach (Vector3 vector in script.triggerBox)
        {

        }
    }
}
