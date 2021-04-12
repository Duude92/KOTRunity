using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(InvokeMe))]
class InvokeMeEditor : Editor
{
    InvokeMe targetIM;
    void OnEnable()
    {
        targetIM = (InvokeMe)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Invoke"))
        {
            targetIM.Invoke();
        }

    }

}