using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class B3DScript : MonoBehaviour
{
    enum SceneCase
    {
        SCENE_BEGIN = 111,
        SCENE_END = 222,
        NEW_OBJECT = 333,
        BLOCK_BREAK = 444,
        OBJECT_END = 555
    }
    public List<Vector3> triggerBox = new List<Vector3>();
    BinaryReader br;
    public List<int> Materials = new List<int>();
    public List<GameObject> SwitchBlocks = new List<GameObject>();
    public List<GameObject> InvokeBlocks = new List<GameObject>();
    public List<GameObject> Blocks05 = new List<GameObject>();
    public List<GameObject> Rooms = new List<GameObject>();
    public List<GameObject> LoadTriggers = new List<GameObject>();
    public FileInfo file;
    public bool ready = false;
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
        InvokeBlocks = new List<GameObject>();

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
            BlockFabrique.script = this;
            pos += 20;
            int TexNum = System.BitConverter.ToInt32(resource, pos);
            pos += 4;
            Texture2D[] texture = new Texture2D[TexNum];

            Sprite[] sprites = new Sprite[TexNum];
            GameObject curLodObj = null;
            short lodCnt = 0;
            List<LOD> lods = new List<LOD>();
            List<Renderer> rends = new List<Renderer>();


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

                Materials.Add(a);
                texture[i] = new Texture2D(1, 1);
                texture[i].name = System.Text.Encoding.UTF8.GetString(texName);

                sprites[i] = Sprite.Create(texture[i], new Rect(0, 0, 1, 1), new Vector2(0f, 0f), 100);
            }
            //int Ccase = System.BitConverter.ToInt32(resource, pos);
            SceneCase Ccase = (SceneCase)System.BitConverter.ToInt32(resource, pos);
            pos += 4;
            if (Ccase == SceneCase.SCENE_BEGIN)
            {
                GameObject lastGameObject = null;
                GameObject newObject = null;
                GameObject rootObj = gameObject;

                for (; ; )
                {
                    Ccase = (SceneCase)System.BitConverter.ToInt32(resource, pos);
                    pos += 4;
                    if (Ccase == SceneCase.SCENE_END)
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
                    else if (Ccase == SceneCase.BLOCK_BREAK)
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
                        var bt = lastGameObject.GetComponent<BlockType>();
                        if (bt)
                        {
                            if (bt.Type == 10)
                            {

                                /*float perc = Distance/(2000f+lodCnt);
                                Debug.Log(perc);

                                lods.Add(new LOD(perc,rends.ToArray()));*/
                                lods.Add(new LOD(1f / ((lodCnt + 1) * 5), rends.ToArray()));
                                rends = new List<Renderer>();
                                lodCnt++;
                            }
                            bt.ComaEvent();
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
                    else if (Ccase == SceneCase.OBJECT_END)
                    {
                        lastGameObject.GetComponent<BlockType>().ClosingEvent();


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
                    else if (Ccase == SceneCase.NEW_OBJECT)
                    {
                        byte[] blockName = new byte[32];
                        System.Array.Copy(resource, pos, blockName, 0, 32);
                        pos += 32;
                        string s = System.Text.Encoding.UTF8.GetString(blockName).Trim(new char[] { '\0' });

                        int type = System.BitConverter.ToInt32(resource, pos);
                        pos += 4;
                        newObject = new GameObject(s);
                        // BlockType bt = newObject.AddComponent<DefaultBlock>();
                        // bt.Type = type;

                        if (lastGameObject != null)
                        {
                            newObject.transform.SetParent(lastGameObject.transform);
                        }
                        else
                        {
                            newObject.transform.SetParent(rootObj.transform);
                            lastGameObject = rootObj;
                        }

                        //--------
                        BlockType blockType = BlockFabrique.GetBlock(newObject, type);
                        ((IBlocktype)blockType).Read(resource, ref pos);
                        //--------


                        

                        if (curLodObj != null)
                        {
                            if (curLodObj != newObject)
                                rends.Add(newObject.GetComponent<Renderer>());
                        }
                        lastGameObject = newObject;
                    }
                    else
                    {

                        Debug.LogError("Case Error  at  " + pos, newObject);
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

        IVerticesBlock ivb = gameObject.parent.GetComponent<BlockType>() as IVerticesBlock;
        if (ivb != null)
            return gameObject.parent;
        else
            return GetParentVertices(gameObject.parent);

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



// class rotationSprite : MonoBehaviour
// {
//     public GameObject camer;
//     void Update()
//     {
//         transform.LookAt(camer.transform, new Vector3(0, 1, 0));
//     }
// }


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
