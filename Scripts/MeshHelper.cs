using UnityEngine;
using System.Collections.Generic;
using System;
[ExecuteInEditMode]
public class MeshHelper : MonoBehaviour
{
    [SerializeField] private List<Vector3> vertices;
    [SerializeField] private List<Vector2> uv;
    [SerializeField] private List<Vector3> normals;
    [SerializeField] bool update = false;
    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        vertices = new List<Vector3>();
        uv = new List<Vector2>();
        normals = new List<Vector3>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.sharedMesh;
        vertices.AddRange(mesh.vertices);
        uv.AddRange(mesh.uv);
        normals.AddRange(mesh.normals);
    }

    void Update()
    {
        if (update)
        {
            update = false;
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
        }
    }

}