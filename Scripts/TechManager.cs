using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
//using System.Timers;


public class TechManager : MonoBehaviour {

	public string TechName;
	public int creationTime;
	public int blockSize;
	public int AddParamCount;
	public int unknVeh;
	public int CarCount;
	public int LorrieCout;
	public int TractorCount;
	public int SemiTrCount;
	public int HeliCount;
	public int VehCount;
	public GameObject TRUCKS;


	public void StartTech()
	{
	DirectoryInfo techPath = new DirectoryInfo(@"db2\");
			FileInfo tech = techPath.GetFiles("vehicle.tech")[0];
			BinaryReader br = new BinaryReader(File.Open(tech.FullName,FileMode.Open));
			byte[] resource = new byte[tech.Length];
			br.Read(resource,0,(int)tech.Length);
			br.Close();
			br = null;
			tech = null;
			techPath = null;
			int pos = 0;

			pos +=4;//13 ver??
			TechName = byteRead.GetLine(resource,ref pos);
			creationTime = System.BitConverter.ToInt32(resource,pos);
			pos+=4;
			System.DateTime dt = System.DateTime.FromBinary(creationTime);
			blockSize = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			AddParamCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			unknVeh = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			CarCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			LorrieCout = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			TractorCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			SemiTrCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			HeliCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			VehCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;

			TrucksBehaviour tb = TRUCKS.GetComponent<TrucksBehaviour>();
			for (int i = 0; i<VehCount;i++)
			{
				//GameObject newOb = new GameObject();
				//var veh = newOb.AddComponent<vehicle>();
				var veh = new vehicle();
				veh.Read(resource,ref pos);
				Transform truckOb = TRUCKS.transform.Find(veh.CarNode);
				if (truckOb)
				{
					Mesh me = new Mesh();
					me.Clear();
					List<Vector3> CM = new List<Vector3>();
					List<int> faces = new List<int>();
					//int number = 0;
					for (int j = 0; j< veh.CornerMarkCount;j++)
					{
						CM.Add(veh.CornerMark[j]);
						/*if (j == 0)
						{
							faces.AddRange(new int[] {0,2,1});
							number+=2;
						}
						else 
						{
							if (number+1 == veh.CornerMarkCount)
								faces.AddRange(new int[3]{(number),(number+1),(number-1)});
							else
								faces.AddRange(new int[6]{(number),(number+1),(number-1),(number+1),(number),(number+2)});
							number+=2;
						}*/

					}
					me.vertices = CM.ToArray();
					me.triangles = faces.ToArray();
					me.RecalculateBounds();
					var a = truckOb.gameObject.AddComponent<MeshCollider>();

					a.sharedMesh = me;
					a.convex = true;
					me = null;
					Component copy = truckOb.gameObject.AddComponent<vehicle>();
					
					System.Reflection.FieldInfo[] fields = veh.GetType().GetFields();
					foreach(System.Reflection.FieldInfo field in fields)
					{
						field.SetValue(copy,field.GetValue(veh));
					}
					tb.Trucks.Add(truckOb.gameObject);

					Transform gob = null;
					
					GameObject Wheels = new GameObject("Wheel Colliders");
					Wheels.transform.SetParent(truckOb);
					foreach(Transform go in truckOb.transform)
					//for (int j = 0; j<truckOb.transform.childCount;j++)
					{
						if (go.name == "hit_"+veh.CarNode)
						{
							truckOb.gameObject.GetComponent<vehicle>().hitObj = go.gameObject;
						}
						
					for (int k = 0; k<8;k++)
					{
						//Debug.LogWarning(truckOb.transform.GetChild(j).name+"	:	"+veh.CarNode+"wheel"+j);
						if (go.name == veh.CarNode+"wheel"+k)
						/*gob = truckOb.Find(veh.CarNode+"wheel"+j);

						if (gob)*/
						{
							gob = go;
							string WSpace = gob.GetChild(0).GetComponent<InvokeMe>().space;

							truckOb.gameObject.GetComponent<vehicle>().WheelMe.Add(gob.transform.GetChild(0).gameObject);

							string Space = gob.transform.GetChild(0).GetComponent<InvokeMe>().space;
							int num = (int)System.Char.GetNumericValue(Space.Split(new string[] { "Wheel"},System.StringSplitOptions.None)[1][0]);

							truckOb.gameObject.GetComponent<vehicle>().WheelNum.Add(num);

							GameObject SpcOb = TRUCKS.transform.Find(WSpace).gameObject;

							/*WheelCollider wc = WheelCol.AddComponent<WheelCollider>(); //TODO: Удалить если используем JCar
							wc.mass = 20;
							wc.radius = veh.rwheel;
							//wc.wheelDampingRate = 0.25f;
							//wc.suspensionDistance = veh.SuspensionMax[1] - veh.SuspensionMin[1];
							//wc.suspensionDistance = 0.3f;
							wc.wheelDampingRate = veh.SuspensionMin[1]; //хз, откуда брать параметр
							wc.suspensionDistance = veh.SuspensionMax[1] - veh.SuspensionMin[1];
							JointSpring js = new JointSpring();
							
							js.spring = veh.KS[1]; //привет из Car Races Demo: сила пружины: ks*(hmax-h)
							js.damper = 4500;
							js.targetPosition = veh.SuspensionMin[1];
							wc.suspensionSpring = js;
							WheelFrictionCurve wf = new WheelFrictionCurve();
							wf.extremumSlip = 0.4f;
							wf.extremumValue = 1;
							wf.asymptoteSlip = 0.8f;
							wf.asymptoteValue = 0.5f;
							wf.stiffness = veh.CS[1];
							wc.forwardFriction = wf;
							WheelFrictionCurve sf = new WheelFrictionCurve();
							sf.extremumSlip = 0.2f;
							sf.extremumValue = 1;
							sf.asymptoteSlip = 0.5f;
							sf.asymptoteValue = 0.75f;
							sf.stiffness = 1;
							wc.sidewaysFriction = sf;*/
						}
						gob = null;

					}	
					}



				}
				else
				{
					Debug.LogError(veh.CarNode+" node not found");
				}
				//newOb.transform.SetParent(transform);
			}
	}
}


/*[ExecuteInEditMode]
public class TechManager : EditorWindow {

	string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Window/Tech Manager")]
	  public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TechManager));
    }
	void OnEnable()
	{
	}

	void OnGUI()
	{
		groupEnabled = EditorGUILayout.BeginToggleGroup("OpenTech",groupEnabled);
		//if (GUILayout.Button("Open tech"))
		{
			DirectoryInfo techPath = new DirectoryInfo(@"db2\");
			FileInfo tech = techPath.GetFiles("vehicle.tech")[0];
			BinaryReader br = new BinaryReader(File.Open(tech.FullName,FileMode.Open));
			byte[] resource = new byte[tech.Length];
			br.Read(resource,0,(int)tech.Length);
			br.Close();
			br = null;
			tech = null;
			techPath = null;
			int pos = 0;

			pos +=4;//13 ver??
			GUILayout.Label ("Base Settings",EditorStyles.boldLabel);
			myString = EditorGUILayout.TextField ("Source File", byteRead.GetLine(resource,ref pos));
			//GUILayout.Label (System.DateTime.Parse(System.BitConverter.ToInt32(resource,pos).ToString()).ToString());
			int time = System.BitConverter.ToInt32(resource,pos);
			pos+=4;
			System.DateTime dt = System.DateTime.FromBinary(time);
			GUILayout.Label(dt.ToString());
			int blockSize = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Block Size: "+blockSize);
			int AddParamCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Addditional parameter: "+ AddParamCount);

			int unknVeh = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Unknown Vehicle count? "+ unknVeh);

			int CarCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Cars count: " + CarCount);

			int LorrieCout = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Lorries Count: " + LorrieCout);

			int TractorCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Tractors Count: " + TractorCount);

			int SemiTrCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Semitrailers count: " + SemiTrCount);

			int HeliCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Helicopter Count: " + HeliCount);

			int VehCount = System.BitConverter.ToInt32(resource,pos);
			pos +=4;
			GUILayout.Label ("Vehicle Count: " + VehCount);

			vehicle[] veh = new vehicle[VehCount];
			for (int i = 0; i<VehCount;i++)
			{
				Debug.Log(pos);
				veh[i] = new vehicle();
				veh[i].Read(resource,ref pos);
				Debug.Log(veh[i].veh_name);
			}

			//resource = null;

			EditorGUILayout.EndToggleGroup ();
		}

	}



}
*/
class byteRead
{
public static string GetLine(byte[] br, ref int pos)
	{
		string result;
		List<byte> chArr = new List<byte>();
		byte b = new byte();
		for(;;)
		{
			b = br[pos];
			if (b!=0)
			{
				chArr.Add(b);
			}
			else
			{
				break;
			}
			pos++;
		}
		result = System.Text.Encoding.UTF8.GetString(chArr.ToArray());
		pos++;
		return result;
	}

public static string GetLine(byte[] br, int pos)
	{
		string result;
		List<byte> chArr = new List<byte>();
		byte b = new byte();
		for(;;)
		{
			b = br[pos];
			if (b!=0)
			{
				chArr.Add(b);
			}
			else
			{
				break;
			}
			pos++;
		}
		result = System.Text.Encoding.UTF8.GetString(chArr.ToArray());
		pos++;
		return result;
	}
}

public class vehicle : MonoBehaviour
{
	public GameObject hitObj;
	public List<GameObject> WheelMe;
	public List<int> WheelNum;

