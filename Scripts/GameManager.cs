using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour {

	public Shader TC2;
	public static Shader TC;
	public Shader TCu2;
	public static Shader TCu;
	public List<GameObject> MainRooms = new List<GameObject>();
	public List<GameObject> Rooms = new List<GameObject>();
	public List<GameObject> LoaT = new List<GameObject>();
	static DirectoryInfo ENVpath = new DirectoryInfo(@"db2\ENV\");
	static DirectoryInfo CommonPath = new DirectoryInfo(@"db2\COMMON\");
	static DirectoryInfo rootPath = new DirectoryInfo(@"db2\");

	public Time time;
	FileInfo[] commonFiles = {CommonPath.GetFiles("common.res")[0],CommonPath.GetFiles("common.b3d")[0]};
	FileInfo[] cabines = CommonPath.GetFiles("cabines*");
	FileInfo[] truckFiles = {CommonPath.GetFiles("trucks.res")[0],CommonPath.GetFiles("trucks.b3d")[0]};

	FileInfo[] envResFiles = {ENVpath.GetFiles("ap.res")[0]};
	FileInfo[] envB3DFiles = {ENVpath.GetFiles("ap.b3d")[0]};
	//FileInfo[] envResFiles = {ENVpath.GetFiles("*.res")[0]};
	//FileInfo[] envB3DFiles = {ENVpath.GetFiles("*.b3d")[0]};
	//FileInfo[] envResFiles = ENVpath.GetFiles("a?.res");
	//FileInfo[] envB3DFiles = ENVpath.GetFiles("a?.b3d");

	
	public GameObject camera;
	public GameObject CarNode;
	string StartCar = "Zil";
	Vector3 StartPosition = new Vector3(-3060,30,-4645);
	GameObject trucks;
	bool DoOnce = false;
	public static GameObject common;

	void OnApplicationQuit()
	{
		GC.Collect();
	}
	void Start()
	{
		int childCount = transform.childCount;
		for(int i = 0; i<childCount;i++)
		{
			List<GameObject> goL = transform.GetChild(i).GetComponent<B3DScript>().InvokeBlocks;
			foreach(var go in goL)
			{
				go.GetComponent<InvokeMe>().Invoke();
			}
			transform.GetChild(i).GetComponent<B3DScript>().InvokeBlocks = null;
		}





		int TrucksCount = trucks.GetComponent<TrucksBehaviour>().Trucks.Count;
		for (int i = 0; i<TrucksCount;i++)
		{
			GameObject gob = trucks.GetComponent<TrucksBehaviour>().Trucks[i];
			if (gob.name == StartCar)
			{
				GameObject car = Instantiate(gob.gameObject);
				camera.transform.SetParent(car.transform);
				camera.transform.localPosition = new Vector3(0,5,-10);
				Quaternion qu = new Quaternion();
				qu.eulerAngles = new Vector3(10,0,0);
				camera.transform.localRotation = qu;
				camera.transform.localScale = new Vector3(0,0,0);
				car.SetActive(true);
				car.transform.SetParent(transform);
				car.transform.localPosition = StartPosition;

				//AddTruckBehaviour(car);
				AddJcar(car);
			

			}
		}


	}
	void AddJcar(GameObject car)
	{
		vehicle veh = car.GetComponent<vehicle>();
		
		car.AddComponent<Rigidbody>().mass = veh.mass;
		car.AddComponent<Collision23>();
		JCar jCar = car.AddComponent<JCar>();
		int i = 0;
		foreach (var go in veh.WheelMe)
		{
			GameObject newWhOb = new GameObject("WheelCollider"+i);
			newWhOb.transform.SetParent(car.transform);
			newWhOb.transform.position = go.transform.GetChild(0).position;
			WheelCollider wc = newWhOb.AddComponent<WheelCollider>();

			//WheelCollider wc = go.transform.GetChild(0).gameObject.AddComponent<WheelCollider>();
			wc.mass = 20;
			wc.radius = veh.rwheel;
			wc.suspensionDistance = 0;//0.2f; смешно переворачивает автомобиль :)

            
            JointSpring js = wc.suspensionSpring;
            js.spring = veh.KS[i] * 4;
            js.damper = 4500;            
            wc.suspensionSpring = js;
 
            // see docs, haven't really managed to get this work
            // like i would but just try out a fiddle with it.
            WheelFrictionCurve fc = wc.forwardFriction;
            fc.asymptoteValue = 5000.0f;
            fc.extremumSlip = 1.0f;
            fc.asymptoteSlip = 2.0f;
            fc.stiffness = 0.9f;
            wc.forwardFriction = fc;
            fc = wc.sidewaysFriction;
            fc.asymptoteValue = 7500.0f;
            fc.asymptoteSlip = 2.0f;
            fc.stiffness = 0.9f;
            wc.sidewaysFriction = fc;


			jCar.Wheels.Add(go.transform.GetChild(0));
			jCar.WheelsCol.Add(newWhOb.transform);

			i++;
		}
		
		
		
		jCar.checkForActive = car;
		jCar.automatic = true;

		//jCar.wheelRadius = veh.rwheel;

		jCar.torque = veh.maxrotmomentum;
		jCar.brakeTorque = veh.breakpower;
		jCar.gears = veh.gear_trans_coeff;
		jCar.gearsCount = veh.gearCount1;
		jCar.maxRPM = veh.tach_with_max_power * 3.0f;
		jCar.shiftUpRPM = veh.tach_with_max_power * 1.25f;
		jCar.shiftDownRPM = veh.tach_with_max_power;

		if (veh.driving_wheels[3] == 0) 
		{
			jCar.wheelDrive = JWheelDrive.Front;
		}
		if (veh.driving_wheels[1] == 0) 
		{
			jCar.wheelDrive = JWheelDrive.Back;
		}
		else
		{
			jCar.wheelDrive = JWheelDrive.All;
		}

		
		/*Mesh me = new Mesh();
		me.Clear();

		me.vertices = veh.CornerMark;
		List <int> faces = new List<int>();
		for (int j = 0; j<veh.CornerMarkCount-3; j++)
		{
			if (j%2==0)
			{
				faces.Add(j);
				faces.Add(j+2);
				faces.Add(j+1);
			}
			else
			{
				faces.Add(j);
				faces.Add(j+1);
				faces.Add(j+2);
			}

		} 
		
		me.triangles = faces.ToArray();
		
		me.RecalculateBounds();
		GameObject newObject1 = new GameObject("me");
		newObject1.transform.SetParent(car.transform);
		newObject1.transform.localPosition = new Vector3(0,0,0);
		newObject1.AddComponent<MeshRenderer>();
		newObject1.AddComponent<MeshFilter>().mesh = me;
		newObject1.AddComponent<MeshCollider>().sharedMesh = me;*/

		jCar.veh = veh;
		foreach(var a in veh.CornerMark)
		{
			GameObject newObject = new GameObject("ColPlane");
			SphereCollider sc = newObject.AddComponent<SphereCollider>();
			newObject.transform.SetParent(car.transform);
			sc.transform.localPosition = new Vector3(0f,0f,0f);
			sc.center = a;
			sc.radius = 0.1f;
			sc.isTrigger = true;
		}
		veh.hitObj.transform.GetChild(0).GetComponent<BlockSwitcher>().switchState(1);
		veh.hitObj.transform.GetChild(0).GetComponent<BlockSwitcher>().rendered = true;
		Debug.LogWarning("switched?",veh.hitObj.transform.GetChild(0));

		jCar.StartJcar();
		

		
	}

	void AddTruckBehaviour (GameObject car)
	{
		Dot_Truck_Controller CarController = car.AddComponent<Dot_Truck_Controller>();
		vehicle veh = car.GetComponent<vehicle>();

		car.AddComponent<Rigidbody>().mass = veh.mass;

		//CarController.maxMotorTorque = 1500;
		//CarController.maxMotorTorque = veh.horse_power + veh.maxrotmomentum;

		CarController.maxSteeringAngle = 45;
		CarController.maxRPM = veh.tach_with_max_power * 2.5f; // veh.tach_with_max_power * 2.5f от фонаря написал
		CarController.breakpower = veh.breakpower;
		CarController.maxrotmomentum = veh.maxrotmomentum;
		CarController.horse_power = veh.horse_power;
		CarController.gearsCount = veh.gearCount1;
		CarController.rear_axle_coeff = veh.rear_axle_coeff;
		CarController.reverse_trans_coeff = veh.reverse_trans_coeff;
		CarController.gear_trans_coeff1 = veh.gear_trans_coeff[1];
		CarController.gear_trans_coeff2 = veh.gear_trans_coeff[2];
		CarController.gear_trans_coeff3 = veh.gear_trans_coeff[3];
		CarController.gear_trans_coeff4 = veh.gear_trans_coeff[4];
		CarController.gear_trans_coeff5 = veh.gear_trans_coeff[5];
		CarController.gear_trans_coeff6 = veh.gear_trans_coeff[6];
		CarController.gear_trans_coeff7 = veh.gear_trans_coeff[7];
		CarController.gear_trans_coeff8 = veh.gear_trans_coeff[8];
		CarController.gear_trans_coeff9 = veh.gear_trans_coeff[9];
		CarController.gear_trans_coeff10 = veh.gear_trans_coeff[10];
		CarController.gear_trans_coeff11 = veh.gear_trans_coeff[11];
		CarController.gear_trans_coeff12 = veh.gear_trans_coeff[12];
		CarController.gear_trans_coeff13 = veh.gear_trans_coeff[13];

		//CKS ColKeyState = gob.Find("hit_"+veh.CarNode);
		//GameObject ColKeyState = gob.Find(CollisionKey).gameObject;
		//ColKeyState.aaa = "1";
		int wheelCount = veh.WheelMe.Count/2;
		CarController.truck_Infos = new List<Dot_Truck>();


		for (int j = 0; j<wheelCount;j++)
		{
			Dot_Truck dt = new Dot_Truck();
			dt.leftWheelMesh = veh.WheelMe[j*2].transform.GetChild(0).gameObject;
			
			dt.rightWheelMesh = veh.WheelMe[j*2+1].transform.GetChild(0).gameObject;
			CarController.truck_Infos.Add(dt);
			if (j == 0)
			{
				dt.steering = true;
			}
			else
			{
				dt.motor = true;
			}
		}
	}
	void Awake () {

		TC = TC2;
		TCu = TCu2;

		common = new GameObject("COMMON");
		common.transform.parent = transform;
		common.AddComponent<Resourcex>().file = commonFiles[0];
		common.GetComponent<Resourcex>().StartRes();


		common.AddComponent<B3DScript>().file = commonFiles[1];
		common.GetComponent<B3DScript>().StartB3D();
		//common.GetComponent<B3DScript>().CreateB3D(common);
		//common.GetComponent<B3DScript>().B3Dread(common,false);
		gameObject.AddComponent<ClockScript>();




		trucks = new GameObject("TRUCKS");
		trucks.transform.parent = transform;
		trucks.AddComponent<Resourcex>().file = truckFiles[0];
		trucks.GetComponent<Resourcex>().StartRes();
		trucks.AddComponent<B3DScript>().file = truckFiles[1];
		trucks.GetComponent<B3DScript>().StartB3D();
		trucks.AddComponent<TrucksBehaviour>();
		trucks.GetComponent<B3DScript>().Disable();


		TechManager TruckTech = new TechManager();
		TruckTech.TRUCKS = trucks;
		TruckTech.StartTech();

		//TruckTech = new GameObject("TruckTech");
		//TruckTech.transform.parent = transform;
		//TruckTech.AddComponent<TechManager>().TRUCKS = trucks;
		

		foreach(var file in envResFiles)
		{
			GameObject resFile = new GameObject(file.Name.Split('.')[0].ToUpper());
			resFile.transform.parent = transform;
			resFile.AddComponent<Resourcex>().file = file;
			resFile.GetComponent<Resourcex>().StartRes();
		}


		foreach (var file in envB3DFiles)
		{
			GameObject b3dFile = GameObject.Find(file.Name.Split('.')[0].ToUpper());
			MainRooms.Add(b3dFile);
			if (b3dFile != null)
			{
				b3dFile.AddComponent<B3DScript>().file = file;
				b3dFile.GetComponent<B3DScript>().StartB3D();
				LoaT.AddRange(b3dFile.GetComponent<B3DScript>().LoadTriggers);
			}
		}
	}


	
	void Update () {

		/*if (!DoOnce)
		{
			
			DoOnce = true;
			int TrucksCount = trucks.GetComponent<TrucksBehaviour>().Trucks.Count;
			for (int i = 0; i<TrucksCount;i++)
			{
				GameObject gob = trucks.GetComponent<TrucksBehaviour>().Trucks[i];
				if (gob.name == StartCar)
				{

					GameObject car = Instantiate(gob.gameObject);
					camera.transform.SetParent(car.transform);
					camera.transform.localPosition = new Vector3(0,5,-10);
					Quaternion qu = new Quaternion();
					qu.eulerAngles = new Vector3(10,0,0);
					camera.transform.localRotation = qu;
					camera.transform.localScale = new Vector3(0,0,0);
					vehicle veh = car.GetComponent<vehicle>();

					car.AddComponent<Rigidbody>().mass = veh.mass;
					car.SetActive(true);
					car.transform.SetParent(transform);
					car.transform.localPosition = StartPosition;
					Dot_Truck_Controller CarController = car.AddComponent<Dot_Truck_Controller>();
					//CarController.maxMotorTorque = 1500;
					//CarController.maxMotorTorque = veh.horse_power + veh.maxrotmomentum;
					
					CarController.maxSteeringAngle = 45;
					CarController.maxRPM = (veh.tach_with_max_power) * 2.5f; // veh.tach_with_max_power * 2.5f от фонаря написал
					CarController.breakpower = veh.breakpower;
					CarController.maxrotmomentum = veh.maxrotmomentum;
					CarController.horse_power = veh.horse_power;
					CarController.gearsCount = veh.gearCount1;
					CarController.rear_axle_coeff = veh.rear_axle_coeff;
					CarController.reverse_trans_coeff = veh.reverse_trans_coeff;
					CarController.gear_trans_coeff1 = veh.gear_trans_coeff[1];
					CarController.gear_trans_coeff2 = veh.gear_trans_coeff[2];
					CarController.gear_trans_coeff3 = veh.gear_trans_coeff[3];
					CarController.gear_trans_coeff4 = veh.gear_trans_coeff[4];
					CarController.gear_trans_coeff5 = veh.gear_trans_coeff[5];
					CarController.gear_trans_coeff6 = veh.gear_trans_coeff[6];
					CarController.gear_trans_coeff7 = veh.gear_trans_coeff[7];
					CarController.gear_trans_coeff8 = veh.gear_trans_coeff[8];
					CarController.gear_trans_coeff9 = veh.gear_trans_coeff[9];
					CarController.gear_trans_coeff10 = veh.gear_trans_coeff[10];
					CarController.gear_trans_coeff11 = veh.gear_trans_coeff[11];
					CarController.gear_trans_coeff12 = veh.gear_trans_coeff[12];
					CarController.gear_trans_coeff13 = veh.gear_trans_coeff[13];
					
					//CKS ColKeyState = gob.Find("hit_"+veh.CarNode);
					//GameObject ColKeyState = gob.Find(CollisionKey).gameObject;
					//ColKeyState.aaa = "1";

					int wheelCount = veh.WheelMe.Count/2;
					CarController.truck_Infos = new List<Dot_Truck>();
	
					
					for (int j = 0; j<wheelCount;j++)
					{
						Dot_Truck dt = new Dot_Truck();
						Debug.Log(veh.WheelMe[j*2].transform.childCount);

						dt.leftWheel = veh.WheelCol[j*2].GetComponent<WheelCollider>();
						dt.leftWheelMesh = veh.WheelMe[j*2].transform.GetChild(0).gameObject;
						
						dt.rightWheel = veh.WheelCol[j*2+1].GetComponent<WheelCollider>();
						dt.rightWheelMesh = veh.WheelMe[j*2+1].transform.GetChild(0).gameObject;
						CarController.truck_Infos.Add(dt);
						if (j == 0)
						{
							dt.steering = true;
						}
						else
						{
							dt.motor = true;
						}
					}


				}
			}

		}*/
	}
}

class CarNode : MonoBehaviour
{
	public GameObject CarObject;
}


class TrucksBehaviour : MonoBehaviour{
	public List<GameObject> Trucks = new List<GameObject>();
}