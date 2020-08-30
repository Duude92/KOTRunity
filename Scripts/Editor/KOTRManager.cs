using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
class KOTRManager : EditorWindow
{
    public static Shader tc;
    private Shader oldShader;


    private string _lastPath = "";
    private GameObject _root;
    private GameObject _currentScene;
    private bool _isDisabled = false;
    private GameObject target;
    private int submesh;
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
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    [MenuItem("KOTR Editor/KOTR Loader")]
    static void Init()
    {
        string shdr = PlayerPrefs.GetString("shader", "");
        tc = Shader.Find(shdr);
        KOTRManager km = (KOTRManager)EditorWindow.GetWindow(typeof(KOTRManager));
        km.Show();
        if (tc == null)
        {
            tc = Shader.Find("Standard");
        }
        GameManager.TC = tc;
        GameManager.TCu = tc;

        Block40.Register();

    }
    void OnGUI()
    {

        if (tc != oldShader && tc != null)
        {
            oldShader = tc;
            PlayerPrefs.SetString("shader", tc.name);
        }
        tc = (Shader)EditorGUILayout.ObjectField(tc, typeof(Shader), false);
        GameManager.TC = tc;
        GameManager.TCu = tc;
        if (!_root)
        {
            _root = new GameObject("Root");
            GameManager gm = _root.AddComponent<GameManager>();
            GameManager.instance = gm;
        }

        if (GUILayout.Button("Open COMMON"))
        {
            OpenB3D(true);
        }

        if (GUILayout.Button("Open B3D"))
        {
            if (!_currentScene)
                OpenB3D();

        }

        if (GUILayout.Button("Save Scene"))
        {
            string path = EditorUtility.SaveFilePanel("Выберите B3D сцену", _lastPath, _lastPath, "b3d");
            if (!string.IsNullOrEmpty(path))
            {
                SceneSaver.SaveScene(_currentScene, path);
            }
        }
        isDisabled = EditorGUILayout.Toggle("Включить инстанциируемые блоки", isDisabled);
        if (GUILayout.Button("Close B3D"))
        {
            GameObject.DestroyImmediate(_root);
            ClearConsole();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Import Mesh to Scene"))
        {
            ImportObject();
        }
        if (GUILayout.Button("Export to OBJ"))
        {
            ExportObject();
        }
        GUILayout.EndHorizontal();
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
    public void OnSceneGUI(SceneView view)
    {
        if (false)//(target)
        {
            Block08 block = target.GetComponent<Block08>();
            if (block)
            {
                foreach (Vector3 vector in block.vertices)
                {
                    Mesh now = new Mesh();
                    now.vertices = new Vector3[4] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1) };
                    now.triangles = new int[6] { 0, 2, 1, 3, 1, 2 };
                    now.uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

                    now.RecalculateBounds();
                    if (!m_QuadMaterial)
                    {
                        InitMaterial(true);
                    }
                    m_QuadMaterial.SetPass(0);
                    Graphics.DrawMeshNow(now, vector, Quaternion.Euler(-Vector3.up), 0);
                }
            }
        }
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
                textWriter.WriteLine(string.Format("v {0} {1} {2}", vertex.x, vertex.y, vertex.z));
            }
            if (UVs.Length > 0)
            {
                foreach (var uv in UVs)
                {
                    textWriter.WriteLine(string.Format("vt {0} {1}", uv.x, uv.y));
                }
            }
            foreach (var normal in normals)
            {
                textWriter.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
            }
            textWriter.WriteLine("usemtl None");
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
            BlockType bt = transform.gameObject.AddComponent<BlockType>();
            bt.Type = 5;
            bt = transform.gameObject.AddComponent<Block05>();
            ((IBlocktype)bt).thisObject = transform.gameObject;
        }
        else if (transform.GetComponent<MeshRenderer>())
        {

            GameObject b37 = new GameObject(transform.name);
            b37.transform.parent = transform.parent;
            b37.transform.SetSiblingIndex(transform.GetSiblingIndex()); // Необходимо, что бы индекс нового обьекта (37, вершинного) совпадал с индексом обрабатываемого обьекта, иначе рекурсивный импорт будет работать через-один



            BlockType bt = b37.AddComponent<BlockType>();
            bt.Type = 37;
            bt = new Block37();
            ((IBlocktype)bt).thisObject = b37;
            IVerticesBlock ivb = (IVerticesBlock)bt;
            ivb.vertices.AddRange(meshFilter.sharedMesh.vertices);
            ivb.normals.AddRange(meshFilter.sharedMesh.normals);
            ivb.uv.AddRange(meshFilter.sharedMesh.uv);
            ivb.uv1.AddRange(meshFilter.sharedMesh.uv2);

            transform.parent = b37.transform;
            bt = transform.gameObject.AddComponent<BlockType>();
            bt.Type = 35;
            bt = transform.gameObject.AddComponent<Block35>();
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
    void OpenB3D(bool isCommon = false)
    {
        string path = EditorUtility.OpenFilePanel("Выберите B3D сцену", _lastPath, "b3d");
        if (isCommon || !isCommon && GameManager.common)
            if (!string.IsNullOrEmpty(path))
            {
                string[] splitted = path.Split('/');
                string sceneName = splitted[splitted.Length - 1];
                sceneName = sceneName.Substring(0, sceneName.Length - 4);
                _currentScene = new GameObject(sceneName);
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
                res.StartRes();

                b3d.StartB3D();

            }
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