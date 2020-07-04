using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODCustom : MonoBehaviour {

	public GameObject[][] gameObjects = new GameObject[2][];
	public GameObject[] go0,go1;
	public Vector3 Center;
	public float Distance;
	// Use this for initialization
	public void SetLods(GameObject[][] gameObjects0,Vector3 Center0,float Distance0)
	{
		gameObjects = gameObjects0;
		Center = Center0;
		Distance = Distance0;
	}
	void Start () {
	
	}
	public void MakeLod()
	{
		SphereCollider SC = gameObject.AddComponent<SphereCollider>();
		SC.center = Center;
		SC.radius = Distance;
		SC.isTrigger = true;	
		go0 = gameObjects[0];
		go1 = gameObjects[1];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SwitchState(int index)
	{
		foreach(var go in gameObjects[index])
		{
			go.SetActive(true);
		}
		if (index==1)
			index = 0;
		else
			index = 1;
		foreach(var go in gameObjects[index])
		{
			go.SetActive(false);
		}	


	}

}
