using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class KOTRManager : EditorWindow
{
    private static Shader _shader;
    private static Shader DefaultShader
    {
        get
        {
            if (!_shader)
            {
                _shader = SettingManager.DefaultShader;
            }
            return _shader;
        }
        set
        {
            if (value != _shader)
            {
                _shader = value;
                SettingManager.DefaultShader = _shader;
                EditorUtility.SetDirty(SettingManager.GetInstance());
            }
        }
    }
    // private static SettingManager _dbase; //TOREMOVE
    // private static SettingManager DataBase
    // {
    //     get
    //     {
    //         if (!_dbase)
    //         {
    //             _dbase = Resources.Load<SettingManager>("SettingManager");
    //             if (!_dbase)
    //             {
    //                 _dbase = ScriptableObject.CreateInstance<SettingManager>();
    //                 AssetDatabase.CreateAsset(_dbase, "Assets/Resources/SettingManager.asset");
    //                 AssetDatabase.SaveAssets();
    //             }
    //         }
    //         return _dbase;
    //     }
    // }


    private string _lastPath = "";
    private GameObject _root;
    private GameObject _currentScene;
    private bool _isDisabled = false;
    private GameObject target;
    private int submesh;
    private GameObject common, trucks, cabines;
    private List<GameObject> env = new List<GameObject>();
    private List<GameObject> envToRemove = new List<GameObject>();
    private bool isDisabled
    {
        get { return _isDisabled; }
        set
        {
            if (_isDisabled != value)
            {
                _isDisabled = value;
                GameManager.SetActiveBlocks(_isDisabled);
            }
        }
    }

    [MenuItem("KOTR Editor/KOTR Loader")]
    static void Init()
    {


        // string shdr = PlayerPrefs.GetString("shader", "");
        // shader = Shader.Find(shdr);


        //DefaultShader = DataBase.defaultShader;//TOREMOVE



        KOTRManager km = (KOTRManager)EditorWindow.GetWindow(typeof(KOTRManager));
        km.Show();
        // if (shader == null)
        // {
        //     shader = Shader.Find("Standard");
        // }
        GameManager.TC = DefaultShader;
        GameManager.TCu = DefaultShader;

        Block40.Register();

    }
    void OnGUI()
    {
        // if (!DefaultShader)
        // {
        //     DefaultShader = DataBase.defaultShader;
        // }
        DefaultShader = (Shader)EditorGUILayout.ObjectField(DefaultShader, typeof(Shader), false);
        GameManager.TC = DefaultShader;
        GameManager.TCu = DefaultShader;
        if (!_root)
        {
            _root = new GameObject("Root");
            GameManager gm = _root.AddComponent<GameManager>();
            GameManager.instance = gm;
        }

        GUILayout.Label("----TRUCKS----");
        GUILayout.Label("Under construction");
        GUILayout.Label("----CABINES----");
        GUILayout.Label("Under Construction");
        GUILayout.Label("----COMMON----");
        if (!common)
        {
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Open COMMON"))
            {
                common = OpenB3D(true);
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            common = (GameObject)EditorGUILayout.ObjectField(common, typeof(GameObject), true);
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Save"))
            {

            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Close"))
            {
                DestroyImmediate(common);
            }
            GUILayout.EndHorizontal();

        }
        GUI.backgroundColor = Color.white;

        GUILayout.Label("----ENV----");
        foreach (var item in env)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(item, typeof(GameObject), true);
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Save"))
            {
                string path = EditorUtility.SaveFilePanel("Выберите B3D сцену", _lastPath, item.name, "b3d");
                if (!string.IsNullOrEmpty(path))
                {
                    SceneSaver.SaveScene(item, path);
                }
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Close"))
            {
                envToRemove.Add(item);
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }
        foreach (var item in envToRemove)
        {
            GameObject tempObject = item;
            env.Remove(item);
            DestroyImmediate(tempObject);
        }
        envToRemove.Clear();


        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Open B3D"))
        {
            var tempObj = OpenB3D();
            if (tempObj)
                env.Add(tempObj);
        }
        if (GUILayout.Button("New scene B3D"))
        {
            GameObject tempObject = new GameObject();
            tempObject.transform.parent = _root.transform;
            tempObject.AddComponent<Resourcex>();
            tempObject.AddComponent<B3DScript>();
            tempObject.AddComponent<Materials>();
            tempObject.AddComponent<Texturefiles>();
            env.Add(tempObject);

        }
        GUILayout.Label("-----------");



        GUI.backgroundColor = Color.white;
        isDisabled = EditorGUILayout.Toggle("Включить инстанциируемые блоки", isDisabled);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Import Mesh to Scene"))
        {
            if (!target)
            {
                Debug.LogWarning("Необходимо выбрать обьект для сохранения");
                return;
            }
            ImportObject();
        }
        if (GUILayout.Button("Export to OBJ"))
        {
            if (!target)
            {
                Debug.LogWarning("Необходимо выбрать обьект для сохранения");
                return;
            }
            ExportObject();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save as prefab"))
        {
            if (!target)
            {
                Debug.LogWarning("Необходимо выбрать обьект для сохранения");
                return;
            }
            string objectName = target.name;
            if (string.IsNullOrEmpty(objectName))
            {
                objectName = "Object" + target.GetInstanceID();
            }
            MeshFilter[] meshes = target.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshes)
            {
                if (mf.sharedMesh)
                {
                    string MeshName = mf.name;
                    if (string.IsNullOrEmpty(MeshName))
                    {
                        MeshName = "Mesh" + mf.GetInstanceID();
                    }
                    AssetDatabase.CreateAsset(mf.sharedMesh, $"Assets/Meshes/{MeshName}.asset");
                }
            }
            PrefabUtility.SaveAsPrefabAsset(target, $"Assets/Prefabs/{objectName}.prefab");
        }

        target = (GameObject)EditorGUILayout.ObjectField("Target object:", (GameObject)target, typeof(GameObject), true);
        submesh = EditorGUILayout.IntField("Get Submesh:", submesh);
        if (target)
        {
            MeshFilter block = target.GetComponent<MeshFilter>();
            if (block)
            {

                if (GUILayout.Button("GetSubmesh"))
                {
                    Mesh me = block.sharedMesh;
                    Mesh newMesh = new Mesh();
                    newMesh.vertices = me.vertices;
                    if (submesh >= me.subMeshCount)
                    {
                        Debug.LogWarning("There is only " + submesh + " submeshes");
                        return;
                    }
                    newMesh.SetIndices(me.GetIndices(submesh), MeshTopology.Quads, 0);
                    GameObject newObject = new GameObject();
                    MeshRenderer mr = newObject.AddComponent<MeshRenderer>();
                    mr.materials = new Material[1];
                    newObject.AddComponent<MeshFilter>().mesh = newMesh;
                    newObject.transform.position = target.transform.position + Vector3.up;
                    newObject.transform.parent = target.transform.parent;

                }
                if (GUILayout.Button("Recalculate Normals"))
                {
                    Mesh me = block.sharedMesh;
                    me.RecalculateNormals();
                    me.RecalculateTangents();

                }

            }
        }
    }
    Material m_LineMaterial;
    Material m_QuadMaterial;
    private void InitMaterial(bool writeDepth)
    {
        if (!m_LineMaterial)
        {
            var shader = Shader.Find("Hidden/InternalLineColorful");
            m_LineMaterial = new Material(shader);
            m_LineMaterial.hideFlags = HideFlags.DontSave;
            var shader2 = Shader.Find("Hidden/InternalQuadColorful");
            m_QuadMaterial = new Material(shader2);
            m_QuadMaterial.hideFlags = HideFlags.DontSave;
        }
        // Set external depth on/off
        m_LineMaterial.SetInt("_ZTest", writeDepth ? 4 : 0);
        m_QuadMaterial.SetInt("_ZTest", writeDepth ? 4 : 0);
    }
    void ExportObject()
    {
        if (target == null)
        {
            Debug.LogError("Выберите target в KotrManager");
            return;
        }
        string path = EditorUtility.SaveFilePanel("Выберите куда сохранить", _lastPath, "", "obj");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        System.IO.TextWriter textWriter = System.IO.File.CreateText(path);
        MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>(true);
        foreach (var mf in meshFilters)
        {
            var name = mf.transform.parent.name;
            textWriter.WriteLine("o " + name);
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] faces = mesh.triangles;
            Vector3[] normals = mesh.normals;
            Vector2[] UVs = mesh.uv;
            foreach (var vertex in vertices)
            {
                textWriter.WriteLine(string.Format("v {0:F6} {1:F6} {2:F6}", vertex.x, vertex.y, vertex.z));
            }
            if (UVs.Length > 0)
            {
                foreach (var uv in UVs)
                {
                    textWriter.WriteLine(string.Format("vt {0:F6} {1:F6}", uv.x, uv.y));
                }
            }
            foreach (var normal in normals)
            {
                textWriter.WriteLine(string.Format("vn {0:F6} {1:F6} {2:F6}", normal.x, normal.y, normal.z));

            }
            string lastMat = "None";
            IMeshInfo mr = mf.GetComponent<IMeshInfo>();
            Materials mts = mf.GetComponentInParent<Materials>();
            foreach (int mt in mr.Materials)
            {
                string matName = mts.material[mt].Split(' ')[0];
                if (lastMat != matName)
                {
                    lastMat = matName;
                }

            }

            textWriter.WriteLine("usemtl " + lastMat); //TODO: разные материалы к группам вершин
            textWriter.WriteLine("s off");
            if (faces.Length % 3 == 0)
            {
                for (int i = 0; i < faces.Length / 3; i++)
                {
                    textWriter.WriteLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", faces[i * 3 + 0] + 1, faces[i * 3 + 1] + 1, faces[i * 3 + 2] + 1));
                }
            }


        }
        textWriter.Close();
    }
    void ImportObject()
    {
        Gigableh.MeshApplyTransform.ApplyTransformRecursive(target.transform, true, true, true);

        ImportRecursively(target.transform);

    }
    void ImportRecursively(Transform transform)
    {
        if (PrefabUtility.GetPrefabInstanceHandle(transform) != null || PrefabUtility.GetCorrespondingObjectFromSource(transform) != null)
        {
            throw new System.Exception("Object is a prefab. Unpack before importing.");
        }
        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            BlockType bt = transform.gameObject.AddComponent<Block05>();
            bt.Type = 5;
            ((IBlocktype)bt).thisObject = transform.gameObject;
        }
        else if (transform.GetComponent<MeshRenderer>())
        {

            GameObject b37 = new GameObject(transform.name);
            b37.transform.parent = transform.parent;
            b37.transform.SetSiblingIndex(transform.GetSiblingIndex()); // Необходимо, что бы индекс нового обьекта (37, вершинного) совпадал с индексом обрабатываемого обьекта, иначе рекурсивный импорт будет работать через-один



            BlockType bt = b37.AddComponent<Block37>();
            bt.Type = 37;
            //bt = new Block37();
            ((IBlocktype)bt).thisObject = b37;
            IVerticesBlock ivb = (IVerticesBlock)bt;
            ivb.vertices.AddRange(meshFilter.sharedMesh.vertices);
            ivb.normals.AddRange(meshFilter.sharedMesh.normals);
            ivb.uv.AddRange(meshFilter.sharedMesh.uv);
            ivb.uv1.AddRange(meshFilter.sharedMesh.uv2);

            transform.parent = b37.transform;
            bt = transform.gameObject.AddComponent<Block35>();
            bt.Type = 35;
            //bt = transform.gameObject.AddComponent<Block35>();
            ((Block35)bt).i_null = 3;
            transform.name = ""; // Просто придержимся традиции софтклаба - не оставлять имена 35 блока

        }
        else if (transform.GetComponent<MeshCollider>())
        {
            BlockType bt = transform.gameObject.AddComponent<Block23>();
            bt.Type = 23;
            bt = (Block23)bt;
            ((IBlocktype)bt).thisObject = transform.gameObject;

            transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh; //апдейтнуть


        }

        foreach (Transform newTransform in transform)
        {
            ImportRecursively(newTransform);
        }
    }
    void OnSelectionChange()
    {
        target = Selection.activeGameObject;
        Repaint();
    }
    GameObject OpenB3D(bool isCommon = false)
    {
        GameObject tempObj = null;
        string path = EditorUtility.OpenFilePanel("Выберите B3D сцену", _lastPath, "b3d");
        //if (isCommon || !isCommon && GameManager.common)
            if (!string.IsNullOrEmpty(path))
            {
                string[] splitted = path.Split('/');
                string sceneName = splitted[splitted.Length - 1];
                sceneName = sceneName.Substring(0, sceneName.Length - 4);
                _currentScene = new GameObject(sceneName);
                tempObj = _currentScene;
                _currentScene.transform.SetParent(_root.transform);
                string resPath = path.Substring(0, path.Length - 3);
                resPath = resPath + "res";
                Resourcex res = _currentScene.AddComponent<Resourcex>();
                res.file = new System.IO.FileInfo(resPath);
                B3DScript b3d = _currentScene.AddComponent<B3DScript>();
                b3d.file = new System.IO.FileInfo(path);
                if (isCommon)
                {
                    GameManager.common = _currentScene;
                    _currentScene = null;

                }
                else
                {
                    GameManager.currentObject = _currentScene;
                }
                //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //AssetDatabase.ImportAsset(path);

                res.StartRes();

                b3d.StartB3D();

            }
        return tempObj;
    }
    System.Collections.IEnumerator ClearConsole()
    {
        // wait until console visible
        while (!Debug.developerConsoleVisible)
        {
            yield return null;
        }
        yield return null; // this is required to wait for an additional frame, without this clearing doesn't work (at least for me)
        Debug.ClearDeveloperConsole();
    }

    void OnDrawGizmos()
    {


    }

}