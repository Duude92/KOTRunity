using UnityEditor;
[CustomEditor(typeof(Block18))]
class Block18Editor : Editor
{
    Block18 target18;
    void OnSceneGUI()
    {
        target18 = (Block18)target;
        target18.gizmosTransform.position = target18.transform.position;
    }

}