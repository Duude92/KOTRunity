using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class Collision23 : MonoBehaviour {

    MeshCollider col;

    public void SetParams(Mesh me,bool convex)
    {
        col = gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = me;
        col.convex = true;
    }
	/*public void OnTriggerEnter (Collider collider)
	{
		
		//Rooms = GameObject.Find("ap.b3d").GetComponent<B3DScript>().Rooms;
		//Debug.Log(Rooms[0].name);
		if (collider.gameObject.GetComponent<LoadTrigger>())
		{
			CameraControl camera =  GameObject.Find("Main Camera").GetComponent<CameraControl>();
			string name = collider.gameObject.GetComponent<LoadTrigger>().roomName;
			camera.ChangeCurrentRoom(name);
		}
		else if (collider.gameObject.GetComponent<LODCustom>())
		{
			//Debug.Log("collider: "+collider,collider);
			collider.gameObject.GetComponent<LODCustom>().SwitchState(0);
		}

	}*/

    //public void OnTriggerEnter(Collider col)
    void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning(collision.transform.name);
        //collider.transform.LookAt(transform.position,)
        //Debug.Log(1);
        //Collision collision = new Collision();
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log( ReturnDirection( contact.thisCollider.gameObject, this.gameObject ) );
            Vector3 vec = new Vector3();
            vec = -(contact.thisCollider.transform.position - contact.point);
            HitDirection hd = HitDirection.None;

            //if (>vec>)

            Debug.DrawLine(contact.thisCollider.transform.position, contact.point, Color.blue,10f);
            


            Debug.Log(vec);

            Debug.Log(contact.thisCollider.gameObject,contact.thisCollider.gameObject);
            Debug.Log(contact.otherCollider.gameObject,contact.otherCollider.gameObject);
            Debug.Log(contact.point+"   :  "+contact.thisCollider.transform.position);
            Debug.DrawRay(contact.point, contact.normal, Color.white,10f);
        }
    }
    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right, FR, FL, BR, BL }
     private HitDirection ReturnDirection( GameObject Object, GameObject ObjectHit ){
         
         HitDirection hitDirection = HitDirection.None;
        
         RaycastHit MyRayHit;
         Vector3 direction = ( Object.transform.position - ObjectHit.transform.position ).normalized;
         Ray MyRay = new Ray( ObjectHit.transform.position, direction );
         
         if ( Physics.Raycast( MyRay, out MyRayHit ) ){
                 
             if ( MyRayHit.collider != null ){
                 
                 Vector3 MyNormal = MyRayHit.normal;
                 MyNormal = MyRayHit.transform.TransformDirection( MyNormal );
                 
                 
                 if( MyNormal == MyRayHit.transform.up ){ hitDirection = HitDirection.Top; }
                 if( MyNormal == -MyRayHit.transform.up ){ hitDirection = HitDirection.Bottom; }
                 if( MyNormal == MyRayHit.transform.forward ){ hitDirection = HitDirection.Forward; }
                 if( MyNormal == -MyRayHit.transform.forward ){ hitDirection = HitDirection.Back; }
                 if( MyNormal == MyRayHit.transform.right ){ hitDirection = HitDirection.Right; }
                 if( MyNormal == -MyRayHit.transform.right ){ hitDirection = HitDirection.Left; }
             }    
         }
         return hitDirection;
     }

    
}