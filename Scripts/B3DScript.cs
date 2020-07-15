using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class B3DScript : MonoBehaviour
{

    Stream s;
    BinaryReader br;
    public List<int> TexInts = new List<int>();
    public List<GameObject> SwitchBlocks = new List<GameObject>();
    public List<GameObject> InvokeBlocks = new List<GameObject>();
    public List<GameObject> Blocks05 = new List<GameObject>();
    public List<GameObject> Rooms = new List<GameObject>();
    public List<GameObject> LoadTriggers = new List<GameObject>();
    public FileInfo file;
    public bool ready = false;

    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector2> UV = new List<Vector2>();
    public List<Vector2> UV1 = new List<Vector2>();
    public List<Vector3> normals = new List<Vector3>();

    public void Disable()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartB3D()
    {

        br = new BinaryReader(File.Open(file.FullName, FileMode.Open));


        byte[] resource = new byte[file.Length];
        br.Read(resource, 0, (int)file.Length);
        int pos = 0;
        br.Close();
        br = null;

        byte[] buff = new byte[4];
        System.Array.Copy(resource, pos, buff, 0, 4);
        pos += 4;

        string binText = System.Text.Encoding.UTF8.GetString(buff);
        if ((binText == "B3D\0") || (binText == "b3d\0"))
        {
            pos += 20;
            int TexNum = System.BitConverter.ToInt32(resource, pos);
            pos += 4;
            Texture2D[] texture = new Texture2D[TexNum];

            Sprite[] sprites = new Sprite[TexNum];
            GameObject curLodObj = null;
            short lodCnt = 0;
            List<LOD> lods = new List<LOD>();
            List<Renderer> rends = new List<Renderer>();
            int nrml0 = 0;
            int nrml1 = 0;
            //bool LodOpened = false;


            for (int i = 0; i < TexNum; i++)
            {
                byte[] texName = new byte[32];
                System.Array.Copy(resource, pos, texName, 0, 32);
                pos += 32;

                Materials mat = GetComponent<Materials>();
                //Debug.Log(mat.material+"		",rootObj);
                //Debug.Log(System.Text.Encoding.UTF8.GetString(texName));
                string matName = System.Text.Encoding.UTF8.GetString(texName);
                int a = mat.FindIndexByString(matName);//.Trim(new char[]{'\0'}));

                TexInts.Add(a);
                texture[i] = new Texture2D(1, 1);
                texture[i].name = System.Text.Encoding.UTF8.GetString(texName);

                sprites[i] = Sprite.Create(texture[i], new Rect(0, 0, 1, 1), new Vector2(0f, 0f), 100);
            }
            int Ccase = System.BitConverter.ToInt32(resource, pos);
            pos += 4;
            if (Ccase == 111)
            {
                int lvl = 0;
                GameObject lastGameObject = null;
                GameObject newObject = null;
                GameObject rootObj = gameObject;
                vertices = new List<Vector3>();
                UV = new List<Vector2>();
                normals = new List<Vector3>();
                for (; ; )
                {
                    lvl++;
                    Ccase = System.BitConverter.ToInt32(resource, pos);
                    pos += 4;
                    if (Ccase == 222)
                    {
                        foreach (var go in rootObj.GetComponent<B3DScript>().Rooms)
                        {
                            go.GetComponent<Block19>().Init();
                        }

                        foreach (var go in LoadTriggers) //аар1
                        {

                            go.GetComponent<LoadTrigger>().Init(rootObj);
                        }



                        break;
                    }
                    else if (Ccase == 444)
                    {
                        newObject = new GameObject("444");
                        if (lastGameObject != null)
                        {
                            newObject.transform.SetParent(lastGameObject.transform);




                        }
                        else
                        {
                            newObject.transform.SetParent(rootObj.transform);
                            lastGameObject = rootObj;
                        }
                        if (lastGameObject.GetComponent<BlockType>())
                        {
                            if (lastGameObject.GetComponent<BlockType>().Type == 10)
                            {

                                /*float perc = Distance/(2000f+lodCnt);
                                Debug.Log(perc);

                                lods.Add(new LOD(perc,rends.ToArray()));*/
                                lods.Add(new LOD(1f / ((lodCnt + 1) * 5), rends.ToArray()));
                                rends = new List<Renderer>();
                                lodCnt++;
                            }
                        }

                        /*if (curLodObj == lastGameObject)
                        {
                            lods[lods.Length-2].renderers = rends.ToArray();
                            rends = new List<Renderer>();
                            curLodObj.GetComponent<LODGroup>().SetLODs(lods);
                        }*/
                        /*if (curSwitch == lastGameObject)
                        {
                            swIt++;
                        }*/
                    }
                    else if (Ccase == 555)
                    {


                        if ((curLodObj == lastGameObject))
                        {
                            ///lodCnt--;
                            lods.Add(new LOD(0f, rends.ToArray()));
                            rends = new List<Renderer>();
                            curLodObj.GetComponent<LODGroup>().SetLODs(lods.ToArray());


                            curLodObj = null;
                            lods = new List<LOD>();
                        }


                        lastGameObject = lastGameObject.transform.parent.gameObject;
                    }
                    else if (Ccase == 333)
                    {
                        byte[] blockName = new byte[32];
                        System.Array.Copy(resource, pos, blockName, 0, 32);
                        pos += 32;
                        string s = System.Text.Encoding.UTF8.GetString(blockName).Trim(new char[] { '\0' });

                        int type = System.BitConverter.ToInt32(resource, pos);
                        pos += 4;
                        newObject = new GameObject(s);
                        BlockType bt = newObject.AddComponent<BlockType>();
                        bt.Type = type;
                        bt.MatNum = 0;

                        if (lastGameObject != null)
                        {
                            newObject.transform.SetParent(lastGameObject.transform);
                        }
                        else
                        {
                            newObject.transform.SetParent(rootObj.transform);
                            lastGameObject = rootObj;
                        }




                        if (type == 0)
                        {
                            bt.component = new Block00();
                            bt.component.thisObject = newObject;
                            pos += 16;
                            pos += 28;
                        }
                        else if (type == 1)
                        {
                            bt.component = new Block01();
                            pos += 32;
                            pos += 32;
                        }
                        else if (type == 2)
                        {
                            bt.component = new Block02();
                            pos += 16;
                            pos += 20;
                        }
                        else if (type == 3)
                        {
                            bt.component = new Block03();
                            pos += 16;
                            pos += 4;
                        }
                        else if (type == 4)
                        {
                            bt.component = new Block04();
                            pos += 16;
                            byte[] spcName = new byte[32];
                            System.Array.Copy(resource, pos, spcName, 0, 32);
                            pos += 32;

                            string SpaceName = System.Text.Encoding.UTF8.GetString(spcName).Trim(new char[] { '\0' });


                            GameObject SpcOb = GameObject.Find(SpaceName);

                            if (SpcOb != null)
                            {
                                Block24 Spc = SpcOb.GetComponent<Block24>();
                                newObject.transform.position = Spc.position;

                                Vector3 rotation = new Vector3();
                                float sy = Mathf.Sqrt(Spc.matrix[0][0] * Spc.matrix[0][0] + Spc.matrix[1][0] * Spc.matrix[1][0]);
                                bool singular = sy < 1e-6;
                                float rad = 180 / Mathf.PI;
                                if (!singular)
                                {
                                    rotation.x = Mathf.Atan2(Spc.matrix[2][1], Spc.matrix[2][2]) * rad;
                                    rotation.z = Mathf.Atan2(-Spc.matrix[2][0], sy) * rad;
                                    rotation.y = Mathf.Atan2(Spc.matrix[1][0], Spc.matrix[0][0]) * rad;
                                }
                                else
                                {
                                    rotation.x = Mathf.Atan2(-Spc.matrix[1][2], Spc.matrix[1][1]) * rad;
                                    rotation.z = Mathf.Atan2(-Spc.matrix[2][0], sy) * rad;
                                    rotation.y = 0;
                                }

                                newObject.transform.eulerAngles = rotation;
                            }

                            pos += 36;

                        }
                        else if (type == 5)
                        {

                            Block05 block = newObject.AddComponent<Block05>();
                            block.Read(resource, ref pos);
                            bt.component = block;
                            block.thisObject = newObject;
                        }
                        else if (type == 6)
                        {
                            pos += 16;
                            pos += 32;
                            pos += 32;
                            System.Array.Copy(resource, pos, buff, 0, 4);
                            pos += 4;
                            int i_null = System.BitConverter.ToInt32(buff, 0);
                            for (int i = 0; i < i_null; i++)
                            {
                                pos += 20;
                            }
                            pos += 4;
                        }
                        else if (type == 7)
                        {
                            bt.component = new Block07();
                            ((Block07)bt.component).script = this;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 8)
                        {
                            bt.component = newObject.AddComponent<Block08>();
                            ((Block08)bt.component).b3dObject = this;
                            ((Block08)bt.component).vertices = vertices;
                            ((Block08)bt.component).UV = UV;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 9)
                        {
                            pos += 16;

                            Block09 blk = newObject.AddComponent<Block09>();
                            bt.component = blk;
                            blk.xz4eEto = new Vector4(System.BitConverter.ToSingle(resource, pos), System.BitConverter.ToSingle(resource, pos + 8), System.BitConverter.ToSingle(resource, pos + 4), System.BitConverter.ToSingle(resource, pos + 12));
                            pos += 16;

                            pos += 4;//childCount
                        }
                        else if (type == 10)
                        {
                            var a = newObject.AddComponent<Block10>();

                            pos += 16;


                            a.Center = new Vector3(System.BitConverter.ToSingle(resource, pos), System.BitConverter.ToSingle(resource, pos + 8), System.BitConverter.ToSingle(resource, pos + 4));
                            a.Distance = System.BitConverter.ToSingle(resource, pos + 12);
                            pos += 16;
                            //System.Array.Copy(resource,pos,buff,0,4); 	
                            pos += 4;


                        }
                        else if (type == 12)
                        {
                            pos += 16;
                            pos += 12;
                            float height = System.BitConverter.ToSingle(resource, pos);
                            pos += 4;
                            newObject.AddComponent<BoxCollider>().size = new Vector3(10000f, 0f, 10000f);

                            newObject.transform.position = new Vector3(0, height * -1, 0);
                            pos += 12;

                        }
                        else if (type == 13)
                        {
                            pos += 16;
                            newObject.AddComponent<Block13>().a = System.BitConverter.ToInt32(resource, pos);
                            newObject.GetComponent<Block13>().b = System.BitConverter.ToInt32(resource, pos + 4);
                            int paramCount = System.BitConverter.ToInt32(resource, pos + 8);
                            newObject.GetComponent<Block13>().paramCount = paramCount;
                            pos += 4;
                            pos += 8;
                            //int i_null = System.BitConverter.ToInt32(buff,0);
                            for (int i = 0; i < paramCount; i++)
                            {
                                newObject.GetComponent<Block13>().Params.Add(System.BitConverter.ToSingle(resource, pos));
                                pos += 4;
                            }
                        }
                        else if (type == 14)
                        {
                            bt.component = new Block14();
                            bt.component.thisObject = newObject;
                            bt.component.Read(resource, ref pos);

                        }
                        else if (type == 16)
                        {
                            pos += 16;
                            pos += 44;
                        }
                        else if (type == 17)
                        {
                            pos += 16;
                            pos += 44;
                        }
                        else if (type == 18)
                        {
                            bt.component = new Block18();
                            bt.component.thisObject = newObject;
                            ((Block18)bt.component).script = this;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 19)
                        {
                            Block19 block = newObject.AddComponent<Block19>();
                            bt.component = block;
                            block.thisObject = newObject;
                            block.room = newObject;
                            rootObj.transform.parent.GetComponent<GameManager>().Rooms.Add(newObject);
                            Rooms.Add(newObject);
                            pos += 4;
                        }
                        else if (type == 20)
                        {
                            bt.component = new Block20();
                            bt.component.thisObject = newObject;
                            bt.component.Read(resource, ref pos);


                        }
                        else if (type == 21)
                        {
                            bt.component = new Block21();
                            ((Block21)bt.component).script = this;
                            bt.component.thisObject = newObject;
                            bt.component.Read(resource, ref pos);



                        }
                        else if (type == 23)
                        {
                            bt.component = new Block23();
                            bt.component.thisObject = newObject;
                            ((Block23)bt.component).script = this;
                            bt.component.Read(resource, ref pos);
                            // bool truck = false;
                            // //List<Mesh> meshe = new List<Mesh>();
                            // if (rootObj.name == "TRUCKS")
                            // {
                            //     truck = true;
                            // }


                            // List<int> faces_all = new List<int>();

                            // pos += 8;
                            // List<Vector3> verticesCol = new List<Vector3>();
                            // List<int> faces = new List<int>();



                            // int i_null = System.BitConverter.ToInt32(resource, pos);
                            // pos += 4;
                            // for (int i = 0; i < i_null; i++)
                            // {
                            //     pos += 4;
                            // }
                            // int triNum = System.BitConverter.ToInt32(resource, pos);
                            // pos += 4;
                            // int number = 0;
                            // for (int i = 0; i < triNum; i++)
                            // {
                            //     int vertNum = System.BitConverter.ToInt32(resource, pos);
                            //     pos += 4;
                            //     if (vertNum == 3)
                            //     {
                            //         faces.AddRange(new int[] { number, number + 2, number + 1 });
                            //         number += 3;
                            //     }
                            //     else if (vertNum == 4)
                            //     {
                            //         faces.AddRange(new int[] { (number), (number + 2), (number + 1), (number + 3), (number + 2), (number) });
                            //         number += 4;
                            //     }
                            //     else
                            //     {
                            //         for (int j = 0; j < vertNum - 2; j++)
                            //         {
                            //             faces.AddRange(new int[3] { number + j + 2, number + j + 1, number });

                            //         }
                            //         number += vertNum;

                            //     }
                            //     for (int j = 0; j < vertNum; j++)
                            //     {
                            //         var vert = new Vector3(System.BitConverter.ToSingle(resource, pos), System.BitConverter.ToSingle(resource, pos + 8), System.BitConverter.ToSingle(resource, pos + 4));
                            //         /*var a = newObject.AddComponent<SphereCollider>();
                            //         a.center = vert;
                            //         a.radius = 0.1f;*/
                            //         pos += 12;
                            //         verticesCol.Add(vert);
                            //         /*var a = new GameObject();
                            //             a.transform.SetParent(newObject.transform);
                            //             a.transform.position = new Vector3(vert.x,vert.z,vert.y);	*/
                            //     }
                            //     if (truck)
                            //     {
                            //         int Count = faces.Count / 3;
                            //         for (int j = 0; j < Count; j++)
                            //         {
                            //             List<Vector3> newTri = Extrude(verticesCol[faces[j]], verticesCol[faces[j + 1]], verticesCol[faces[j + 2]]);
                            //             verticesCol.AddRange(newTri);
                            //             faces.Add(verticesCol.Count - 3);
                            //             faces.Add(verticesCol.Count - 2);
                            //             faces.Add(verticesCol.Count - 1);

                            //         }



                            //         Mesh me = new Mesh();
                            //         MeshCollider col = newObject.AddComponent<MeshCollider>();
                            //         me.vertices = verticesCol.ToArray();
                            //         me.triangles = faces.ToArray();
                            //         me.name = ((DamageKey)i).ToString();


                            //         col.sharedMesh = me;
                            //         col.convex = true;
                            //         faces_all.AddRange(faces);
                            //         faces = new List<int>();
                            //     }


                            // }
                            // {//Mesh me conflict
                            //     Mesh me = new Mesh();
                            //     MeshCollider col = newObject.AddComponent<MeshCollider>();
                            //     me.vertices = verticesCol.ToArray();
                            //     if (!truck)
                            //     {
                            //         me.triangles = faces.ToArray();
                            //         col.sharedMesh = me;
                            //     }
                            // }
                            // /*else
                            // {
                            // 	me.triangles = faces_all.ToArray();
                            // 	col.convex = true;
                            // } */



                        }
                        else if (type == 24)
                        {

                            Block24 p = newObject.AddComponent<Block24>();
                            bt.component = p;
                            bt.component.thisObject = newObject;

                            byte[] bts = new byte[12];

                            System.Array.Copy(resource, pos, bts, 0, 12); pos += 12;
                            //p.matrix = new [3][];
                            p.matrix[0] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));

                            System.Array.Copy(resource, pos, bts, 0, 12); pos += 12;
                            p.matrix[1] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));
                            System.Array.Copy(resource, pos, bts, 0, 12); pos += 12;
                            p.matrix[2] = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 4), System.BitConverter.ToSingle(bts, 8));
                            System.Array.Copy(resource, pos, bts, 0, 12); pos += 12;
                            p.position = new Vector3(System.BitConverter.ToSingle(bts, 0), System.BitConverter.ToSingle(bts, 8), System.BitConverter.ToSingle(bts, 4));
                            pos += 8;
                        }
                        else if (type == 25)
                        {
                            pos += 12;
                            pos += 32;
                            pos += 4;
                            pos += 40;
                        }
                        else if (type == 28)
                        {
                            List<int> faces = new List<int>();
                            List<Vector2> uvs = new List<Vector2>();
                            faces.AddRange(new int[] { 0, 1, 2, 3, 0, 2 });

                            Mesh me = new Mesh();
                            me.Clear();
                            me.vertices = vertices.ToArray();
                            me.triangles = faces.ToArray();
                            uvs.Add(new Vector2(0, 0));
                            uvs.Add(new Vector2(1, 0));
                            uvs.Add(new Vector2(0, 1));
                            uvs.Add(new Vector2(1, 1));
                            me.uv = uvs.ToArray();
                            me.RecalculateBounds();
                            int tex = 0;


                            newObject.AddComponent<MeshRenderer>();
                            pos += 16;
                            pos += 12;
                            System.Array.Copy(resource, pos, buff, 0, 4);
                            pos += 4;
                            int i_null = System.BitConverter.ToInt32(buff, 0);
                            int j_null, i_null2;

                            if (i_null == 1)
                            {
                                System.Array.Copy(resource, pos, buff, 0, 4);
                                pos += 4;
                                j_null = System.BitConverter.ToInt32(buff, 0);
                                pos += 8;
                                tex = System.BitConverter.ToInt32(resource, pos);
                                pos += 4;
                                System.Array.Copy(resource, pos, buff, 0, 4);
                                pos += 4;
                                i_null2 = System.BitConverter.ToInt32(buff, 0);

                                if (j_null > 1)
                                {
                                    for (int i = 0; i < i_null2; i++)
                                    {
                                        pos += 16;
                                    }
                                }
                                else
                                {
                                    pos += 32;
                                }
                            }
                            else if (i_null == 2)
                            {
                                pos += 20;
                                for (int i = 0; i < 4; i++)
                                {
                                    pos += 28;
                                }

                                pos += 4;
                            }
                            else if ((i_null == 10) || (i_null == 6))
                            {
                                for (int i = 0; i < i_null; i++)
                                {
                                    pos += 16;
                                    System.Array.Copy(resource, pos, buff, 0, 4); pos += 4;
                                    i_null2 = System.BitConverter.ToInt32(buff, 0);
                                    for (int j = 0; j < i_null2; j++)
                                    {
                                        pos += 8;
                                    }
                                }
                            }

                            newObject.AddComponent<MeshFilter>().mesh = me;
                            newObject.GetComponent<MeshRenderer>().material = GetComponent<Materials>().maths[TexInts[tex]];
                            //newObject.AddComponent<rotationSprite>().camer = rootObj.transform.parent.GetComponent<GameManager>().currentCamera;

                        }
                        else if (type == 29)
                        {
                            pos += 16;
                            System.Array.Copy(resource, pos, buff, 0, 4); pos += 4;
                            int i_null = System.BitConverter.ToInt32(buff, 0);
                            pos += 4;
                            pos += 28;
                            if (i_null == 4)
                            {
                                pos += 4;
                            }
                            else
                            {
                                ;
                            }
                            pos += 4;
                        }
                        else if (type == 30)
                        {
                            newObject.AddComponent<MeshRenderer>();//OnWillRender()
                            pos += 16;
                            byte[] name = new byte[32];
                            //
                            System.Array.Copy(resource, pos, name, 0, 32);
                            pos += 32;

                            LoadTrigger go = newObject.AddComponent<LoadTrigger>();
                            go.roomName = System.Text.Encoding.UTF8.GetString(name).Replace("\x0", string.Empty);

                            byte[] position = new byte[12];
                            System.Array.Copy(resource, pos, position, 0, 12); pos += 12; //position
                            go.point0 = new Vector3(System.BitConverter.ToSingle(position, 0), System.BitConverter.ToSingle(position, 8), System.BitConverter.ToSingle(position, 4));
                            System.Array.Copy(resource, pos, position, 0, 12); pos += 12; //position
                            go.point1 = new Vector3(System.BitConverter.ToSingle(position, 0), System.BitConverter.ToSingle(position, 8), System.BitConverter.ToSingle(position, 4));
                            go.Trigger();
                            LoadTriggers.Add(newObject); //аар1

                        }
                        else if (type == 31)
                        {
                            pos += 16;
                            pos += 20;
                            pos += 20;
                            for (int i = 0; i < 27; i++)
                            {
                                pos += 8; //
                            }
                            pos += 4;
                        }
                        else if (type == 33)
                        {
                            bt.component = new Block33();
                            bt.component.thisObject = newObject;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 34)
                        {
                            pos += 16;
                            pos += 4;
                            System.Array.Copy(resource, pos, buff, 0, 4); pos += 4;
                            int i_null = System.BitConverter.ToInt32(buff, 0);
                            for (int i = 0; i < i_null; i++)
                            {
                                pos += 16;
                            }

                        }
                        else if (type == 35)
                        {
                            bt.component = newObject.AddComponent<Block35>();
                            ((Block35)bt.component).script = this;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 36)
                        {
                            nrml0 = 0;
                            nrml1 = 0;
                            pos += 16;
                            pos += 32;
                            pos += 32;
                            System.Array.Copy(resource, pos, buff, 0, 4);
                            pos += 4;
                            int i_null = System.BitConverter.ToInt32(buff, 0);
                            int j_null;
                            System.Array.Copy(resource, pos, buff, 0, 4);
                            pos += 4;
                            j_null = System.BitConverter.ToInt32(buff, 0);
                            vertices = new List<Vector3>();
                            UV = new List<Vector2>();
                            normals = new List<Vector3>();
                            if (i_null == 0)
                            {
                                ;
                            }
                            else if (i_null == 2)
                            {
                                for (int i = 0; i < j_null; i++)
                                {
                                    byte[] newBuff = new byte[32];
                                    System.Array.Copy(resource, pos, newBuff, 0, 32); pos += 32;
                                    vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                                    UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                                    normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
                                }
                            }
                            else if (i_null == 3)
                            {
                                for (int i = 0; i < j_null; i++)
                                {
                                    byte[] newBuff = new byte[24];
                                    //
                                    System.Array.Copy(resource, pos, newBuff, 0, 24);
                                    pos += 24;
                                    vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                                    UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                                    //
                                }
                            }
                            else if (i_null == 514)
                            {
                                for (int i = 0; i < j_null; i++)
                                {
                                    byte[] newBuff = new byte[48];
                                    //
                                    System.Array.Copy(resource, pos, newBuff, 0, 48);
                                    pos += 48;
                                    vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                                    UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                                    normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
                                    //
                                }
                            }
                            else if ((i_null == 258) || (i_null == 515))
                            {
                                for (int i = 0; i < j_null; i++)
                                {
                                    byte[] newBuff = new byte[40];
                                    //
                                    System.Array.Copy(resource, pos, newBuff, 0, 40);
                                    pos += 40;
                                    vertices.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4)));
                                    UV.Add(new Vector2(System.BitConverter.ToSingle(newBuff, 12), System.BitConverter.ToSingle(newBuff, 16)));
                                    normals.Add(new Vector3(System.BitConverter.ToSingle(newBuff, 20), System.BitConverter.ToSingle(newBuff, 24), System.BitConverter.ToSingle(newBuff, 28)));
                                    //
                                }
                            }
                            pos += 4;

                        }
                        else if (type == 37)
                        {
                            // bt.component = newObject.AddComponent<Block37>();
                            // bt.component.thisObject = newObject;
                            // ((Block37)bt.component).script = this;
                            // bt.component.Read(resource,ref pos);
                            bt.component = new Block37();
                            bt.component.thisObject = newObject;
                            ((Block37)bt.component).script = this;
                            bt.component.Read(resource, ref pos);
                        }
                        else if (type == 39)
                        {
                            pos += 16;
                            pos += 16;
                            pos += 4;
                            pos += 4;

                        }
                        else if (type == 40)
                        {
                            GeneratorInvoker GI = newObject.AddComponent<GeneratorInvoker>();
                            GI.resOb = rootObj;
                            List<float> Params = new List<float>();

                            byte[] newBuff = new byte[16];
                            //
                            System.Array.Copy(resource, pos, newBuff, 0, 16);
                            pos += 16;

                            Vector3 position = new Vector3(System.BitConverter.ToSingle(newBuff, 0), System.BitConverter.ToSingle(newBuff, 8), System.BitConverter.ToSingle(newBuff, 4));
                            newObject.transform.position = position;
                            GI.Scale = System.BitConverter.ToSingle(newBuff, 12);

                            pos += 32;  //null

                            byte[] GenBytes = new byte[32];
                            //
                            System.Array.Copy(resource, pos, GenBytes, 0, 32);
                            pos += 32;

                            string Generator = System.Text.Encoding.UTF8.GetString(GenBytes).Trim(new char[] { '\0' });

                            int gType = System.BitConverter.ToInt32(resource, pos); pos += 4;
                            int xz = System.BitConverter.ToInt32(resource, pos); pos += 4;
                            int paramCount = System.BitConverter.ToInt32(resource, pos); pos += 4;
                            for (int i = 0; i < paramCount - 2; i++)
                            {
                                Params.Add(System.BitConverter.ToSingle(resource, pos)); pos += 4;
                            }
                            int gType2 = System.BitConverter.ToInt32(resource, pos);
                            pos += 4;


                            pos += 4;

                            GI.invokeName = Generator;
                            GI.Type = gType;
                            GI.hernja = xz;
                            GI.Params = Params;
                            GI.Type2 = gType2;
                            //newObject.GetComponent<GeneratorInvoker>().Generate();
                            GI.Generate();
                        }
                        else
                        {
                            //Debug.Log("typeerror");
                            Debug.LogError("Type Error " + type.ToString() + "	" + System.Text.Encoding.UTF8.GetString(blockName) + "	");
                            break;
                        }
                        if (curLodObj != null)
                        {
                            if (curLodObj != newObject)
                                rends.Add(newObject.GetComponent<Renderer>());
                        }
                        /*
                        if (curSwitch != null)
                        {
                            //Debug.LogWarning(swIt);
                            if (curSwitch!=newObject)
                            {
                                curSwitch.GetComponent<BlockSwitcher>().Gobs[swIt].Add(newObject);
                            }
                        }
                        */
                        lastGameObject = newObject;
                    }
                    else
                    {

                        Debug.LogError("Case Error");
                        break;
                    }

                }
                foreach (var go in Blocks05)
                {
                    //Скорее всего это не нужно, но в блоке 05 есть упоминание данных блоков - возможно что в оригинале блоком 05 бралос управление над указаным блоком
                    string objName = go.GetComponent<Block05>().nameToJoin;
                    if (objName != "")
                    {
                        //GameObject.Find(go.GetComponent<JoinBlock>().name).transform.parent = go.transform;
                        GameObject obj = rootObj.transform.Find(objName).gameObject;
                        obj.SetActive(false);
                        obj = GameObject.Instantiate(obj);
                        obj.SetActive(true);
                        go.GetComponent<Block05>().obj = obj;
                        obj.transform.SetParent(go.transform);

                    }

                }
                Blocks05 = null;

            }
        }
        else
        {
            Debug.LogError("b3d is unknown");
        }
        ready = true;
        resource = null;
        br = null;

    }
    public Transform GetParentVertices(Transform gameObject)
    {
        try
        {
            List<Mesh> mesh = ((VerticesBlock)gameObject.parent.GetComponent<BlockType>().component).mesh;
            return gameObject.parent;
        }
        catch
        {
            return GetParentVertices(gameObject.parent);
        }
    }
    public void ClearB3D()
    {
        Debug.Log(this, gameObject);
        int childCount = gameObject.transform.childCount;
        Debug.Log(childCount);
        for (int i = 0; i < childCount; i++)
        {
            Debug.Log(gameObject.transform.GetChild(i) + " " + gameObject.transform.GetChild(i).gameObject.activeSelf);
            if (!gameObject.transform.GetChild(i).gameObject.activeSelf)
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {


    }

    public List<Vector3> Extrude(Vector3 a, Vector3 b, Vector3 c)
    {
        //mesh = GetComponent<MeshFilter>().sharedMesh;

        //Vector3 a = mesh.vertices[0];
        //Vector3 b = mesh.vertices[1];
        //Vector3 c = mesh.vertices[2];
        List<Vector3> newVerts = new List<Vector3>();

        Vector3 a0 = a + transform.position;
        Vector3 b0 = b + transform.position;
        Vector3 c0 = c + transform.position;

        Vector3 d = b0 - a0;
        Vector3 e = c0 - a0;

        Vector3 perp = Vector3.Cross(d, e);
        perp = perp * 0.001f;
        if (perp == new Vector3(0, 0, 0))
        {
            perp = new Vector3(0, -1, 0);
        }


        List<Vector3> verts = new List<Vector3>();
        List<int> tria = new List<int>();
        //verts.AddRange(mesh.vertices);
        //tria.AddRange(mesh.triangles);

        newVerts.Add(a + perp);
        newVerts.Add(b + perp);
        newVerts.Add(c + perp);

        //tria.Add(verts.Count-3);
        //tria.Add(verts.Count-2);
        //tria.Add(verts.Count-1);

        /*mesh.vertices = verts.ToArray();
		mesh.triangles = tria.ToArray();

		mesh.RecalculateBounds();

		GetComponent<MeshFilter>().sharedMesh = mesh;

		
		Debug.Log(perp);
		Debug.DrawRay(a,perp,Color.blue,10f); */
        return newVerts;
    }


}







class test : MonoBehaviour
{
    public List<Vector2> uvs = new List<Vector2>();
    public List<int> verts = new List<int>();
    public List<Vector3> vertexs = new List<Vector3>();
}



class rotationSprite : MonoBehaviour
{
    public GameObject camer;
    void Update()
    {
        transform.LookAt(camer.transform, new Vector3(0, 1, 0));
    }
}


enum DamageKey
{

    TopRight,
    Roof,
    TopLeft,
    Window,
    BotRight,
    Back,
    BotLeft,
    Front,
    Bottom
};