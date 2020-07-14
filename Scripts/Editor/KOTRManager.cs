using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
class KOTRManager : EditorWindow
{
    private string _lastPath = "";
    private GameObject _root;
    public static Shader tc;
    private GameObject _currentScene;

    private bool _isDisabled = false;
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



    [MenuItem("Window/KOTR Loader")]
    static void Init()
    {
        tc = Shader.Find( PlayerPrefs.GetString("shader",""));
        KOTRManager km = (KOTRManager)EditorWindow.GetWindow(typeof(KOTRManager));
        km.Show();
        if (tc==null)
        {
            tc = Shader.Find("Standard");
        }
        GameManager.TC = tc;
        GameManager.TCu = tc;

    }
    void OnGUI()
    {
        tc = (Shader)EditorGUILayout.ObjectField(tc, typeof(Shader), false);
        GameManager.TC = tc;
        GameManager.TCu = tc;
        if (!_root)
        {
            _root = new GameObject("Root");
            GameManager gm = _root.AddComponent<GameManager>();
            tc = Shader.Find("Default");
            GameManager.instance = gm;
        }


        if (GUILayout.Button("Open B3D"))
        {
            if (!_currentScene)
                OpenB3D();

        }
        if (GUILayout.Button("Open COMMON"))
        {
            OpenB3D(true);
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
    }
    void OpenB3D(bool isCommon = false)
    {
        string path = EditorUtility.OpenFilePanel("Выберите B3D сцену", _lastPath, "b3d");
        if(isCommon||!isCommon&&GameManager.common)
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

}