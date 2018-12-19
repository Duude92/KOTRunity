using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

class GeneratorInvoker : MonoBehaviour {
	public string invokeName;
	public float Scale;
	public int Type,Type2;
	public int hernja;
	public GameObject resOb;

	public List<float> Params = new List<float>();

	public void Generate()
	{
		if (invokeName == "$$TreeGenerator1")
		{
			
			MeshFilter meshF = gameObject.AddComponent<MeshFilter>();
			Mesh me = meshF.mesh;
			me.Clear();

			//////Polygon of a treee
			float half = 0;
			string texName = "";

			if (18>Type)
			{
				texName = "tree01";
				half = 0.5f;
			}
			else if (32>Type&&Type>17)
			{
				texName = "tree01";
				half = 1f;

			}
			else if (48>Type&&Type>31)
			{
				texName = "tree23";
				half = 0.5f;

			}
			else if (65>Type&&Type>47 )
			{
				texName = "tree23";
				half = 1f;
			}
			//tree types 65,67,69,71,73,75,77,79 requires tree4 material in res
			else if (80>Type&&Type>46 )
			{
				texName = "tree45";
				half = 0.5f;
			}
			//tree types 91,93,95 requires tree5 material in res
			else if (96>Type&&Type>=80 )
			{
				texName = "tree45";
				half = 1f;
			}
			else if (550>Type&&Type>=540 )
			{
				texName = "tree23";
				half = 0.5f;
			}
			else if (570>Type&&Type>=560 )
			{
				texName = "tree23";
				half = 1f;
			}
			else
			{
				texName = "zero";
			}

			gameObject.AddComponent<MeshRenderer>().material = resOb.GetComponent<Materials>().maths[resOb.GetComponent<Materials>().FindIndexByString(texName)];
			gameObject.AddComponent<BoxCollider>();
			
			//Rigidbody gameObjectsRigidBody = gameObject.AddComponent<Rigidbody>();
			//gameObjectsRigidBody.mass = 5;
			
			BoxCollider BC = gameObject.GetComponent<BoxCollider>();
			BC.center=new Vector3(0f,2f,0f); 
			BC.size=new Vector3(0.5f,4f,0.5f);
			
			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> triangles = new List<int>();


			vertices.Add(new Vector3(Scale/5 * -1,0,0));
			vertices.Add(new Vector3(Scale/5,0,0));
			vertices.Add( new Vector3(Scale/5 * -1,Scale,0));
			vertices.Add( new Vector3(Scale/5,Scale,0));

			vertices.Add(new Vector3(0,0,Scale/5 * -1));
			vertices.Add(new Vector3(0,0,Scale/5));
			vertices.Add( new Vector3(0,Scale,Scale/5 * -1));
			vertices.Add( new Vector3(0,Scale,Scale/5));
			
			vertices.Add(new Vector3(Scale/7 * -1,0,Scale/7 * -1));
			vertices.Add(new Vector3(Scale/7,0,Scale/7));
			vertices.Add( new Vector3(Scale/7 * -1,Scale,Scale/7 * -1));
			vertices.Add( new Vector3(Scale/7,Scale,Scale/7));
			
			vertices.Add(new Vector3(Scale/7 * -1,0,Scale/7));
			vertices.Add(new Vector3(Scale/7,0,Scale/7 * -1));
			vertices.Add( new Vector3(Scale/7 * -1,Scale,Scale/7));
			vertices.Add( new Vector3(Scale/7,Scale,Scale/7 * -1));
			
			uvs.Add( new Vector2(half - 0.49f,1f));
			uvs.Add( new Vector2(half - 0.05f,1f));
			uvs.Add( new Vector2(half - 0.49f,0));
			uvs.Add(new Vector2(half - 0.05f,0));
			
			uvs.Add( new Vector2(half - 0.05f,1f));
			uvs.Add( new Vector2(half - 0.49f,1f));
			uvs.Add( new Vector2(half - 0.05f,0));
			uvs.Add(new Vector2(half - 0.49f,0));

			uvs.Add( new Vector2(half - 0.49f,1f));
			uvs.Add( new Vector2(half - 0.05f,1f));
			uvs.Add( new Vector2(half - 0.49f,0));
			uvs.Add(new Vector2(half - 0.05f,0));
			
			uvs.Add( new Vector2(half - 0.05f,1f));
			uvs.Add( new Vector2(half - 0.49f,1f));
			uvs.Add( new Vector2(half - 0.05f,0));
			uvs.Add(new Vector2(half - 0.49f,0));
			
			triangles.AddRange(new int[]{0,1,2,3,2,1,0,2,1,3,1,2,4,5,6,7,6,5,4,6,5,7,5,6,8,9,10,11,10,9,8,10,9,11,9,10,12,13,14,15,14,13,12,14,13,15,13,14});

			me.vertices = vertices.ToArray();
			me.uv = uvs.ToArray();
			me.triangles = triangles.ToArray();
			
			me.RecalculateBounds();
			

		}
		else if (invokeName == "$$GeneratorOfTerrain")
		{
			MeshFilter meshF = gameObject.AddComponent<MeshFilter>();
			Mesh me = meshF.mesh;
			me.Clear();
			int res = 257;
			DirectoryInfo ENVpath = new DirectoryInfo(@"db2\ENV\");
			FileInfo[] Files = ENVpath.GetFiles("terrain1.raw");	
			//Debug.Log("raw0 Length:	"+Files[0].Length)	;
			byte[] filebuff = new byte[Files[0].Length];
			File.OpenRead(Files[0].FullName).Read(filebuff,0,(int)Files[0].Length);
			List<float> vert = new List<float>();

			for (int i = 0;i<Files[0].Length;i+=2)
			{
				vert.Add((float)filebuff[i]/10f);
				if (filebuff[i]!=filebuff[i+1])
				{
					Debug.LogError("bytes not equal");
				}
			}

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();

			for (int i = 0;i<res;i++)
			{
				for (int j = 0;j<res;j++)
				{
					uvs.Add(new Vector2(i*0.15f,j*0.15f));
					vertices.Add(new Vector3(i,vert[(i*res)+j],j));
				}
			}


			List<int> faces = new List<int>();
			for (int i = 0; i<res-3;i++)
			{
				for (int j = 0;j<res-1;j++)
				{
					faces.AddRange(new int[]{i*res+j,i*res+j+1,(i+1)*res+j});
					
					if (j!=0)
					{
						
						faces.AddRange(new int[]{i*res+j,(i+1)*res+j,(i+1)*res+j-1});
					}
					
				}
			}
			me.vertices = vertices.ToArray();
			me.triangles = faces.ToArray();
			me.uv = uvs.ToArray();
			me.RecalculateNormals();

			gameObject.transform.localScale = new Vector3(4f,4f,4f);
			gameObject.transform.position = new Vector3(2037,424.5f,-3992);
			gameObject.GetComponent<Renderer>().material = GameObject.Find("AA").GetComponent<Materials>().maths[52];

		}
		else if (invokeName == "$$People")	
		{
			MeshFilter meshF = gameObject.AddComponent<MeshFilter>();
			Mesh me = meshF.mesh;
			me.Clear();

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> triangles = new List<int>();


			vertices.Add(new Vector3(Params[0],Params[2],Params[1]));
			vertices.Add(new Vector3(Params[5],Params[7],Params[6]));
			vertices.Add( new Vector3(Params[10],Params[12],Params[11]));
			vertices.Add( new Vector3(Params[15],Params[17],Params[16]));

			uvs.Add( new Vector2(-Params[3],Params[4]));
			uvs.Add( new Vector2(-Params[8],Params[9]));
			uvs.Add( new Vector2(-Params[13],Params[14]));
			uvs.Add(new Vector2(-Params[18],Params[19]));			

			
			

			triangles.AddRange(new int[]{0,1,3,3,1,2});
			gameObject.AddComponent<MeshRenderer>().material = resOb.GetComponent<Materials>().maths[resOb.GetComponent<Materials>().FindIndexByString("people"+Type2)];


			me.vertices = vertices.ToArray();
			me.uv = uvs.ToArray();
			me.triangles = triangles.ToArray();
			
			me.RecalculateBounds();
		}
	}
}
