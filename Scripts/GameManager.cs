﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{
    private static List<IDisableable> disableList = new List<IDisableable>();
    public static GameManager instance;
    [SerializeField] public static Shader TC; //TOREMOVE
    [SerializeField] public static Shader TCu;//TOREMOVE
    public List<GameObject> MainRooms = new List<GameObject>();
    public List<GameObject> Rooms = new List<GameObject>();
    public static GameObject common;
    public static GameObject currentObject;

    static public void RegisterDisableale(IDisableable block)
    {
        disableList.Add(block);
    }

    void OnApplicationQuit()
    {
        GC.Collect();
    }
    void Init()
    {
        if (instance)
        {
            DestroyImmediate(instance.gameObject);
            instance = this;
        }
    }
    void Start()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            List<GameObject> goL = transform.GetChild(i).GetComponent<B3DScript>().InvokeBlocks;
            foreach (var go in goL)
            {
                go.GetComponent<InvokeMe>().Invoke();
            }
            transform.GetChild(i).GetComponent<B3DScript>().InvokeBlocks = null;
        }
    }
    // void Awake()
    // {

    //     TC = TC2;
    //     TCu = TCu2;

    //     common = new GameObject("COMMON");
    //     common.transform.parent = transform;
    //     common.AddComponent<Resourcex>().file = commonFiles[0];
    //     common.GetComponent<Resourcex>().StartRes();


    //     common.AddComponent<B3DScript>().file = commonFiles[1];
    //     common.GetComponent<B3DScript>().StartB3D();
    //     //common.GetComponent<B3DScript>().CreateB3D(common);
    //     //common.GetComponent<B3DScript>().B3Dread(common,false);
    //     gameObject.AddComponent<ClockScript>();




    //     trucks = new GameObject("TRUCKS");
    //     trucks.transform.parent = transform;
    //     trucks.AddComponent<Resourcex>().file = truckFiles[0];
    //     trucks.GetComponent<Resourcex>().StartRes();
    //     trucks.AddComponent<B3DScript>().file = truckFiles[1];
    //     trucks.GetComponent<B3DScript>().StartB3D();
    //     trucks.GetComponent<B3DScript>().Disable();


    //     TechManager TruckTech = new TechManager();
    //     TruckTech.TRUCKS = trucks;
    //     TruckTech.StartTech();

    //     //TruckTech = new GameObject("TruckTech");
    //     //TruckTech.transform.parent = transform;
    //     //TruckTech.AddComponent<TechManager>().TRUCKS = trucks;


    //     foreach (var file in envResFiles)
    //     {
    //         GameObject resFile = new GameObject(file.Name.Split('.')[0].ToUpper());
    //         resFile.transform.parent = transform;
    //         resFile.AddComponent<Resourcex>().file = file;
    //         resFile.GetComponent<Resourcex>().StartRes();
    //     }


    //     foreach (var file in envB3DFiles)
    //     {
    //         GameObject b3dFile = GameObject.Find(file.Name.Split('.')[0].ToUpper());
    //         MainRooms.Add(b3dFile);
    //         if (b3dFile != null)
    //         {
    //             b3dFile.AddComponent<B3DScript>().file = file;
    //             b3dFile.GetComponent<B3DScript>().StartB3D();
    //             LoaT.AddRange(b3dFile.GetComponent<B3DScript>().LoadTriggers);
    //         }
    //     }
    // }


    public static void SetActiveBlocks(bool enable)
    {
        instance.SetEnableObjects(enable);
    }
    private void RemoveIDisableNulls()
    {
        List<IDisableable> disableables = new List<IDisableable>();
        foreach (var item in disableList)
        {
            if (item != null)
            {
                disableables.Add(item);
            }
        }
        disableList = disableables;
    }
    private void SetEnableObjects(bool enable)
    {
        RemoveIDisableNulls();
        IDisableable[] disableables = new IDisableable[disableList.Count];
        disableList.CopyTo(disableables);
        foreach (var go in disableables)
        {
            if (go != null)
                if (go.GetGameObject.activeSelf)
                {
                    if (enable)
                        go.Enable();
                    else
                        go.Disable();
                }
        }

    }

}