/**
 * A simple car physics script using wheel colliders.
 * Jaap Kreijkamp [jaap] at [ctrl-j.com.au]
 *
 * orientation should be that front of car is in direction of
 * the 'blue arrow' in Unity, the roof should be in direction of
 * the green angle. Thus with rotation 0, 0, 0, adding 1 to Z
 * will move car 1m forward, adding 1 to Y will move car 1m
 * upward. The wheels should be children of the car object this
 * script is added to and connected to the wheelFL, wheelFR, ...
 * variables.
 *
 * Please modify script and do whatever you like with it,
 * in it's current state it should give a working car,
 * but by no means perfect (or even close) behavior.
 * It's my first attempt and don't really need in my current
 * project so haven't put too much effort into it to perfect
 * it. As often people are looking for help to getting
 * a car working with wheel colliders, I'd appreciate when
 * improvements are posted back on the unity forums.
 *
 * Lastly, thanks to all the people helping me on forums (especially
 * the order of initialisation problem and other example
 * code that helped me much in learning how to do stuff like
 * this).
 */
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public enum JWheelDrive {
    Front = 0,
    Back = 1,
    All = 2
}
 
 
public class JCar : MonoBehaviour { 
 
    // if connected the controls will block if object not  active
    // (for example steer only if car camera is active).
    public vehicle veh;
    public List<Transform> Wheels = new List<Transform>();
    public List<Transform> WheelsCol = new List<Transform>();
    public GameObject checkForActive;
 
    /*public Transform wheelFR; // connect to Front Right Wheel transform
    public Transform wheelFL; // connect to Front Left Wheel transform
    public Transform wheelBR; // connect to Back Right Wheel transform
    public Transform wheelBL; // connect to Back Left Wheel transform*/
 
    //public float suspensionDistance = 0.2f; // amount of movement in suspension
    /*public float springs = 1000.0f; // suspension springs
    public float dampers = 2f; // how much damping the suspension has
    public float wheelRadius = 0.25f; // the radius of the wheels
    public float torque = 100f; // the base power of the engine (per wheel, and before gears)
    public float brakeTorque = 2000f; // the power of the braks (per wheel)*/
    public float torque;
    public float brakeTorque;
    public Vector3 shiftCentre = new Vector3(0.0f, -0.25f, 0.0f); // offset of centre of mass
 
    public float maxSteerAngle = 30.0f; // max angle of steering wheels
	//public JWheelDrive wheelDrive = JWheelDrive.Front;
    public JWheelDrive wheelDrive; // which wheels are powered
 
	public float idleRPM = 500.0f; // idle rpm
	public float maxRPM;
    public float shiftDownRPM; // rpm script will shift gear down
    public float shiftUpRPM; // rpm script will shift gear up
 
    /* public float fwdStiffness = 0.9f; // for wheels, determines slip
    public float swyStiffness = 0.9f; // for wheels, determines slip*/
 
    // gear ratios (index 0 is reverse)
    public float[] gears;
	public int gearsCount;
 
    // automatic, if true car shifts automatically up/down
    public bool automatic = true;
 
    public float killEngineSoundTimeout = 3.0f; // time until engine sound is cut off (in s.)
 
