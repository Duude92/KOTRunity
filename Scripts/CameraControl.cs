using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class CameraControl : MonoBehaviour {

	RaycastHit[] hits;
	Ray ray;
	public string CurrentRoom = "room_ap_049";
	string lastRoom = "";
	public string mainRoom = "";
	public string lastMainRoom = "";
	public string nextMainRoom = "";
	float time = 0.25f;
	public GameObject rootObj;
	public List<GameObject> EnableList;
	public List<GameObject> DisableList;
	float angle = 0;
	public Plane[] planes;

	void Start () {
		DisableList = new List<GameObject>();
		planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());		

	}
	
	void Update () {

		if (Input.GetKey(KeyCode.Delete))
		{
			angle-=0.01f;
			Vector3 pos = transform.localPosition;
			pos.x=10*Mathf.Cos(angle);
			pos.z=10*Mathf.Sin(angle);
			transform.localPosition = pos;
			transform.LookAt(transform.parent,new Vector3(0,1,0));

		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			angle+=0.01f;
			Vector3 pos = transform.localPosition;
			pos.x=10*Mathf.Cos(angle);
			pos.z=10*Mathf.Sin(angle);
			transform.localPosition = pos;
			transform.LookAt(transform.parent,new Vector3(0,1,0));
		}
		foreach (var room in rootObj.GetComponent<GameManager>().Rooms)
		{
			if ((room.name == CurrentRoom)&&(!room.activeSelf))
			{
				room.SetActive(true);
			}
		}
		
		ray = new Ray(transform.position,transform.forward);
		EnableList = new List<GameObject>();
		DisableList = new List<GameObject>();
		

		hits = Physics.RaycastAll(ray,1000f);
		
		Vector3 forward = transform.TransformDirection(Vector3.forward) * 1000;
		Debug.DrawRay(transform.position-new Vector3(0,1,0),forward,Color.black);




		///-----------------------------------------------
		/*if(time >= 0.25f)
		foreach(var lo in rootObj.GetComponent<GameManager>().LoaT)
		{
			time = 0f;
			planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
			if (GeometryUtility.TestPlanesAABB(planes,lo.GetComponent<BoxCollider>().bounds))
			{
				LoadTrigger lt = lo.GetComponent<LoadTrigger>();
				if (lt.roomObject)
					lt.roomObject.SetActive(true);
			}
			else
			{
				LoadTrigger lt = lo.GetComponent<LoadTrigger>();
				if ((lt.roomName!=CurrentRoom)&&(lt.roomObject))
					lt.roomObject.SetActive(false);
			}
		}




		///-----------------------------------------------

	/*	if(time >= 0.25f)
		{
			if (mainRoom.Length == 0)
			{
				changeMainRoom("AQ:");
			}
			List<GameObject> Rooms = rootObj.GetComponent<GameManager>().Rooms;
			
			foreach(var hit in hits)
			{

				if (hit.collider.gameObject.GetComponent<LoadTrigger>() !=null)
				{
					string roomName = hit.collider.gameObject.GetComponent<LoadTrigger>().roomName;
					if (roomName.Length>11)
					{
						roomName = changeMainRoom(roomName,false);
					}
					foreach (var room in Rooms)
					{
						if ((room.name == roomName)||(room.name == CurrentRoom)||(room.name == lastRoom))
						{
							EnableList.Add(room);
						}
					}
					//hit.collider.gameObject.GetComponent<LoadTrigger>().roomObject.SetActive(true); //аар1 До лучших времен потерпит
				}
			}
			if (EnableList.Count>2)
				DisableList = Rooms.Except(EnableList).ToList();
			time = 0f;

			//Debug.Break();
			foreach(var room in EnableList)
			{
				room.SetActive(true);

				List<GameObject> newb = room.GetComponent<RoomBlock>().hits;
				foreach(GameObject rr in newb)
				{
					rr.SetActive(true);
				}


			}
			foreach(var room in DisableList)
			{
				room.SetActive(false);
				List<GameObject> newb = room.GetComponent<RoomBlock>().hits;
				foreach(var rr in newb)
				{
					if (rr.activeSelf)
						rr.SetActive(false);
				}
			}
		}
		*/
		time +=Time.deltaTime;

	}
	public void ChangeCurrentRoom(string room)
	{					
		if (room.Length>11)
		{
			room = changeMainRoom(room);
		}

		if (lastRoom != room)
			if (CurrentRoom != room)
			{
				lastRoom = CurrentRoom;
				CurrentRoom = room;
					
			}
	}

	string changeMainRoom(string nextRoomName,bool change = true)
	{
					
		string newname = nextRoomName.Split(':')[0];
		if (change)
		{
			if (newname!=lastMainRoom)
			{
				lastMainRoom = mainRoom.ToUpper();
				mainRoom = newname.ToUpper();
				GameObject.Find("Root").GetComponent<GameManager>().Rooms = new List<GameObject>();
				foreach (var obj in GameObject.Find("Root").GetComponent<GameManager>().MainRooms)
				{
					if ((obj.name == lastMainRoom)||(obj.name == mainRoom)||(obj.name == nextMainRoom))
					{
						GameObject.Find("Root").GetComponent<GameManager>().Rooms.AddRange(obj.GetComponent<B3DScript>().Rooms);
					}
				}
			}
		}
		else
		{
			nextMainRoom = newname.ToUpper();
		}

		nextRoomName = nextRoomName.Split(':')[1];
		foreach (var obj in GameObject.Find("Root").GetComponent<GameManager>().MainRooms)
		{
			if (!change)
			{
				if ((obj.name == newname)||(obj.name == nextMainRoom))
					obj.SetActive(true);
			}
			else
				if ((obj.name != mainRoom)&&(obj.name != lastMainRoom)&&(obj.name != nextMainRoom))
				{
					obj.SetActive(false);
				}
				else
				{
					obj.SetActive(true);
				}
		}
		return nextRoomName;
	}
}