	public string CarNode;
	public Vector3[] PlaceNode_matrix_mul;
	public Vector3 CenterMassInB3D; // центр масс объекта
	public Vector3[] matrix0;
	public Vector3 xyz0;
	public float mass; // масса объекта
	public float rwheel; // радиус колеса (для коллизии)
	public float lwheel; // ширина колеса (для коллизии)
	public float tank;  // объём топливного бака
	public float fuelTimeEndurance; // хз, для расчёта времени использования топлива используется
	public Vector3 xyz1;
	public float x0;
	public float x1;
	public float tach_with_max_power; // вообще из ДБ1 осталось, вроде бы не используется
	public float x2;
	public int gearCount0; // кол-во передач+1
	public int gearCount1; // кол-во передач
	public int gearCount2; // ещё одно кол-во передач
	public float x3;
	public float fuelConsumption; // расход топплива в литрах на 10 километров
	public float x4;
	public float kilometrage; // предположительно, километраж нового (?) автомобиля, остался из ДБ1 (в первой части был одометр)
	public float air_press_coeff; // давление воздуха
    public float air_resist_coeff; // лобовое сопротивление воздуха
    public float air_side_resist_coeff; // боковое сопротивление воздуха
    public float front_wheel_thickness; // ширина отрисовки следов передних шин на асфальте
    public float rear_wheel_thickness; // ширина отрисовки следов задних шин на асфальте
    public float maxrotmomentum; // максимальный крутящий момент
    public float var50;
    public float breakpower; // сила торможения
    public float rear_axle_coeff; // коэффициент передачи движения с коробки передач на колёса, короче: передаточное число редуктора
	public float reverse_trans_coeff;
	public float[] gear_trans_coeff;
    public float tach_with_max_power2; // ?
    public float cargo_and_body_pick_load; // ?
    public float body_mass; // ?
    public float body_height; // ?
    public float body_square; // ?
	public Vector3 center_of_cargo_bottom; // центр массы груза, влияет на общий центр масс автомобиля
	public int[] driving_wheels;
	public int[] steering_wheels;
	public int[] touching_road_wheels;
	public Vector3 OV; // координаты салона
	public float tangViewer; // угол наклона салона автомобиля
	public Vector3 I; // инерция
	public Vector3[] RS; // координаты верхних концов пружин (0 - FL, 1 - FR, 2 - BL, 3 - BR)

