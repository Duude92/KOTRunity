using UnityEngine;
using System.Collections.Generic;
public interface IVerticesBlock 
{
    // [System.Obsolete("Use vertices instead")]
    // List<Mesh> mesh{get;set;}
    List<Vector3> vertices{get;set;}
    List<Vector2> uv{get;set;}
    List<Vector2> uv1{get;set;}
    List<Vector3> normals{get;set;}
}