using UnityEngine;
using UnityEditor;
public class SettingManager : ScriptableObject
{
    public Shader defaultShader;
    private static SettingManager _dbase;
    private static Shader _shader;
    public static SettingManager GetInstance()
    {
        return _dbase;
    }
    
    public static Shader DefaultShader
    {
        get
        {
            if (!_shader)
            {
                _shader = DataBase.defaultShader;
            }
            return _shader;
        }
        set
        {
            if (value != _shader)
            {
                _shader = value;
                DataBase.defaultShader = _shader;
            }
        }
    }
    private static SettingManager DataBase
    {
        get
        {
            if (!_dbase)
            {
                _dbase = Resources.Load<SettingManager>("SettingManager");
                if (!_dbase)
                {
                    _dbase = ScriptableObject.CreateInstance<SettingManager>();
                    AssetDatabase.CreateAsset(_dbase, "Assets/Resources/SettingManager.asset");
                    AssetDatabase.SaveAssets();
                }
            }
            return _dbase;
        }
    }


}