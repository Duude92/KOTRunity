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
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    [MenuItem("Window/KOTR Loader")]
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
        if (GUILayout.Button("Import Mesh to Scene"))
        {
            ImportObject();
        }
        target = (GameObject)EditorGUILayout.ObjectField("Target object:", (GameObject)target, typeof(GameObject), true);
        submesh = EditorGUILayout.IntField("Get Submesh:", submesh);
        if (target)
        {
            Block08 block = target.GetComponent<Block08>();
            if (block)
            {

                if (GUILayout.Button("GetSubmesh"))
                {
                    Mesh me = target.GetComponent<MeshFilter>().sharedMesh;
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
            bt.component = transform.gameObject.AddComponent<Block05>();
        }
        else
        {

            GameObject b37 = new GameObject(transform.name);
            b37.transform.parent = transform.parent;
            b37.transform.SetSiblingIndex(transform.GetSiblingIndex()); // Необходимо, что бы индекс нового обьекта (37, вершинного) совпадал с индексом обрабатываемого обьекта, иначе рекурсивный импорт будет работать через-один



            BlockType bt = b37.AddComponent<BlockType>();
            bt.Type = 37;
            bt.component = new Block37();
            bt.component.thisObject = b37;
            ((VerticesBlock)bt.component).mesh.Add(meshFilter.sharedMesh);

            transform.parent = b37.transform;
            bt = transform.gameObject.AddComponent<BlockType>();
            bt.Type = 35;
            bt.component = transform.gameObject.AddComponent<Block35>();
            transform.name = ""; // Просто придержимся традиции софтклаба - не оставлять имена 35 блока
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