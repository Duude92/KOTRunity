using UnityEngine;
using System.Collections.Generic;
using System;
public static class BlockFabrique
{
    public static Dictionary<int, Type> tRegistry = new Dictionary<int, Type>();
    public static B3DScript script;
    public static void Register(Type type, int num)
    {
        tRegistry.Add(num, type);
    }

    public static BlockType GetBlock(GameObject gameObject, int type)
    {
        string classString = "Block" + (type > 9 ? type.ToString() : ("0" + type)); 
        Type blType = Type.GetType(classString, true);
        BlockType bt;
        Component cmp = gameObject.AddComponent(blType);
        bt = cmp as BlockType;
        ((IBlocktype)bt).thisObject = gameObject;
        bt.script = script;
        bt.Type = type;
        return bt;
        //bt.component.Read(resource, ref pos);
    }


}