    // table of efficiency at certain RPM, in tableStep RPM increases, 1.0f is 100% efficient
    // at the given RPM, current table has 100% at around 2000RPM
    float[] efficiencyTable = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 1.0f, 1.0f, 0.95f, 0.80f, 0.70f, 0.60f, 0.5f, 0.45f, 0.40f, 0.36f, 0.33f, 0.30f, 0.20f, 0.10f, 0.05f };
 
    // the scale of the indices in table, so with 250f, 750RPM translates to efficiencyTable[3].
    float efficiencyTableStep = 250.0f;
 
    int currentGear = 1; // duh.

    
 
    // shortcut to the component audiosource (engine sound).
    AudioSource audioSource;
 
    // every wheel has a wheeldata struct, contains useful wheel specific info
    class WheelData {
        public Transform transform;
        public GameObject go;
        public WheelCollider col;
        public Vector3 startPos;
        public float rotation = 0.0f;
        public float maxSteer;
        public bool motor;
    };
 
    WheelData[] wheels; // array with the wheel data
 
    // setup wheelcollider for given wheel data
    // wheel is the transform of the wheel
    // maxSteer is the angle in degrees the wheel can steer (0f for no steering)
    // motor if wheel is driven by engine or not
    WheelData SetWheelParams(Transform wheel, float maxSteer, bool motor) {
        if (wheel == null) {
            throw new System.Exception("wheel not connected to script!");
        }
        WheelData result = new WheelData(); // the container of wheel specific data
 
        // we create a new gameobject for the collider and move, transform it to match
        // the position of the wheel it represents. This allows us to do transforms
        // on the wheel itself without disturbing the collider.
        GameObject go = new GameObject("WheelCollider");
        go.transform.parent = transform; // the car, not the wheel is parent
        go.transform.position = wheel.position; // match wheel pos
 
        // create the actual wheel collider in the collider game object
        WheelCollider col = (WheelCollider) go.AddComponent(typeof(WheelCollider));
        col.motorTorque = 0.0f;
 
        // store some useful references in the wheeldata object
        result.transform = wheel; // access to wheel transform 
        result.go = go; // store the collider game object
        result.col = col; // store the collider self
        result.startPos = go.transform.localPosition; // store the current local pos of wheel
        result.maxSteer = maxSteer; // store the max steering angle allowed for wheel
        result.motor = motor; // store if wheel is connected to engine
 
        return result; // return the WheelData
    }
 
    // Use this for initialization
    void Start()
    {
       // StartJcar();
    }
    public void StartJcar () {
        // 4 wheels, if needed different size just modify and modify
        // the wheels[...] block below.
        wheels = new WheelData[Wheels.Count];
 
        // setup wheels
        bool frontDrive = (wheelDrive == JWheelDrive.Front) || (wheelDrive == JWheelDrive.All);
        bool backDrive = (wheelDrive == JWheelDrive.Back) || (wheelDrive == JWheelDrive.All);
 
        // we use 4 wheels, but you can change that easily if neccesary.
        // this is the only place that refers directly to wheelFL, ...
        // so when adding wheels, you need to add the public transforms,
        // adjust the array size, and add the wheels initialisation here.

        for (int i = 0; i<Wheels.Count;i++)
        {
            wheels[i] = new WheelData();
            wheels[i].col = WheelsCol[i].GetComponent<WheelCollider>();
            wheels[i].go = Wheels[i].gameObject;
            //Debug.Log(i+"   WhNum:  "+veh.WheelNum[i]);
            wheels[i].startPos = Wheels[i].localPosition;
            wheels[i].transform = Wheels[i];
        }


        for (int i = 0; i<Wheels.Count;i++)//TODO: переделать нормально блять
        {
            if (veh.steering_wheels[veh.WheelNum[i]] == 1)
                wheels[i].maxSteer = maxSteerAngle;

            wheels[i].motor = veh.driving_wheels[veh.WheelNum[i]] == 1;

        }


        /* wheels[0] = SetWheelParams(wheelFR, maxSteerAngle, frontDrive);
        wheels[1] = SetWheelParams(wheelFL, maxSteerAngle, frontDrive);
        wheels[2] = SetWheelParams(wheelBR, 0.0f, backDrive);
        wheels[3] = SetWheelParams(wheelBL, 0.0f, backDrive);*/
 
        // found out the hard way: some parameters must be set AFTER all wheel colliders
        // are created, like wheel mass, otherwise your car will act funny and will
        // flip over all the time.

        //---------------------
        /*foreach (WheelData w in wheels) {
            WheelCollider col = w.col;
            col.suspensionDistance = suspensionDistance;
            JointSpring js = col.suspensionSpring;
            js.spring = springs;
            js.damper = dampers;            
            col.suspensionSpring = js;
            col.radius = wheelRadius;
            col.mass = wheelWeight;
 
            // see docs, haven't really managed to get this work
            // like i would but just try out a fiddle with it.
            WheelFrictionCurve fc = col.forwardFriction;
            fc.asymptoteValue = 5000.0f;
            fc.extremumSlip = 1.0f;
            fc.asymptoteSlip = 2.0f;
            fc.stiffness = fwdStiffness;
            col.forwardFriction = fc;
            fc = col.sidewaysFriction;
            fc.asymptoteValue = 7500.0f;
            fc.asymptoteSlip = 2.0f;
            fc.stiffness = swyStiffness;
            col.sidewaysFriction = fc;
        }*/
 
        // we move the centre of mass (somewhere below the centre works best.)
        GetComponent<Rigidbody>().centerOfMass += shiftCentre;
 
        // shortcut to audioSource should be engine sound, if null then no engine sound.
        audioSource = (AudioSource) GetComponent(typeof(AudioSource));
        if (audioSource == null) {
            Debug.Log("No audio source, add one to the car with looping engine noise (but can be turned off");
        }
 
    }
 
    void Update() {
		//if (Input.GetKeyDown("a")) {
        if (Input.GetKeyDown("x")) {
            ShiftUp();
        }
        if (Input.GetKeyDown("z")) {
            ShiftDown();
        }
    }
 
    float shiftDelay = 0.0f;
 
    // handle shifting a gear up
    public void ShiftUp() {
        float now = Time.timeSinceLevelLoad;
 
        // check if we have waited long enough to shift
        if (now < shiftDelay) return;
 
        // check if we can shift up
        if (currentGear < gearsCount) {
            currentGear ++;
 
            // we delay the next shift with 1s. (sorry, hardcoded)
            shiftDelay = now + 1.0f;
        }
    }
 
    // handle shifting a gear down
    public void ShiftDown() {
        float now = Time.timeSinceLevelLoad;
 
        // check if we have waited long enough to shift
        if (now < shiftDelay) return;
 
        // check if we can shift down (note gear 0 is reverse)
        if (currentGear > 0) {
            currentGear --;
 
            // we delay the next shift with 1/10s. (sorry, hardcoded)
            shiftDelay = now + 0.1f;
        }
    }
 
    float wantedRPM = 0.0f; // rpm the engine tries to reach
    float motorRPM = 0.0f;
    float killEngine = 0.0f;
 
    // handle the physics of the engine
    void FixedUpdate () {
        float delta = Time.fixedDeltaTime;
 
        float steer = 0; // steering -1.0 .. 1.0
        float accel = 0; // accelerating -1.0 .. 1.0
        bool brake = false; // braking (true is brake)
 
        if ((checkForActive == null) || checkForActive.activeSelf) {
            // we only look at input when the object we monitor is
            // active (or we aren't monitoring an object).
            steer = Input.GetAxis("Horizontal");
            accel = Input.GetAxis("Vertical");
            brake = Input.GetButton("Jump");
        }
 
        // handle automatic shifting
        if (automatic && (currentGear == 1) && (accel < 0.0f)) {
            ShiftDown(); // reverse
        }
        else if (automatic && (currentGear == 0) && (accel > 0.0f)) {
            ShiftUp(); // go from reverse to first gear
        }
        else if (automatic && (motorRPM > shiftUpRPM) && (accel > 0.0f)) {
            ShiftUp(); // shift up
        }
        else if (automatic && (motorRPM < shiftDownRPM) && (currentGear > 1)) {
            ShiftDown(); // shift down
        }
        if (automatic && (currentGear == 0)) {
            accel = - accel; // in automatic mode we need to hold arrow down for reverse
        }
        if (accel < 0.0f) {
            // if we try to decelerate we brake.
            brake = true;
            accel = 0.0f;
            wantedRPM = 0.0f;
        }
 
        // the RPM we try to achieve.
        wantedRPM = (maxRPM * accel) * 0.1f + wantedRPM * 0.9f;
 
        float rpm = 0.0f;
        int motorizedWheels = 0;
        bool floorContact = false;
 
        // calc rpm from current wheel speed and do some updating
        foreach (WheelData w in wheels) {
            WheelHit hit;
            WheelCollider col = w.col;
 
            // only calculate rpm on wheels that are connected to engine
            if (w.motor) {
                rpm += col.rpm;
                motorizedWheels++;
            }
 
            // calculate the local rotation of the wheels from the delta time and rpm
            // then set the local rotation accordingly (also adjust for steering)
            w.rotation = Mathf.Repeat(w.rotation + delta * col.rpm * 360.0f / 60.0f, 360.0f);
            w.transform.localRotation = Quaternion.Euler(w.rotation, col.steerAngle, 0.0f);
 
            // let the wheels contact the ground, if no groundhit extend max suspension distance
            Vector3 lp = w.transform.localPosition;
            if (col.GetGroundHit(out hit)) {
                lp.y -= Vector3.Dot(w.transform.position - hit.point, transform.up) - col.radius;
                floorContact = floorContact || (w.motor);
            }
            else {
                lp.y = w.startPos.y - col.suspensionDistance;
            }
            w.transform.localPosition = lp;
        }
        // calculate the actual motor rpm from the wheels connected to the engine
        // note we haven't corrected for gear yet.
        if (motorizedWheels > 1) {
            rpm = rpm / motorizedWheels;
        }
 
        // we do some delay of the change (should take delta instead of just 95% of
        // previous rpm, and also adjust or gears.
        motorRPM = 0.95f * motorRPM + 0.05f * Mathf.Abs(rpm * gears[currentGear]);
        if (motorRPM > maxRPM) motorRPM = maxRPM;
 
        // calculate the 'efficiency' (low or high rpm have lower efficiency then the
        // ideal efficiency, say 2000RPM, see table
        int index = (int) (motorRPM / efficiencyTableStep);
        if (index >= efficiencyTable.Length) index = efficiencyTable.Length - 1;
        if (index < 0) index = 0;
 
        // calculate torque using gears and efficiency table
		//float newTorque = torque * gears[currentGear] * efficiencyTable[index];
        float newTorque = torque * gears[currentGear];
 
        // go set torque to the wheels
        foreach (WheelData w in wheels) {
            WheelCollider col = w.col;
 
            // of course, only the wheels connected to the engine can get engine torque
            if (w.motor) {
                // only set torque if wheel goes slower than the expected speed
                if (Mathf.Abs(col.rpm) > Mathf.Abs(wantedRPM)) {
                    // wheel goes too fast, set torque to 0
                    col.motorTorque = 0;
                }
                else {
                    // 
                    float curTorque = col.motorTorque;
                    col.motorTorque = curTorque * 0.9f + newTorque * 0.1f;
                }
            }
            // check if we have to brake
            col.brakeTorque = (brake)?brakeTorque:0.0f;
 
            // set steering angle
            col.steerAngle = steer * w.maxSteer;
        }
 
        // if we have an audiosource (motorsound) adjust pitch using rpm        
        if (audioSource != null) {
            // calculate pitch (keep it within reasonable bounds)
            float pitch = Mathf.Clamp(1.0f + ((motorRPM - idleRPM) / (shiftUpRPM - idleRPM) * 2.5f), 1.0f, 10.0f);
            audioSource.pitch = pitch;
 
            if (motorRPM > 100) {
                // turn on sound if it's not playing yet and RPM is > 100.
                if (!audioSource.isPlaying) {
                    audioSource.Play();
                }
                // how long we should wait with engine RPM <= 100 before killing engine sound
                killEngine = Time.time + killEngineSoundTimeout;
            }
            else if ((audioSource.isPlaying) && (Time.time > killEngine)) {
                // standing still, kill engine sound.
                audioSource.Stop();
            }
        }
    }
 
    public void OnGUI() {
        if (checkForActive.activeSelf) {
            // calculate actual speed in Km/H (SI metrics rule, so no inch, yard, foot,
            // stone, or other stupid length measure!)
            float speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
 
            // message to display
            string msg = "Speed " + speed.ToString("f0") + "Km/H, " + motorRPM.ToString("f0") + "RPM, current gear " + currentGear + ", gear_coeff " + gears[currentGear]; //  + " torque " + newTorque.ToString("f2") + ", efficiency " + table[index].ToString("f2");
 
            GUILayout.BeginArea(new Rect(Screen.width -400 - 32, 32, 400, 40), GUI.skin.window);
            GUILayout.Label(msg);
            GUILayout.EndArea();
        }
    }
}