	public float[] SuspensionMin;
	public float[] SuspensionMax;
	public float[] KS; // "коэффициент жесткости пружин, сила пружины: ks*(hmax-h)"
	public float[] CS; // "коэфф.аммортизаторов", привет из CARV.ini от Car Races Demo
	public float CollisionRadius; // ?
	public Vector3 CollisionCenter; // ?
	public int var216;
	public float rubber_collision_coeff; // возможно, коэффициент трения шин
    public float slide_collision_coeff; // коэффициент бокового трения
    public int CornerMarkCount;
    public int AddColPntsCount;
	public Vector3[] CornerMark; // крайние точки модели, коллизия для столкновений с статическими объектами (игровой мир)
	public Vector3[] AddColPnts; // непонятная хрень вот тута, хз
	public float[] x7;
	public int CornerMarkWCount;
	public Vector3[] CornerMarkW; // координаты точек колёс, может быть, что они тоже для коллизии, хз
	public Vector4[] CollisionPlane; // коллизия для столкновений с динамическими объектами (машины)
    public int var443;
    public float var444;
    public float var445;
	public Vector3[] tvCameraPosition; // камеры относительно модели автомобиля, 1 - зеркало левое, 2 - внешней камеры (только высота, x и y взяты за вращение), 3 - правое зеркало.
	public int CameraCount;
	public Vector3 tvCameraRightCorner; // хз, камера от бампера вроде бы
	public Vector3 SmokePos0;
	public Vector3 SmokeDir0;
	public Vector3 SmokePos1;
	public Vector3 SmokeDir1;
	public Vector3 FlamePos;
	public Vector3 FaraFl; // координаты левой фары (и света)
	public Vector3 FaraFr; // координаты правой фары (и света)
	public Vector3 FaraBls; // координаты света заднего левого фонаря
	public Vector3 FaraBrs; // координаты света заднего правого фонаря
	public Vector3 FaraBl; // координаты заднего левого фонаря (и света индикатора заднего хода)
	public Vector3 FaraBr; // координаты заднего правого фонаря (и света индникатора заднего хода)

