using Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class LoadTrigger : MonoBehaviour {
	public string roomName;
	public Vector3 point0;
	public Vector3 point1;
	public GameObject roomObject;
	Renderer obR;
	bool active = true;

	void Start()
	{
		obR = GetComponent<Renderer>();
		//active = obR.isVisible;
	}
	void Update()
	{
		/*if ((obR.isVisible))//&&(active))
		{
			if (roomObject)
			{
				roomObject.SetActive(true);
				Debug.Log("active",gameObject);
			}
			active=!active;
		}
		if ((obR.isVisible))//&&(active))
		{
			if (roomObject)
			{
				roomObject.SetActive(false);
				Debug.Log("not active",gameObject);
			}
			active=!active;
		}*/
	}
	void OnWillRenderObject()
	{
		if (roomObject)
			roomObject.SetActive(true);
	}
	/*void OnBecameInvisible()
	{
		if (roomObject)
			roomObject.SetActive(false);
	}*/

	void UnTrigger()
	{

	}
	public void Trigger()
	{
		/* gameObject.AddComponent<BoxCollider>();
		BoxCollider BC = gameObject.GetComponent<BoxCollider>();

		BC.center =((point1-point0)/2) +point0; 
		BC.size = point1 - point0;
		BC.isTrigger = true;*/
		//GameObject newObject = new GameObject("BoxCollider 30 blk");
		GameObject newObject = gameObject;
		// newObject.name = "BoxCollider 30 blk";
		newObject.transform.SetParent(transform);



		Vector3 newVec = point0-point1;
		float rot;
		float div = Mathf.Sqrt(newVec.z*newVec.z+newVec.z*newVec.z);
		if (div !=0)
		{
			rot = Mathf.Acos(newVec.x/div) * Mathf.Rad2Deg;
		}
		else
		{
			rot = 0;
		}
		//Debug.Log(rot);

		BoxCollider BC = newObject.AddComponent<BoxCollider>();
		newObject.transform.position = ((point1-point0)/2) +point0;
		Quaternion qack = new Quaternion();

		if (point0.z>point1.z)
		{
			rot = -1*rot;
		}
		qack.eulerAngles = new Vector3(0,rot,0);
		newObject.transform.localRotation = qack;

		Vector3 poin00,poin01;
		poin00 = point0;
		poin01 = point1;
		poin00.y = 0;
		poin01.y = 0;

		float x = Vector3.Distance(poin00,poin01);
		poin00 = point0;
		poin01 = point1;
		poin00.z = 0;
		poin01.z = 0;


		float y = Vector3.Distance(poin00,poin01);
		BC.size = new Vector3(x,y,0);
		BC.isTrigger = true;
		
		


/*
		Mesh me = new Mesh();
		me.Clear();

		Vector3[] vec = new Vector3[4];

		vec[0] = point0;
		vec[1] = point1;
		vec[2] = point0;
		vec[2].y = point1.y;
		vec[3] = point1;
		vec[3].y = point0.y;

		int[] faces = new int[6]{0,2,1,0,1,3};

		me.vertices = vec;
		me.triangles = faces;
		me.RecalculateBounds();
		Debug.Log("",gameObject);
	
		//gameObject.AddComponent<MeshFilter>().sharedMesh = me;
		gameObject.AddComponent<MeshCollider>().sharedMesh = me;
		gameObject.GetComponent<MeshCollider>().convex = true;//material = (Material)Resources.Load("Default-Material", typeof(Material));
		gameObject.GetComponent<MeshCollider>().isTrigger = true;
*/



	}


	public void Init(GameObject resOb) //TODO
	{
		List<GameObject> Rooms = resOb.GetComponent<B3DScript>().Rooms;
		foreach (var room in Rooms)
		{
			if (roomName.Length>11)
			{
				string newname = roomName.Split(':')[0];
				foreach (var obj in GameObject.Find("Root").GetComponent<GameManager>().MainRooms)
				{
					if (obj.name == newname.ToUpper())
					{
						obj.SetActive(true);
						string roomName2 = roomName.Split(':')[1];
						foreach(var room1 in obj.GetComponent<B3DScript>().Rooms)
						{
							if (room.name == roomName2)
							{
								roomObject = room;
							}
						}
					}
				}
			}
			if (room.name == roomName)
			{
				roomObject = room;
			}
		}

	}

}