	public int horse_power; // мощность
    public int price; // цена, использовалась только в ДБ1, в ДБ2 берётся из evehicles

    public int glowwing; // ?
	public Vector3 LowSaddle; // ?
	public Vector3 TopSaddle; // ?
	public Vector3 TowHook; // ?
	public Vector3 Coupler; // ?
	public float shiftSaddle; // ?
    public float CouplerLength; // максимальное расстояние, на котором можно присоединить полуприцеп к тягачу

//FPANEL
// Панель: первое значение пока хз, второе значение - промежуток, в котором может двигаться стрелка
// SpeedScale=180.000000 240.000015
// TachScale=55.000000 240.000015
// FuelScale=-1.125000 90.000000 
// В FS 2 float=90.000000, т.е. максимально допустимый наклон стрелки - 90 градусов
	public float speedometr_max_speed;
    public float SpeedScale;
    public float tachometr_max_freq;
    public float TachoScale;
    public float FuelScale;
//FPANEL
	public Vector3 CabRWindow;
	public Vector3 CabLWindow;
	public Vector4 CabFWindow;
	public float[] Drivers_neck_Angles;
	public float var538;
    public float var539;
	public string CockpitSpace;
	public string tchFile;
	public string vch;
	public string veh_name;
	public string prefix;
	public string prefixCab;
	public int x8;
	public int trailerType;
	public Vector3 FaraPos;
	public Vector3 FaraDir; // ?
	public Vector3 FaraWidth; // ширина фар для расчёта столкновений

	
	public void Read(byte[] resource,ref int pos)
	{
		WheelMe = new List<GameObject>();
		WheelNum = new List<int>();
		PlaceNode_matrix_mul = new Vector3[3];
		for (int i = 0; i<3;i++)
		{
			PlaceNode_matrix_mul[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}
		CenterMassInB3D = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		matrix0 = new Vector3[3];
		for (int i = 0; i<3;i++)
		{
			matrix0[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}

		xyz0 = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		mass = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 rwheel = System.BitConverter.ToSingle(resource,pos);
		pos+=4;

		 lwheel= System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 tank= System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 fuelTimeEndurance= System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		xyz1 = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 x0= System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 x1= System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 tach_with_max_power= (System.BitConverter.ToSingle(resource,pos)) / 0.017453292f;
		 pos+=4;
		 x2= System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 
		 gearCount0 = System.BitConverter.ToInt32(resource,pos);
		 gearCount1= System.BitConverter.ToInt32(resource,pos+4);
		 gearCount2= System.BitConverter.ToInt32(resource,pos+8);
		 pos+=12;

		 x3 = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 fuelConsumption = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 x4 = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 kilometrage = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 air_press_coeff = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 air_resist_coeff = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 air_side_resist_coeff = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 front_wheel_thickness = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 rear_wheel_thickness = System.BitConverter.ToSingle(resource,pos);
		 pos+=4;
		 maxrotmomentum = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 var50 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 breakpower = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 rear_axle_coeff = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 reverse_trans_coeff = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		gear_trans_coeff = new float[14];
		for (int i = 0; i<14;i++)
		{
			gear_trans_coeff[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}

		 tach_with_max_power2 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 cargo_and_body_pick_load = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 body_mass = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 body_height = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 body_square = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 center_of_cargo_bottom = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		driving_wheels = new int [8];
		for (int i = 0; i<8;i++)
		{
			driving_wheels[i] = System.BitConverter.ToInt32(resource,pos);
			pos+=4;
		}
		steering_wheels = new int [8];
		for (int i = 0; i<8;i++)
		{
			steering_wheels[i] = System.BitConverter.ToInt32(resource,pos);
			pos+=4;
		}
		touching_road_wheels = new int [8];
		for (int i = 0; i<8;i++)
		{
			touching_road_wheels[i] = System.BitConverter.ToInt32(resource,pos);
			pos+=4;
		}

		 OV = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 tangViewer = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 I = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		RS = new Vector3[4];
		for (int i = 0; i<4;i++)
		{
			RS[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}
		pos+=276;
		SuspensionMin = new float[4];
		for (int i = 0; i<4;i++)
		{
			SuspensionMin[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}
		SuspensionMax = new float[4];
		for (int i = 0; i<4;i++)
		{
			SuspensionMax[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}
		KS = new float[8];
		for (int i = 0; i<8;i++)
		{
			KS[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}
		CS = new float[8];
		for (int i = 0; i<8;i++)
		{
			CS[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}
		CollisionRadius = System.BitConverter.ToSingle(resource,pos);
		pos+=4; 
		CollisionCenter = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 var216 = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 rubber_collision_coeff = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 slide_collision_coeff = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 CornerMarkCount = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 AddColPntsCount = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		CornerMark = new Vector3[20];
		for (int i = 0; i<20;i++)
		{
			CornerMark[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}
		AddColPnts = new Vector3[20];
		for (int i = 0; i<20;i++)// непонятная хрень вот тута, хз
		{
			AddColPnts[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}
		x7 = new float[9];
		for (int i = 0; i<9;i++)// непонятная хрень вот тута, хз
		{
			x7[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}

		 CornerMarkWCount = System.BitConverter.ToInt32(resource,pos);
		CornerMarkW = new Vector3[4];
		pos+=4;
		for (int i = 0; i<4;i++)// непонятная хрень вот тута, хз
		{
			CornerMarkW[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}
		CollisionPlane = new Vector4[20];
		for (int i = 0; i<20;i++)// непонятная хрень вот тута, хз
		{
			CollisionPlane[i] = new Vector4(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4),System.BitConverter.ToSingle(resource,pos+12));
			pos+=16;
		}
		
		 var443 = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 var444 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 var445 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		tvCameraPosition = new Vector3[5];
		for (int i = 0; i<5;i++)// непонятная хрень вот тута, хз
		{
			tvCameraPosition[i] = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
			pos+=12;
		}

		 CameraCount = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 tvCameraRightCorner = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 SmokePos0= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 SmokeDir0= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 SmokePos1= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 SmokeDir1= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FlamePos= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraFl= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraFr= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraBls= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraBrs= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraBl= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraBr= new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 horse_power = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 price = System.BitConverter.ToInt32(resource,pos);
		pos+=4;

		 glowwing = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 LowSaddle = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 TopSaddle = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 TowHook = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 Coupler = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 shiftSaddle = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 CouplerLength = System.BitConverter.ToSingle(resource,pos);
		pos+=4;

	//FPANEL
		 speedometr_max_speed = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 SpeedScale = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 tachometr_max_freq = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 TachoScale = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 FuelScale = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
	//FPANEL
		 CabRWindow = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 CabLWindow = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		CabFWindow = new Vector4(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+4),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+12));
		pos+=16;
		Drivers_neck_Angles = new float[7];
		for (int i = 0; i<7;i++)// непонятная хрень вот тута, хз
		{
			Drivers_neck_Angles[i] = System.BitConverter.ToSingle(resource,pos);
			pos+=4;
		}
		 var538 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;
		 var539 = System.BitConverter.ToSingle(resource,pos);
		pos+=4;

		CockpitSpace = byteRead.GetLine(resource,pos);
		pos+=60;
		tchFile = byteRead.GetLine(resource,pos);
		pos+=30;
		vch = byteRead.GetLine(resource,pos);
		pos+=30;
		veh_name = byteRead.GetLine(resource,pos);
		pos+=30;
		prefix = byteRead.GetLine(resource,pos);
		pos+=20;
		prefixCab = byteRead.GetLine(resource,pos);
		pos+=20;
		CarNode = byteRead.GetLine(resource,pos);
		pos+=30;
		 x8 = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 trailerType = System.BitConverter.ToInt32(resource,pos);
		pos+=4;
		 FaraPos = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraDir = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		 FaraWidth = new Vector3(System.BitConverter.ToSingle(resource,pos),System.BitConverter.ToSingle(resource,pos+8),System.BitConverter.ToSingle(resource,pos+4));
		pos+=12;
		//name = CarNode;
	}
	
}
